using AnonymizationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public class EfCoreExternalServiceRequestLimitRepository : EfCoreRepository<AnonymizationServiceDbContext, ExternalServiceRequestLimit, Guid>,
        IExternalServiceRequestLimitRepository
    {
        public EfCoreExternalServiceRequestLimitRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ExternalServiceRequestLimit>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            var dbResults = await dbSet
                    .OrderBy(sorting)
                    .Skip(skipCount)
                    .Take(maxResultCount)
                    .ToListAsync();

            return dbResults.WhereIf(!filter.IsNullOrWhiteSpace(), e => Enum.GetName(e.ExternalServiceRequestsType).Contains(filter)).ToList();
        }
    }
}
