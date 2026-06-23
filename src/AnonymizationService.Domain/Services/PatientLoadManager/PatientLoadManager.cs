using AnonymizationService.ClinicalDocuments;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.Enums;
using AnonymizationService.Localization;
using AnonymizationService.PatientUploadRequests;
using AnonymizationService.Services.Dicom;
using AnonymizationService.Services.Notifications;
using AnonymizationService.StateMachines.LoadPatientData;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace AnonymizationService.Services.PatientLoadManager
{
    public class PatientLoadManager : IPatientLoadManager, ISingletonDependency
    {
        private const int MAX_NUMBER_OF_PARALLEL_WORKERS = 3;

        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<(UploadRequestParameter uploadRequestParameter, LoadPatientDataStateMachine StateMachine)> _stateMachineQueue = new();
        private readonly ConcurrentDictionary<Guid, UploadRequestParameter> _stateMachinesInProcess = new();

        private readonly StateMachineManager _stateMachineManager;
        private readonly PatientUploadRequestService _patientUploadRequestService;
        private readonly INotifyer _notifyer;
        private readonly IStringLocalizer<AnonymizationServiceResource> _stringLocalizer;
        private readonly IDefaultImportParameterRepository _defaultImportParameterRepository;
        private readonly IGuidGenerator _guidGenerator;

        private readonly IPatientUploadRequestRepository _patientUploadRequestRepository;
        private readonly IRepository<StateMachine> _stateMachineRepository;

        private class UploadRequestParameter
        {
            public Guid UploadRequestId { get; set; }
            public string MasterPatientIndex { get; set; }
        }

        public PatientLoadManager(StateMachineManager stateMachineManager
            , PatientUploadRequestService patientUploadRequestService
            , INotifyer notifyer
            , IStringLocalizer<AnonymizationServiceResource> stringLocalizer
            , IDefaultImportParameterRepository defaultImportParameterRepository
            , IPatientUploadRequestRepository patientUploadRequestRepository
            , IGuidGenerator guidGenerator
            , IRepository<StateMachine> stateMachineRepository)
        {
            _semaphore = new SemaphoreSlim(MAX_NUMBER_OF_PARALLEL_WORKERS);
            _stateMachineManager = stateMachineManager;
            _notifyer = notifyer;
            _stringLocalizer = stringLocalizer;
            _patientUploadRequestService = patientUploadRequestService;
            _defaultImportParameterRepository = defaultImportParameterRepository;
            _patientUploadRequestRepository = patientUploadRequestRepository;
            _guidGenerator = guidGenerator;
            _stateMachineRepository = stateMachineRepository;
        }

        public async Task LoadBatchPatientsAsync(IList<PatientUploadInput> patientUploadInputList, Guid userId)
        {
            var batchUploadRequest = await _patientUploadRequestService.CreateBatchUploadRequestAsync(userId);

            foreach (var patientUploadInput in patientUploadInputList)
            {
                var patientUploadRequestParameters = await GetPatientUploadRequestParameters(patientUploadInput);
                await CreateAndEnqueueUploadRequestAsync(userId, patientUploadRequestParameters, batchUploadRequest.Id);
                await TryRunNextStateMachine();
            }
        }

        public async Task LoadNewPatientAsync(PatientUploadInput patientUploadInput, Guid userId)
        {
            var patientUploadRequestParameters = await GetPatientUploadRequestParameters(patientUploadInput);
            await CreateAndEnqueueUploadRequestAsync(userId, patientUploadRequestParameters);
            await TryRunNextStateMachine();
        }

        public async Task ReloadPatientUploadRequestAsync(PatientUploadRequest patientUploadRequest)
        {
            var patientUploadRequestParameters = new PatientUploadRequestParameters
            {
                ClinicalStateMachineStartTime = patientUploadRequest.ClinicalStateMachineStartTime,
                ClinicalStateMachineDocumentType = patientUploadRequest.ClinicalStateMachineDocumentType,
                DicomStateMachineStartTime = patientUploadRequest.DicomStateMachineStartTime,
                DicomStateMachinePacsSource = patientUploadRequest.DicomPacsSource,
                DicomStateMachineEndTime = patientUploadRequest.DicomStateMachineEndTime,
                DicomStateMachineModalities = patientUploadRequest.DicomStateMachineModalities,
                DicomStateMachineParametersJson = JsonConvert.DeserializeObject<DicomParametersJson>(patientUploadRequest.DicomStateMachineParameterJson),
                LaboratoryStateMachineStartTime = patientUploadRequest.LaboratoryStateMachineStartTime,
                DbUriStateMachineStartTime = patientUploadRequest.DbUriStateMachineStartTime,
                UseDefaultClinicalStartTime = patientUploadRequest.ClinicalStartTimeIsDefault,
                UseDefaultClinicalDocumentType = patientUploadRequest.ClinicalDocumentTypeAreDefault,
                UseDefaultDicomStartTime = patientUploadRequest.DicomStartTimeIsDefault,
                UseDefaultDicomModalities = patientUploadRequest.DicomModalitiesAreDefault,
                IntensiveCareStateMachineStartTime = patientUploadRequest.IntensiveCareStateMachineStartTime,
                IntensiveCareStateMachineNosologicalCode = patientUploadRequest.IntensiveCareStateMachineNosologicalCode,
                IntensiveCareStateMachineShouldStart = patientUploadRequest.IntensiveCareStateMachineShouldStart,
                RedcapStateMachineStartTime = patientUploadRequest.RedcapStateMachineStartTime,
                RedcapStateMachineShouldStart = patientUploadRequest.RedcapStateMachineShouldStart,
                RedcapStateMachineRedcapStudyConfigurationId = patientUploadRequest.RedcapStateMachineRedcapStudyConfigurationId,
                RedcapStateMachineRecordId = patientUploadRequest.RedcapStateMachineRecordId,
                UseDefaultLaboratoryStartTime = patientUploadRequest.LaboratoryStartTimeIsDefault,
                UseDefaultIntensiveCareStartTime = patientUploadRequest.IntensiveCareStartTimeIsDefault,
                UseDefaultRedcapStartTime = patientUploadRequest.RedcapStartTimeIsDefault,
                ClinicalStateMachineShouldStart = patientUploadRequest.ClinicalStateMachineShouldStart,
                DicomStateMachineShouldStart = patientUploadRequest.DicomStateMachineShouldStart,
                LaboratoryStateMachineShouldStart = patientUploadRequest.LaboratoryStateMachineShouldStart,
                MasterPatientIndex = patientUploadRequest.MasterPatientIndex,
                StudyId = patientUploadRequest.StudyId,
            };

            await CreateAndEnqueueUploadRequestAsync(Guid.Empty, patientUploadRequestParameters);
            await TryRunNextStateMachine();
        }

        public async Task HandleStateMachineCompletionAsync(Guid stateMachineId, string errorMessage)
        {
            if (_stateMachinesInProcess.TryRemove(stateMachineId, out UploadRequestParameter uploadRequestParameter))
            {
                _semaphore.Release();

                var uploadRequest = await _patientUploadRequestService.SetResultForUploadRequestAsync(uploadRequestParameter.UploadRequestId, errorMessage);

                if (uploadRequest.BatchRequestId is not null)
                {
                    var runningRequests = await _patientUploadRequestService.GetRunningPatientUploadRequestsForBatchId(uploadRequest.BatchRequestId.Value);

                    if (runningRequests.Count == 0)
                    {
                        var batchRequest = await _patientUploadRequestService.TrySetBatchUploadCompletedAsync(uploadRequest.BatchRequestId.Value);
                        await SendNotificationForBatchUploadRequestToUser(batchRequest.CreatorId);
                    }
                }
                else
                {
                    if (uploadRequest.Status == PatientUploadStatus.Success)
                    {
                        await SendNotificationForSuccessfulUploadRequest(uploadRequest.CreatorId);
                    }

                    if (uploadRequest.Status == PatientUploadStatus.Failed)
                    {
                        await SendNotificationForFailedUploadRequest(uploadRequest.CreatorId);
                    }
                }

                await TryRunNextStateMachine();
            }
        }

        private (bool, T) GetParameterToUse<T>(T inputParameter, T defaultParameter, bool shouldStart)
        {
            if (shouldStart && inputParameter is null)
            {
                return (true, defaultParameter);
            }
            if (shouldStart && inputParameter is string && ((string)(object)inputParameter).IsNullOrWhiteSpace())
            {
                return (true, defaultParameter);
            }
            return (false, inputParameter);
        }

        private async Task<PatientUploadRequestParameters> GetPatientUploadRequestParameters(PatientUploadInput patientUploadInput)
        {
            var defaultImportParameters = await _defaultImportParameterRepository.GetListAsync();
            var defaultImportStartDate = DateTime.ParseExact(defaultImportParameters.Where(p => p.Name == "ImportStartDate").FirstOrDefault()?.Value, "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.None);
            var defaultModalities = defaultImportParameters.FirstOrDefault(p => p.Name == "Modalities").Value;
            //var defaultDocumentType = defaultImportParameters.Where(p => p.Name == "DocumentType").FirstOrDefault().Value;
            var defaultDocumentType = new ClinicalDocumentTypeFilter[0];

            var (useDefaultClinicalStartTime, clinicalStartTime) = GetParameterToUse(
                patientUploadInput.ClinicalStateMachineStartTime,
                defaultImportStartDate,
                patientUploadInput.ClinicalStateMachineShouldStart);

            var (useDefaultClinicalDocumentType, clinicalDocumentType) = GetParameterToUse(
              patientUploadInput.ClinicalStateMachineDocumentType,
              defaultDocumentType,
              patientUploadInput.ClinicalStateMachineShouldStart);


            var (useDefaultDicomStartTime, dicomStartTime) = GetParameterToUse(
                patientUploadInput.DicomStateMachineStartTime,
                defaultImportStartDate,
                patientUploadInput.DicomStateMachineShouldStart);
            var dicomPacsSource = patientUploadInput.DicomPacsSource;
            var dicomEndTime = patientUploadInput.DicomStateMachineEndTime;
            var dicomParameterJson = patientUploadInput.DicomParametersJson;

            var (useDefaultDicomModalities, dicomModalities) = GetParameterToUse(
               string.Join(',', patientUploadInput.DicomStateMachineModalities),
               defaultModalities,
               patientUploadInput.DicomStateMachineShouldStart);

            var (useDefaultLaboratoryStartTime, laboratoryStartTime) = GetParameterToUse(
                patientUploadInput.LaboratoryStateMachineStartTime,
                defaultImportStartDate,
                patientUploadInput.LaboratoryStateMachineShouldStart);

			var (useDefaultDbUriStartTime, dbUriStartTime) = GetParameterToUse(
			    patientUploadInput.DbUriStateMachineStartTime,
			    defaultImportStartDate,
			    patientUploadInput.DbUriStateMachineShouldStart);

			var (useDefaultPathoxStartTime, pathoxStartTime) = GetParameterToUse(
				patientUploadInput.PathoxStateMachineStartTime,
				defaultImportStartDate,
				patientUploadInput.PathoxStateMachineShouldStart);

            var (useDefaultIntensiveCareStartTime, intensiveCareStartTime) = GetParameterToUse(
                patientUploadInput.IntensiveCareStateMachineStartTime,
                defaultImportStartDate,
                patientUploadInput.IntensiveCareStateMachineShouldStart);

            var (useDefaultRedcapStartTime, redcapStartTime) = GetParameterToUse(
                patientUploadInput.RedcapStateMachineStartTime,
                defaultImportStartDate,
                patientUploadInput.RedcapStateMachineShouldStart);

            return new PatientUploadRequestParameters
            {
                ClinicalStateMachineStartTime = clinicalStartTime,
                ClinicalStateMachineDocumentType = clinicalDocumentType,
                DicomStateMachineStartTime = dicomStartTime,
                DicomStateMachineEndTime = dicomEndTime,
                DicomStateMachineModalities = dicomModalities,
                DicomStateMachinePacsSource = dicomPacsSource,
                DicomStateMachineParametersJson = dicomParameterJson,
                LaboratoryStateMachineStartTime = laboratoryStartTime,
                UseDefaultClinicalStartTime = useDefaultClinicalStartTime,
                UseDefaultClinicalDocumentType = useDefaultClinicalDocumentType,
                UseDefaultDicomStartTime = useDefaultDicomStartTime,
                UseDefaultDicomModalities = useDefaultDicomModalities,
                UseDefaultLaboratoryStartTime = useDefaultLaboratoryStartTime,
                UseDefaultDbUriStartTime = useDefaultDbUriStartTime,
				UseDefaultPathoxStartTime = useDefaultPathoxStartTime,
                UseDefaultIntensiveCareStartTime = useDefaultIntensiveCareStartTime,
                UseDefaultRedcapStartTime = useDefaultRedcapStartTime,
				ClinicalStateMachineShouldStart = patientUploadInput.ClinicalStateMachineShouldStart,
                DicomStateMachineShouldStart = patientUploadInput.DicomStateMachineShouldStart,
                LaboratoryStateMachineShouldStart = patientUploadInput.LaboratoryStateMachineShouldStart,
                DbUriStateMachineShouldStart = patientUploadInput.DbUriStateMachineShouldStart,
                DbUriStateMachineStartTime = dbUriStartTime,
				PathoxStateMachineShouldStart = patientUploadInput.PathoxStateMachineShouldStart,
				PathoxStateMachineStartTime = pathoxStartTime,
                IntensiveCareStateMachineShouldStart = patientUploadInput.IntensiveCareStateMachineShouldStart,
                IntensiveCareStateMachineStartTime = intensiveCareStartTime,
                IntensiveCareStateMachineNosologicalCode = patientUploadInput.IntensiveCareStateMachineNosologicalCode,
                RedcapStateMachineShouldStart = patientUploadInput.RedcapStateMachineShouldStart,
                RedcapStateMachineStartTime = redcapStartTime,
                RedcapStateMachineRedcapStudyConfigurationId = patientUploadInput.RedcapStateMachineRedcapStudyConfigurationId,
                RedcapStateMachineRecordId = patientUploadInput.RedcapStateMachineRecordId,
                MasterPatientIndex = patientUploadInput.MasterPatientIndex,
                StudyId = patientUploadInput.StudyId
            };
        }

        private async Task CreateAndEnqueueUploadRequestAsync(Guid userId, PatientUploadRequestParameters patientUploadRequestParameters, Guid? batchId = null)
        {
            var stateMachineParameters = new LoadPatientDataStateMachineParameters
            {
                MasterPatientIndex = patientUploadRequestParameters.MasterPatientIndex,
                StudyId = patientUploadRequestParameters.StudyId,
                ClinicalStateMachineShouldStart = patientUploadRequestParameters.ClinicalStateMachineShouldStart,
                ClinicalStateMachineStartTime = patientUploadRequestParameters.ClinicalStateMachineStartTime,
                ClinicalStateMachineDocumentType = patientUploadRequestParameters.ClinicalStateMachineDocumentType,
                DicomStateMachineShouldStart = patientUploadRequestParameters.DicomStateMachineShouldStart,
                DicomStateMachineStartTime = patientUploadRequestParameters.DicomStateMachineStartTime,
                DicomStateMachineEndTime = patientUploadRequestParameters.DicomStateMachineEndTime,
                DicomStateMachineModalities = patientUploadRequestParameters.DicomStateMachineModalities.Split(','),
                DicomStateMachinePacsSource = patientUploadRequestParameters.DicomStateMachinePacsSource,
                DicomStateMachineParametersJson = patientUploadRequestParameters.DicomStateMachineParametersJson,
                LaboratoryStateMachineShouldStart = patientUploadRequestParameters.LaboratoryStateMachineShouldStart,
                LaboratoryStateMachineStartTime = patientUploadRequestParameters.LaboratoryStateMachineStartTime,
                DbUriStateMachineShouldStart = patientUploadRequestParameters.DbUriStateMachineShouldStart,
                DbUriStateMachineStartTime = patientUploadRequestParameters.DbUriStateMachineStartTime,
				PathoxStateMachineShouldStart = patientUploadRequestParameters.PathoxStateMachineShouldStart,
				PathoxStateMachineStartTime = patientUploadRequestParameters.PathoxStateMachineStartTime,
                IntensiveCareStateMachineShouldStart = patientUploadRequestParameters.IntensiveCareStateMachineShouldStart,
                IntensiveCareStateMachineStartTime = patientUploadRequestParameters.IntensiveCareStateMachineStartTime,
                IntensiveCareStateMachineNosologicalCodes = patientUploadRequestParameters.IntensiveCareStateMachineNosologicalCode is null ? null : patientUploadRequestParameters.IntensiveCareStateMachineNosologicalCode.Split(',') ,
                RedcapStateMachineShouldStart = patientUploadRequestParameters.RedcapStateMachineShouldStart,
                RedcapStateMachineStartTime = patientUploadRequestParameters.RedcapStateMachineStartTime,
                RedcapStateMachineRedcapStudyConfigurationId = patientUploadRequestParameters.RedcapStateMachineRedcapStudyConfigurationId,
                RedcapStateMachineRecordId = patientUploadRequestParameters.RedcapStateMachineRecordId
			};

            var stateMachineId = _guidGenerator.Create();

            var stateMachine = new LoadPatientDataStateMachine(stateMachineId, stateMachineParameters);

            await _stateMachineRepository.InsertAsync(stateMachine);

            var uploadRequest = await _patientUploadRequestService.CreateUploadRequestAsync(
                patientUploadRequestParameters.MasterPatientIndex
                , patientUploadRequestParameters.StudyId
                , patientUploadRequestParameters.ClinicalStateMachineShouldStart
                , patientUploadRequestParameters.ClinicalStateMachineStartTime
                , patientUploadRequestParameters.DicomStateMachineShouldStart
                , patientUploadRequestParameters.DicomStateMachineStartTime
                , patientUploadRequestParameters.DicomStateMachineEndTime
                , patientUploadRequestParameters.DicomStateMachineModalities
                , patientUploadRequestParameters.DicomStateMachinePacsSource
                , JsonConvert.SerializeObject(patientUploadRequestParameters.DicomStateMachineParametersJson)
                //, patientUploadRequestParameters.DicomStateMachineParametersJson
                , patientUploadRequestParameters.IntensiveCareStateMachineShouldStart
                , patientUploadRequestParameters.IntensiveCareStateMachineStartTime
                , patientUploadRequestParameters.IntensiveCareStateMachineNosologicalCode
                , patientUploadRequestParameters.RedcapStateMachineShouldStart
                , patientUploadRequestParameters.RedcapStateMachineStartTime
                , patientUploadRequestParameters.RedcapStateMachineRedcapStudyConfigurationId
                , patientUploadRequestParameters.RedcapStateMachineRecordId
                , patientUploadRequestParameters.LaboratoryStateMachineShouldStart
                , patientUploadRequestParameters.LaboratoryStateMachineStartTime
                , userId
                , patientUploadRequestParameters.ClinicalStateMachineDocumentType
                , patientUploadRequestParameters.UseDefaultClinicalStartTime
                , patientUploadRequestParameters.UseDefaultDicomStartTime
                , patientUploadRequestParameters.UseDefaultLaboratoryStartTime
                , patientUploadRequestParameters.UseDefaultIntensiveCareStartTime
                , patientUploadRequestParameters.UseDefaultRedcapStartTime
                , patientUploadRequestParameters.UseDefaultDicomModalities
                , patientUploadRequestParameters.UseDefaultClinicalDocumentType
                , stateMachineId
                , patientUploadRequestParameters.DbUriStateMachineShouldStart
                , patientUploadRequestParameters.DbUriStateMachineStartTime
                , patientUploadRequestParameters.UseDefaultDbUriStartTime
				, patientUploadRequestParameters.PathoxStateMachineShouldStart
				, patientUploadRequestParameters.PathoxStateMachineStartTime
				, patientUploadRequestParameters.UseDefaultPathoxStartTime

				, batchId);

            var uploadRequestParameter = new UploadRequestParameter
            {
                MasterPatientIndex = patientUploadRequestParameters.MasterPatientIndex,
                UploadRequestId = uploadRequest.Id
            };

            _stateMachineQueue.Enqueue((uploadRequestParameter, stateMachine));
        }

        private Task SendNotificationForBatchUploadRequestToUser(Guid? userId)
            => SendNotificationToUserAsync(
                _stringLocalizer[AnonymizationServiceResource.PatientBatchUploadCompletedNotificationMessage]
                , NotificationType.BatchPatientUploadNotification
                , userId
                , MessageType.Success);

        private Task SendNotificationForSuccessfulUploadRequest(Guid? userId)
            => SendNotificationToUserAsync(
                _stringLocalizer[AnonymizationServiceResource.PatientUploadSuccessNotificationMessage]
                , NotificationType.SinglePatientUploadNotification
                , userId
                , MessageType.Success);

        private Task SendNotificationForFailedUploadRequest(Guid? userId)
            => SendNotificationToUserAsync(
                _stringLocalizer[AnonymizationServiceResource.PatientUploadFailedNotificationMessage]
                , NotificationType.SinglePatientUploadNotification
                , userId
                , MessageType.Error);

        private async Task SendNotificationToUserAsync(string notificationMessage, string notificationType, Guid? userId, MessageType messageType)
        {
            if (!userId.HasValue)
            {
                return;
            }

            await _notifyer.SendNotificationToUserAsync(userId.ToString(), new Notification()
            {
                Message = notificationMessage,
                Type = notificationType,
                MessageType = messageType
            });

        }

        private async Task TryRunNextStateMachine()
        {
            if (_semaphore.CurrentCount > 0)
            {
                await _semaphore.WaitAsync();

                var evaluatedStateMachinesQueue = new List<Guid>();

                var dequeueAgain = true;

                do
                {
                    if (!_stateMachineQueue.TryDequeue(out var stateMachineUploadRequestTuple))
                    {
                        _semaphore.Release();
                        return;
                    }

                    if (!evaluatedStateMachinesQueue.Contains(stateMachineUploadRequestTuple.StateMachine.Id))
                    {
                        evaluatedStateMachinesQueue.Add(stateMachineUploadRequestTuple.StateMachine.Id);
                        var masterPatientIndex = stateMachineUploadRequestTuple.uploadRequestParameter.MasterPatientIndex;

                        if (_stateMachinesInProcess.Select(sm => sm.Value.MasterPatientIndex).Contains(masterPatientIndex))
                        {
                            _semaphore.Release();
                            _stateMachineQueue.Enqueue(stateMachineUploadRequestTuple);
                        }
                        else
                        {
                            var loadPatientDataStateMachine = stateMachineUploadRequestTuple.StateMachine;
                            var uploadRequestParameter = stateMachineUploadRequestTuple.uploadRequestParameter;
                            dequeueAgain = false;
                            await StartLoadPatientDataStateMachineAsync(uploadRequestParameter, loadPatientDataStateMachine);
                        }
                    }
                    else
                    {
                        dequeueAgain = false;
                    }
                }
                while (dequeueAgain);
            }
        }

        private async Task StartLoadPatientDataStateMachineAsync(UploadRequestParameter uploadRequestParameter, LoadPatientDataStateMachine loadPatientDataStateMachine)
        {
            if (_stateMachinesInProcess.TryAdd(loadPatientDataStateMachine.Id, uploadRequestParameter))
            {
                var uploadRequest = await _patientUploadRequestService.SetUploadRequestRunningAsync(uploadRequestParameter.UploadRequestId);

                if (uploadRequest.BatchRequestId is not null)
                {
                    await _patientUploadRequestService.TrySetBatchUploadRunningAsync(uploadRequest.BatchRequestId.Value);
                }

                await _stateMachineManager.TryRunNewStateMachineAsync(loadPatientDataStateMachine);
            }
        }

        public async Task RetrieveRunningLoadPatientDataStateMachines()
        {
            var runningPatientUploadRequests = await _patientUploadRequestRepository.GetRunningListAsync();

            foreach (var request in runningPatientUploadRequests)
            {
                var uploadRequestParameter = new UploadRequestParameter { UploadRequestId = request.Id, MasterPatientIndex = request.MasterPatientIndex };
                _stateMachinesInProcess.TryAdd(request.StateMachineId, uploadRequestParameter);
            }
        }

        public async Task RetrievePendingLoadPatientDataStateMachines()
        {
            var pendingPatientUploadRequests = await _patientUploadRequestRepository.GetPendingListAsync();

            foreach (var uploadRequest in pendingPatientUploadRequests)
            {
                var uploadRequestParameter = new UploadRequestParameter { UploadRequestId = uploadRequest.Id, MasterPatientIndex = uploadRequest.MasterPatientIndex };
                var stateMachine = (LoadPatientDataStateMachine) await _stateMachineRepository.GetAsync(s => s.Id == uploadRequest.StateMachineId);
                stateMachine.RebuildCurrentState();

                _stateMachineQueue.Enqueue((uploadRequestParameter, stateMachine));
                await TryRunNextStateMachine();
            }

        }
    }
}

