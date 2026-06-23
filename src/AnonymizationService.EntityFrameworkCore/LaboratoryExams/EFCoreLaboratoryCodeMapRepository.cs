using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.LaboratoryExams
{
    public class EfCoreLaboratoryCodeMapRepository : EfCoreRepository<AnonymizationServiceDbContext, LaboratoryCodeMap>
    {
        public EfCoreLaboratoryCodeMapRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
