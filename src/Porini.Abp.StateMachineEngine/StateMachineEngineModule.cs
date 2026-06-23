using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Porini.Abp.StateMachineEngine
{
    public class StateMachineEngineModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var stateMachineManager = context.ServiceProvider.GetRequiredService<StateMachineManager>();
            var logger = context.ServiceProvider.GetRequiredService<ILogger<StateMachineEngineModule>>();

            try
            {
                logger.LogInformation("Initializing State Machine Manager");
                stateMachineManager.InitializeStateMachineManagerAsync().Wait();
                logger.LogInformation("State Machine Manager initialization completed");
            }
            catch
            {
                logger.LogError("State Machine Manager initialization failed");
            }

            var scheduler = context.ServiceProvider.GetService<IScheduler>();
            scheduler.Start();
        }
    }
}
