using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace AnonymizationService.PatientUploadRequests
{
    public class PatientUploadRequestService : DomainService
    {
        private readonly IRepository<PatientUploadRequest> _patientUploadRequestRepository;
        private readonly IRepository<PatientBatchUploadRequest> _patientUploadBatchRequestRepository;

        public PatientUploadRequestService(IRepository<PatientBatchUploadRequest> patientUploadBatchRequestRepository, IRepository<PatientUploadRequest> patientUploadRequestRepository)
        {
            _patientUploadBatchRequestRepository = patientUploadBatchRequestRepository;
            _patientUploadRequestRepository = patientUploadRequestRepository;
        }

        public async Task<PatientUploadRequest> CreateUploadRequestAsync(string masterPatientIndex
            , string studyId
            , bool clinicalStateMachineShouldStart
            , DateTime? clinicalStateMachineStartTime
            , bool dicomStateMachineShouldStart
            , DateTime? dicomStateMachineStartTime
            , DateTime? dicomStateMachineEndTime
            , string dicomStateMachineModalities
            , string dicomStateMachinePacsSource
            , string dicomStateMachineParametersJson
            , bool intensiveCareStateMachineShouldStart
            , DateTime? intensiveCareStateMachineStartTime
            , string intensiveCareStateMachineNosologicalCode
            , bool redcapStateMachineShouldStart
            , DateTime? redcapStateMachineStartTime
            , string redcapStateMachineRedcapStudyConfigurationId
            , string redcapStateMachineRecordId
            , bool laboratoryStateMachineShouldStart
            , DateTime? laboratoryStateMachineStartTime
            , Guid creatorId
            , ClinicalDocumentTypeFilter[] clinicalStateMachineDocumentType
            , bool clinicalStartTimeIsDefault
            , bool dicomStartTimeIsDefault
            , bool laboratoryStartTimeIsDefault
            , bool intensiveCareStartTimeIsDefault
            , bool redcapStartTimeIsDefault
            , bool dicomModalitiesAreDefault
            , bool clinicalDocumentTypeAreDefault
            , Guid statemachineId
			, bool dbUriStateMachineShouldStart
			, DateTime? dbUriStateMachineStartTime
			, bool dbUriStartTimeIsDefault
			, bool pathoxStateMachineShouldStart
			, DateTime? pathoxStateMachineStartTime
			, bool pathoxStartTimeIsDefault

			, Guid? batchId = null)
        {
            var uploadRequest = new PatientUploadRequest(
                GuidGenerator.Create()
                , masterPatientIndex
                , studyId
                , clinicalStateMachineShouldStart
                , clinicalStateMachineStartTime
                , dicomStateMachineShouldStart
                , dicomStateMachineStartTime
                , dicomStateMachineEndTime
                , dicomStateMachineModalities
                , dicomStateMachinePacsSource
                , dicomStateMachineParametersJson
                , intensiveCareStateMachineShouldStart
                , intensiveCareStateMachineStartTime
                , intensiveCareStateMachineNosologicalCode
                , redcapStateMachineShouldStart
                , redcapStateMachineStartTime
                , redcapStateMachineRedcapStudyConfigurationId
                , redcapStateMachineRecordId
                , laboratoryStateMachineShouldStart
                , laboratoryStateMachineStartTime
                , creatorId
                , clinicalStateMachineDocumentType
                , clinicalStartTimeIsDefault
                , dicomStartTimeIsDefault
                , laboratoryStartTimeIsDefault
                , intensiveCareStartTimeIsDefault
                , redcapStartTimeIsDefault
                , dicomModalitiesAreDefault
                , clinicalDocumentTypeAreDefault
                , statemachineId
                , dbUriStateMachineShouldStart
                , dbUriStateMachineStartTime
                , dbUriStartTimeIsDefault
				, pathoxStateMachineShouldStart
				, pathoxStateMachineStartTime
				, pathoxStartTimeIsDefault
                
				, batchId);

            return await _patientUploadRequestRepository.InsertAsync(uploadRequest, autoSave: true);
        }

        public async Task<PatientUploadRequest> SetResultForUploadRequestAsync(Guid id, string errorMessage)
        {
            var uploadRequest = await _patientUploadRequestRepository.GetAsync(r => r.Id == id);
            if (string.IsNullOrEmpty(errorMessage))
            {
                uploadRequest.SetSuccess();
            }
            else
            {
                uploadRequest.SetFailure(errorMessage);
            }

            return await _patientUploadRequestRepository.UpdateAsync(uploadRequest);
        }

        public async Task<PatientUploadRequest> SetUploadRequestRunningAsync(Guid id)
        {
            var uploadRequest = await _patientUploadRequestRepository.GetAsync(r => r.Id == id);
            uploadRequest.SetRunning();
            return await _patientUploadRequestRepository.UpdateAsync(uploadRequest);
        }

        public Task<List<PatientUploadRequest>> GetRunningPatientUploadRequestsForBatchId(Guid batchId) =>
            _patientUploadRequestRepository.GetListAsync(
                r => r.BatchRequestId == batchId
                && (r.Status == PatientUploadStatus.Running || r.Status == PatientUploadStatus.Pending));

        public async Task<PatientBatchUploadRequest> CreateBatchUploadRequestAsync(Guid creatorId)
        {
            var batchUploadRequest = new PatientBatchUploadRequest(GuidGenerator.Create(), creatorId);
            return await _patientUploadBatchRequestRepository.InsertAsync(batchUploadRequest, autoSave: true);
        }

        public async Task<PatientBatchUploadRequest> TrySetBatchUploadCompletedAsync(Guid batchId)
        {
            var batchRequest = await _patientUploadBatchRequestRepository.GetAsync(b => b.Id == batchId);
            if (batchRequest.TrySetCompleted())
            {
                await _patientUploadBatchRequestRepository.UpdateAsync(batchRequest);
            }

            return batchRequest;
        }

        public async Task<PatientBatchUploadRequest> TrySetBatchUploadRunningAsync(Guid batchId)
        {
            var batchRequest = await _patientUploadBatchRequestRepository.GetAsync(b => b.Id == batchId);
            if (batchRequest.TrySetRunning())
            {
                await _patientUploadBatchRequestRepository.UpdateAsync(batchRequest);
            }
            return batchRequest;
        }
    }
}
