using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.PatientUploadRequests
{
    public interface IPatientBatchUploadRequestRepository : IRepository<PatientBatchUploadRequest, Guid>
    {
        Task<List<PatientBatchUploadRequest>> GetListAsync(int skipCount, int maxResultCount, string sorting);
        Task<List<PatientBatchUploadRequest>> GetFilteredListAsync(int skipCount, int maxResultCount, string sorting, DateTime? creationTime);
    }
}
