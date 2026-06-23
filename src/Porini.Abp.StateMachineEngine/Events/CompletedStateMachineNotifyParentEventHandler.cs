using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class CompletedStateMachineNotifyParentEventHandler
        : ILocalEventHandler<CompletedStateMachineEvent>,
          ITransientDependency
    {
        private readonly StateMachineManager _stateMachineManager;

        public CompletedStateMachineNotifyParentEventHandler(StateMachineManager stateMachineManager)
        {
            _stateMachineManager = stateMachineManager;
        }

        public async Task HandleEventAsync(CompletedStateMachineEvent eventData)
        {
            if (eventData?.ParentStateMachineId is not null)
            {
                await _stateMachineManager.DispatchEventToStateMachineAsync(eventData.ParentStateMachineId.Value, eventData.ExecutedEvent);
            }
        }
    }
}
