using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.PathoxData
{
	public class EFCorePathoxExamRepository : EfCoreRepository<AnonymizationServiceDbContext, PathoxExam, string>
	{
		public EFCorePathoxExamRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}
	}
}
