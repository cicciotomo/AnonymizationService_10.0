using AnonymizationService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.Redcap
{
    public class EfCoreTransmissionToUseCaseRepository : EfCoreRepository<AnonymizationServiceDbContext, TransmissionToUseCase, Guid>
    {
        public EfCoreTransmissionToUseCaseRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
