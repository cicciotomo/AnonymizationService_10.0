using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.DefaultImportParameters
{
    public interface IDefaultImportParameterRepository : IRepository<DefaultImportParameter, Guid>
    {
        Task<List<DefaultImportParameter>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
    }
}
