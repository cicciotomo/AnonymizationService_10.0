using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class ExecutedTaskEventHandler
        : ILocalEventHandler<ExecutedTaskEvent>,
          ITransientDependency
    {
        private readonly StateMachineManager _stateMachineManager;

        public ExecutedTaskEventHandler(StateMachineManager stateMachineManager)
        {
            _stateMachineManager = stateMachineManager;
        }

        public async Task HandleEventAsync(ExecutedTaskEvent eventData)
        {
            await _stateMachineManager.DispatchEventToStateMachineAsync(eventData.StateMachineId, eventData.ExecutedEvent);
        }
    }
}
