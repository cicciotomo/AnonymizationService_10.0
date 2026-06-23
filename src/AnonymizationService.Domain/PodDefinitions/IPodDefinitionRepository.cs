using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.PodDefinitions
{
    public interface IPodDefinitionRepository : IRepository<PodDefinition, Guid>
    {
        Task<List<PodDefinition>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
