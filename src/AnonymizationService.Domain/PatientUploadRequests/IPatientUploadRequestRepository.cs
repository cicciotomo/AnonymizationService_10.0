using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.PatientUploadRequests
{
    public interface IPatientUploadRequestRepository : IRepository<PatientUploadRequest, Guid>
    {
        Task<List<PatientUploadRequest>> GetListAsync(int skipCount, int maxResultCount, string sorting, string masterPatientIndex = null);
        Task<List<PatientUploadRequest>> GetListByBatchIdAsync(int skipCount, int maxResultCount, string sorting, Guid batchId);
        Task<List<PatientUploadRequest>> GetRunningListAsync();
        Task<List<PatientUploadRequest>> GetPendingListAsync();
        Task<List<PatientUploadRequest>> GetLastRequestForEachPatientListAsync();
        Task<List<PatientUploadRequest>> GetListCreationTimeAsync(int skipCount, int maxResultCount, string sorting, DateTime? creationTime);
        Task<List<PatientUploadRequest>> GetFilteredListAsync(int skipCount, int maxResultCount, string sorting, string? masterPatientIndex, DateTime? startCreationTimeFilter,
                                                                        DateTime? endCreationTimeFilter, bool? clinicalStateMachineShouldStart
                                                                        , bool? dicomStateMachineShouldStart, bool? laboratoryStateMachineShouldStart
                                                                        , bool? dbUriStateMachineShouldStart, bool? pathoxStateMachineShouldStart
                                                                        , bool? redcapStateMachineShouldStart, bool? IntensiveCareStateMachineShouldStart);
    }
}
