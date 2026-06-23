using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Events;
using Quartz;
using System;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Local;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public class StateMachineJobListener : JobListener<JobArgs, JobResult>
    {
        private readonly ILogger<StateMachineJobListener> _logger;
        private readonly ILocalEventBus _localEventBus;

        public StateMachineJobListener(ILocalEventBus localEventBus, ILogger<StateMachineJobListener> logger)
        {
            _localEventBus = localEventBus;
            _logger = logger;
        }

        public override string Name => "JobListener";

        protected override async Task JobCompletedSuccessfully(JobResult jobResult)
        {
            await _localEventBus.PublishAsync(
                new ExecutedTaskEvent(jobResult.StateMachineId, jobResult.JobResultEvent)
            );
        }

        protected override async Task JobCompletedUnsuccessfully(JobArgs jobArgs, JobExecutionException jobExecutionException)
        {
            var exception = GetRealException(jobExecutionException);
            _logger.LogError(jobExecutionException, "Job failed with error {error}", exception.Message);

            await _localEventBus.PublishAsync(
                new FailedJobEvent
                {
                    StateMachineId = jobArgs.StateMachineId,
                    Exception = exception
                }
            );
        }

        private Exception GetRealException(JobExecutionException jobExecutionException)
        {
            Exception resultException = jobExecutionException;
            while (resultException is JobExecutionException
                || resultException is SchedulerException)
            {
                if (resultException.InnerException is null)
                {
                    return resultException;
                }
                else
                {
                    resultException = resultException.InnerException;
                }
            }

            return resultException;
        }
    }
}
