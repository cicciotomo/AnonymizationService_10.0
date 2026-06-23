using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Threading.Tasks;

namespace Porini.Abp.StateMachineEngine.States
{
    internal class StateMachineCompletedState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            await LocalEventBus.PublishAsync(
                new CompletedStateMachineEvent
                {
                    StateMachineId = stateMachine.Id,
                    ExecutedEvent = stateMachine.StateMachineCompletedEventType,
                    ParentStateMachineId = stateMachine.ParentStateMachineId
                }
            );
        }
    }
}
