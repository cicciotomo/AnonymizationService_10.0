using AnonymizationService.EntityFrameworkCore;
using AnonymizationService.HospitalPatients;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.ClinicalDocuments

{
    public class EfCoreClinicalDocumentTypeRepository : EfCoreRepository<AnonymizationServiceDbContext, ClinicalDocumentType>, IClinicalDocumentTypeRepository
    {
        public EfCoreClinicalDocumentTypeRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ClinicalDocumentType>> GetListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.ToListAsync();
        }
    }
}
