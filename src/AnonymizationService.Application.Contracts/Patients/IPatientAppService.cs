using AnonymizationService.Patients;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace AnonymizationService.HospitalPatients
{
    public interface IPatientAppService
    {
        Task CreateAsync(CreatePatientDto createPatientDto);
        Task CreateManyAsync(IRemoteStreamContent streamContent);
        Task<PagedResultDto<BatchUploadRequestDto>> GetBatchUploadRequestListAsync(PagedAndSortedResultRequestDto requestDto);
        Task<PagedResultDto<BatchUploadRequestDto>> GetBatchUploadRequestByCreationTimeListAsync(PagedAndSortedResultRequestFilterDto requestDto);
        Task<PagedResultDto<PatientDto>> GetListAsync(PatientListRequestDto requestDto);
        Task<PagedResultDto<UploadRequestDto>> GetUploadRequestListAsync(PagedAndSortedResultRequestDto requestDto);
        Task<PagedResultDto<UploadRequestDto>> GetUploadRequestsForBatchAsync(UploadRequestForBatchRequestDto requestDto);
        Task<PagedResultDto<UploadRequestDto>> GetUploadRequestsForPatientAsync(UploadRequestForPatientRequestDto requestDto);
        Task Delete(Guid id);
        Task IssuePatientDeletion(Guid cloudPatientId);
        Task<string> GetMPI(Guid cloudPatientId);
        Task<Guid?> GetCPI(string masterPatientId);
    }
}
