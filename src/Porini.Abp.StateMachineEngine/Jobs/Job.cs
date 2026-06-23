using Newtonsoft.Json;
using Quartz;
using System.Threading.Tasks;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public abstract class Job<TArgs, TResult> : IJob
    {
        protected Job()
        {
        }

        public abstract Task<TResult> ExecuteJobAsync(TArgs args);

        public async Task Execute(IJobExecutionContext context)
        {
            if (context.MergedJobDataMap.TryGetValue("TArgs", out object value))
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.Objects,
                };

                var valueArgs = (TArgs)value;
                var result = await ExecuteJobAsync(valueArgs);
                context.Result = JsonConvert.SerializeObject(result, settings);
            }
        }
    }
}
