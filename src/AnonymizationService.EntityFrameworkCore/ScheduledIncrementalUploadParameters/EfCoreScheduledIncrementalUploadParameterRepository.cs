using AnonymizationService.EntityFrameworkCore;
using AnonymizationService.ScheduledUploadParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;


namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    public class EfCoreScheduledIncrementalUploadParameterRepository : EfCoreRepository<AnonymizationServiceDbContext, ScheduledIncrementalUploadParameter, Guid>,
        IScheduledIncrementalUploadParameterRepository
    {
        public EfCoreScheduledIncrementalUploadParameterRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ScheduledIncrementalUploadParameter>> GetListAsync(
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
            return dbResults.WhereIf(!filter.IsNullOrWhiteSpace(), s => Enum.GetName(s.ParameterKey).Contains(filter)).ToList();

        }
    }
}
