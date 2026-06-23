using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.ClinicalDocuments
{
    public class EfCoreClinicalDocumentRepository : EfCoreRepository<AnonymizationServiceDbContext, ClinicalDocument>
    {
        public EfCoreClinicalDocumentRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
