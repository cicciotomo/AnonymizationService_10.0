using AnonymizationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.ContainerImages
{
    public class EfCoreContainerImageRepository : EfCoreRepository<AnonymizationServiceDbContext, ContainerImage, Guid>, IContainerImageRepository
    {
        public EfCoreContainerImageRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ContainerImage>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();

            var containerImages = await dbSet
                .Include(x => x.Variables)
                .Include(x => x.Volumes)
                .Include(x => x.InvocationHeaders)
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    containerImage => containerImage.Name.Contains(filter)
                )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();

            return containerImages;
        }
    }
}
