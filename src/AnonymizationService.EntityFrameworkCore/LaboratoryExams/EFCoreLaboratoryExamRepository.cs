using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.LaboratoryExams
{
    public class EfCoreLaboratoryExamRepository : EfCoreRepository<AnonymizationServiceDbContext, LaboratoryExam>
    {
        public EfCoreLaboratoryExamRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
