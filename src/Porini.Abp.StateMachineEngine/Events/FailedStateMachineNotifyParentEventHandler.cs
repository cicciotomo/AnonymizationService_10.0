using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class FailedStateMachineNotifyParentEventHandler
        : ILocalEventHandler<FailedStateMachineEvent>,
          ITransientDependency
    {
        private readonly StateMachineManager _stateMachineManager;

        public FailedStateMachineNotifyParentEventHandler(StateMachineManager stateMachineManager)
        {
            _stateMachineManager = stateMachineManager;
        }

        public async Task HandleEventAsync(FailedStateMachineEvent eventData)
        {
            if (eventData?.ParentStateMachineId is not null)
            {
                var exc = new FailedChildrenStateMachineException($"Children state machine with ID {eventData.StateMachineId} has failed with error {eventData.ErrorMessage}");
                await _stateMachineManager.DispatchExceptionToStateMachineAsync(eventData.ParentStateMachineId.Value, exc);
            }
        }
    }

    internal class FailedChildrenStateMachineException : Exception
    {
        public FailedChildrenStateMachineException(string message) : base(message)
        {
        }
    }
}
