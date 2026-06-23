using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.ContainerImages
{
    public interface IContainerImageRepository : IRepository<ContainerImage, Guid>
    {
        Task<List<ContainerImage>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}