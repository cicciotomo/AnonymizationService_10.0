using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Threading.Tasks;

namespace Porini.Abp.StateMachineEngine.States
{
    internal class StateMachineBrokenState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            await LocalEventBus.PublishAsync(
                new FailedStateMachineEvent
                {
                    StateMachineId = stateMachine.Id,
                    ParentStateMachineId = stateMachine.ParentStateMachineId,
                    ErrorMessage = stateMachine.ErrorMessage
                }
            );
        }
    }
}
