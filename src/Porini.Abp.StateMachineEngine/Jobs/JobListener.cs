using Newtonsoft.Json;
using Quartz;
using Quartz.Listener;
using System.Threading;
using System.Threading.Tasks;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public abstract class JobListener<TArgs, TJobResult> : JobListenerSupport
    {
        public JobListener() { }

        protected abstract Task JobCompletedSuccessfully(TJobResult jobResult);
        protected abstract Task JobCompletedUnsuccessfully(TArgs jobResult, JobExecutionException jobExecutionException);

        public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            if (jobException is not null)
            {
                if (context.MergedJobDataMap.TryGetValue("TArgs", out object value))
                {
                    var valueArgs = (TArgs)value;
                    await JobCompletedUnsuccessfully(valueArgs, jobException);
                }
            }
            else
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.Objects,
                };

                var jobResult = JsonConvert.DeserializeObject<TJobResult>(context.Result.ToString(), settings);
                await JobCompletedSuccessfully(jobResult);
            }
        }
    }
}
