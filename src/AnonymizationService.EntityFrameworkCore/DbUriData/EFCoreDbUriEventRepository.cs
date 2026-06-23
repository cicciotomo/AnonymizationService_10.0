using AnonymizationService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.DbUriData
{
	public class EFCoreDbUriEventRepository : EfCoreRepository<AnonymizationServiceDbContext, DbUriEvent, Int64>
	{
		public EFCoreDbUriEventRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}
	}

}
