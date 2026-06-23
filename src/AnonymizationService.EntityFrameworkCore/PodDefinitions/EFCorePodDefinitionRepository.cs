using AnonymizationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.PodDefinitions
{
    public class EfCorePodDefinitionRepository : EfCoreRepository<AnonymizationServiceDbContext, PodDefinition, Guid>, IPodDefinitionRepository
    {
        public EfCorePodDefinitionRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<PodDefinition>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            var podDefinitions = await dbSet
                .Include(x => x.ContainerList)
                .Include(x => x.EnvironmentVariables)
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    podDefinition => podDefinition.Name.Contains(filter)
                )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();

            return podDefinitions;
        }
    }
}
