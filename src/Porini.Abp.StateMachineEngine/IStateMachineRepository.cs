using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Porini.Abp.StateMachineEngine
{
    public interface IStateMachineRepository : IRepository<StateMachine, Guid>
    {
        Task<List<StateMachine>> GetRunningStateMachinesAsync();
    }
}
