using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class FailedJobEventHandler
        : ILocalEventHandler<FailedJobEvent>,
          ITransientDependency
    {
        private readonly StateMachineManager _stateMachineManager;

        public FailedJobEventHandler(StateMachineManager stateMachineManager)
        {
            _stateMachineManager = stateMachineManager;
        }

        public async Task HandleEventAsync(FailedJobEvent eventData)
        {
            await _stateMachineManager.DispatchExceptionToStateMachineAsync(eventData.StateMachineId, eventData.Exception);
        }
    }
}
