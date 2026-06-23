using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public class JobScheduler : IJobScheduler, ITransientDependency
    {
        private readonly ISchedulerFactory schedulerFactory;

        public JobScheduler(ISchedulerFactory schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory;
        }

        public async Task EnqueueJob<T, TArgs, TResult>(TArgs args) where T : Job<TArgs, TResult>
        {
            var scheduler = await schedulerFactory.GetScheduler();

            IDictionary<string, object> dic = new Dictionary<string, object>() { { "TArgs", args } };

            IJobDetail jobDetail = JobBuilder
                .Create<T>()
                .UsingJobData(new JobDataMap(dic))
                .RequestRecovery()
                .Build();

            ITrigger trigger = TriggerBuilder
                .Create()
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }

    public interface IJobScheduler
    {
        Task EnqueueJob<T, TArgs, TResult>(TArgs args) where T : Job<TArgs, TResult>;
    }
}
