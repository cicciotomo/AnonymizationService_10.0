using AnonymizationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.StateMachines
{
    public class EfCoreStateMachineRepository : EfCoreRepository<AnonymizationServiceDbContext, StateMachine, Guid>, IStateMachineRepository
    {
        public EfCoreStateMachineRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<StateMachine>> GetRunningStateMachinesAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(x => !x.Completed && !x.Failed && x.Started)
                .ToListAsync();
        }
    }
}
