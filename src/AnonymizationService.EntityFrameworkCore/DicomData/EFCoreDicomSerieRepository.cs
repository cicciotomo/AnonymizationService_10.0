using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.DicomData
{
    public class EfCoreDicomSerieRepository : EfCoreRepository<AnonymizationServiceDbContext, DicomSerie, string>
    {
        public EfCoreDicomSerieRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
