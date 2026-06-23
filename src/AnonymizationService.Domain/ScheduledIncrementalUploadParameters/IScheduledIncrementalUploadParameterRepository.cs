using AnonymizationService.ScheduledUploadParameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    public interface IScheduledIncrementalUploadParameterRepository : IRepository<ScheduledIncrementalUploadParameter, Guid>
    {
        Task<List<ScheduledIncrementalUploadParameter>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
