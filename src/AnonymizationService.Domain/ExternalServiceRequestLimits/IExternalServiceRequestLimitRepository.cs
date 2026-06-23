using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public interface IExternalServiceRequestLimitRepository : IRepository<ExternalServiceRequestLimit, Guid>
    {
        Task<List<ExternalServiceRequestLimit>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
