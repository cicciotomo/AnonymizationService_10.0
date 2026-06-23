using AnonymizationService.EventTracker;
using AnonymizationService.FileProcessors;
using AnonymizationService.IntensiveCareData;
using AnonymizationService.Localization;
using AnonymizationService.Patients;
using AnonymizationService.PatientUploadRequests;
using AnonymizationService.Permissions;
using AnonymizationService.Services.PatientLoadManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace AnonymizationService.HospitalPatients
{
    [Authorize(AnonymizationServicePermissions.PatientManagementPermission)]
    public class PatientAppService : AnonymizationServiceAppService, IPatientAppService
    {
        private readonly IPatientLoadManager _patientLoadManager;
        private readonly IEventTracker _eventTracker;
        private readonly IExcelProcessor _excelProcessor;
        private readonly IHospitalPatientRepository _patientRepository;
        private readonly IPatientUploadRequestRepository _patientUploadRequestRepository;
        private readonly IPatientBatchUploadRequestRepository _patientBatchUploadRequestRepository;
        private readonly IHospitalPatientService _hospitalPatientService;
        private readonly IServiceProvider _serviceProvider;

        public PatientAppService(IPatientLoadManager patientLoadManager
            , IEventTracker eventTracker
            , IExcelProcessor excelProcessor
            , IHospitalPatientRepository patientRepository
            , IPatientUploadRequestRepository patientUploadRequestRepository
            , IPatientBatchUploadRequestRepository patientBatchUploadRequestRepository
            , IHospitalPatientService hospitalPatientService
            , IServiceProvider serviceProvider)
        {
            _patientLoadManager = patientLoadManager;
            _eventTracker = eventTracker;
            _excelProcessor = excelProcessor;
            _patientRepository = patientRepository;
            _patientUploadRequestRepository = patientUploadRequestRepository;
            _patientBatchUploadRequestRepository = patientBatchUploadRequestRepository;
            _hospitalPatientService = hospitalPatientService;
            _serviceProvider = serviceProvider;
        }

        public async Task CreateAsync(CreatePatientDto createPatientDto)
        {
            _eventTracker.TrackEvent("StartPatientLoading", createPatientDto);
            var patientUploadInput = ObjectMapper.Map<CreatePatientDto, PatientUploadInput>(createPatientDto);

            if (patientUploadInput != null && patientUploadInput.DicomStateMachineEndTime==null) 
            { 
                patientUploadInput.DicomStateMachineEndTime = DateTime.Now;  
            }

            await _patientLoadManager.LoadNewPatientAsync(patientUploadInput, CurrentUser.GetId());
        }

        public async Task CreateManyAsync(IRemoteStreamContent streamContent)
        {
            if (streamContent == null || streamContent.ContentLength == 0)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.EmptyFileExceptionMessage]);
            }

            if (streamContent.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.InvalidFileExceptionMessage]);
            }

            IEnumerable<BatchPatientDto> patientList;

            try
            {
                patientList = (await _excelProcessor.MapFromExcelFileAsync<BatchPatientDto>(streamContent.GetStream())).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

            patientList = patientList.Where(p => p.MasterPatientIndex is not null);

            if (patientList is null || !patientList.Any())
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.UnparsableFileExceptionMessage]);
            }

            await _patientLoadManager.LoadBatchPatientsAsync(ObjectMapper.Map<IEnumerable<BatchPatientDto>, IEnumerable<PatientUploadInput>>(patientList).ToList(), CurrentUser.Id.Value);
        }

        public async Task<PagedResultDto<PatientDto>> GetListAsync(PatientListRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(HospitalPatient.MasterPatientIndex);
            }

            var patients = await _patientRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.Filter
            );

            var totalCount = requestDto.Filter == null
                ? await _patientRepository.CountAsync()
                : await _patientRepository.CountAsync(
                    author => author.MasterPatientIndex.Contains(requestDto.Filter));

            return new PagedResultDto<PatientDto>(
                totalCount,
                ObjectMapper.Map<List<HospitalPatient>, List<PatientDto>>(patients)
            );
        }

        public async Task<PagedResultDto<UploadRequestDto>> GetUploadRequestsForPatientAsync(UploadRequestForPatientRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = $"{nameof(PatientUploadRequest.CreationTime)} desc";
            }

            var uploadRequests = await _patientUploadRequestRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.MasterPatientIndex
            );

            var totalCount = await _patientUploadRequestRepository
                .CountAsync(r => r.MasterPatientIndex == requestDto.MasterPatientIndex);

            return new PagedResultDto<UploadRequestDto>(
                totalCount,
                ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests)
            );
        }


        public async Task<PagedResultDto<UploadRequestDto>> GetUploadRequestsForBatchAsync(UploadRequestForBatchRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(PatientUploadRequest.CreationTime);
            }

            var uploadRequests = await _patientUploadRequestRepository.GetListByBatchIdAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.BatchId
            );

            var totalCount = await _patientUploadRequestRepository
                .CountAsync(r => r.BatchRequestId == requestDto.BatchId);

            return new PagedResultDto<UploadRequestDto>(
                totalCount,
                ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests)
            );
        }

        public async Task<PagedResultDto<UploadRequestDto>> GetUploadRequestListAsync(PagedAndSortedResultRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = $"{nameof(PatientUploadRequest.CreationTime)} desc";
            }

            var uploadRequests = await _patientUploadRequestRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting
            );

            var totalCount = await _patientUploadRequestRepository.CountAsync();

            return new PagedResultDto<UploadRequestDto>(
                totalCount,
                ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests)
            );
        }

        public async Task<PagedResultDto<PatientDto>> GetFilteredtListAsync(PatientFilerListRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(PatientUploadRequest.CreationTime);
            }

            var uploadRequests = await _patientRepository.GetFilteredtListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.CreationTime,
                requestDto.CloudPatientId,
                requestDto.MasterPatientIndex,
                requestDto.FhirPatientId

            );

            var totalCount = requestDto.MaxResultCount == 0 ? await _patientRepository.CountAsync() : uploadRequests.Count();
            uploadRequests = uploadRequests.Skip(requestDto.SkipCount).Take(requestDto.MaxResultCount).ToList();

            return new PagedResultDto<PatientDto>(
                totalCount,
                ObjectMapper.Map<List<HospitalPatient>, List<PatientDto>>(uploadRequests)
            );
        }


        public async Task<PagedResultDto<UploadRequestDto>> GetListByCreationTimeAsync(PatientListFilteredRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(PatientUploadRequest.CreationTime);
            }

            var uploadRequests = await _patientUploadRequestRepository.GetListCreationTimeAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.CreationTime.HasValue ? requestDto.CreationTime.Value : null
                );
            var totalCount = uploadRequests.Count();
            //var totalCount = await _patientUploadRequestRepository
            //    .CountAsync(requestDto.CreationTime.HasValue ? requestDto.CreationTime.Value : true);

            return new PagedResultDto<UploadRequestDto>(
                totalCount,
                ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests)
            );
        }

        public async Task<PagedResultDto<UploadRequestDto>> GetFilteredUploadRequestListAsync(PatientListFilteredRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = $"{nameof(PatientUploadRequest.CreationTime)} desc";
            }

            var uploadRequests = await _patientUploadRequestRepository.GetFilteredListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.MasterPatientIndex, requestDto.StartCreationTime, requestDto.EndCreationTime, requestDto.ClinicalStateMachineShouldStart,
                requestDto.DicomStateMachineShouldStart, requestDto.LaboratoryStateMachineShouldStart,
                requestDto.DbUriStateMachineShouldStart, requestDto.PathoxStateMachineShouldStart,
                requestDto.RedcapStateMachineShouldStart, requestDto.IntensiveCareStateMachineShouldStart
            );

            var totalCount = uploadRequests != null ? uploadRequests.Count : await _patientUploadRequestRepository.CountAsync();
            uploadRequests = uploadRequests.Skip(requestDto.SkipCount).Take(requestDto.MaxResultCount).ToList();

            List<UploadRequestDto> result = ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests);
            var stateMachineRepository = _serviceProvider.GetService<IStateMachineRepository>();
            var intensiveCareRepository = _serviceProvider.GetService<IRepository<IntensiveCarePatientData>>();
            foreach (var req in result)
            { 
                var smId = req.StateMachineId;
                IQueryable<StateMachine> stateMachineChildrenList = stateMachineRepository.GetQueryableAsync().Result;
                List<Guid> smIDs = stateMachineChildrenList.Where(x => x.ParentStateMachineId == smId).Select(x => x.Id).ToList();

                IQueryable<IntensiveCarePatientData> items = intensiveCareRepository.GetQueryableAsync().Result;
                var pipelineRunIds = items.Where(x => smIDs.Contains(x.StateMachineId)).Where(x => !(bool)x.PipelineRunResult).Select(x => x.PipelineId ).ToList();

                req.IntensiveCareFailedPipelineRunID = string.Join(" - ", pipelineRunIds);
                if (req.IntensiveCareFailedPipelineRunID.Any())
                {
                    req.FlagIntensiveCareFailedPipelineRun = true;
                }
                else
                {
                    req.FlagIntensiveCareFailedPipelineRun = false;
                }
            }

            //return new PagedResultDto<UploadRequestDto>(
            //    totalCount,
            //    ObjectMapper.Map<List<PatientUploadRequest>, List<UploadRequestDto>>(uploadRequests)
            //);
            return new PagedResultDto<UploadRequestDto>(
                totalCount,
                result
            );
        }


        public async Task ExecuteRequestWithSameParametersAsync(Guid uploadRequestId)
        {
            var patientUploadRequest = await _patientUploadRequestRepository.GetAsync(uploadRequestId);
            await _patientLoadManager.ReloadPatientUploadRequestAsync(patientUploadRequest);
        }

        public async Task<PagedResultDto<BatchUploadRequestDto>> GetBatchUploadRequestListAsync(PagedAndSortedResultRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = $"{nameof(BatchUploadRequestDto.CreationTime)} desc";
            }

            var batchUploadRequests = await _patientBatchUploadRequestRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting
            );

            var totalCount = await _patientBatchUploadRequestRepository.CountAsync();

            return new PagedResultDto<BatchUploadRequestDto>(
                totalCount,
                ObjectMapper.Map<List<PatientBatchUploadRequest>, List<BatchUploadRequestDto>>(batchUploadRequests)
            );
        }

        public async Task IssuePatientDeletion(Guid cloudPatientId)
        {
            await _hospitalPatientService.IssuePatientDeletionRequest(cloudPatientId);
        }

        [AllowAnonymous]
        public async Task Delete(Guid id)
        {
            await _hospitalPatientService.DeletePatientAsync(id);
        }

        [AllowAnonymous]
        public async Task<string> GetMPI(Guid cloudPatientId)
        {
            var patient = await _patientRepository.FindAsync(x => x.CloudPatientId == cloudPatientId);
            return patient != null ? patient.MasterPatientIndex: "NO MPI";
        }
        [AllowAnonymous]
        public async Task<Guid?> GetCPI(string masterPatientId)
        {
            var patient = await _patientRepository.FindAsync(x => x.MasterPatientIndex == masterPatientId);
            if (patient != null)
            {
                return patient.CloudPatientId;
            }
            else {
                return null;
            }
            
        }


        public async Task<PagedResultDto<BatchUploadRequestDto>> GetBatchUploadRequestByCreationTimeListAsync(PagedAndSortedResultRequestFilterDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = $"{nameof(BatchUploadRequestDto.CreationTime)} desc";
            }
            var batchUploadRequests = await _patientBatchUploadRequestRepository.GetFilteredListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.CreationTime
            );

            var totalCount = await _patientBatchUploadRequestRepository
                .CountAsync(r => r.CreationTime.Date == requestDto.CreationTime.Date);

            return new PagedResultDto<BatchUploadRequestDto>(
               totalCount,
               ObjectMapper.Map<List<PatientBatchUploadRequest>, List<BatchUploadRequestDto>>(batchUploadRequests)
           );
        }
    }
}
