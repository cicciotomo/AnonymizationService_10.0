using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Local;

namespace Porini.Abp.StateMachineEngine.States
{
    public abstract class State
    {
        protected ILocalEventBus LocalEventBus { get; private set; }
        protected IJobScheduler JobScheduler { get; private set; }

        protected abstract Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine);

        internal async Task TryRunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (!AlreadyRun)
            {
                LocalEventBus = serviceProvider.GetService<ILocalEventBus>();
                JobScheduler = serviceProvider.GetService<IJobScheduler>();
                AlreadyRun = true;
                try
                {
                    await RunAsync(serviceProvider, stateMachine);
                }
                catch (Exception exc)
                {
                    await LocalEventBus.PublishAsync(new FailedJobEvent
                    {
                        StateMachineId = stateMachine.Id,
                        Exception = exc
                    });
                }
            }
        }

        protected async Task PublishEventOnBusAsync(ExecutedTaskEvent @event)
        {
            await LocalEventBus?.PublishAsync(@event);
        }

        internal bool AlreadyRun;

        protected State()
        {
            AlreadyRun = false;
        }

        public virtual void HandleEvent(Event @event) { }
        public virtual void HandleException(Exception exception) { }

        public virtual bool ShouldReplay() { return !AlreadyRun; }
        public virtual bool CanGoNext() { return true; }
    }
}
