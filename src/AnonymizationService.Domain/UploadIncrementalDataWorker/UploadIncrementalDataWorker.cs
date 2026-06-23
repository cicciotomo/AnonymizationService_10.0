using AnonymizationService.Services.UploadIncrementalDataManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace AnonymizationService.UploadIncrementalDataWorker
{
    public class UploadIncrementalDataWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public UploadIncrementalDataWorker(
                AbpAsyncTimer timer,
                IServiceScopeFactory serviceScopeFactory
            ) : base(
                timer,
                serviceScopeFactory)
        {
            Timer.Period = 900000; //15 minutes
        }

        protected async override Task DoWorkAsync(
            PeriodicBackgroundWorkerContext workerContext)
        {

            Logger.LogInformation("Starting: UploadIncrementalDataWorker");

            var uploadIncrementalDataManager = workerContext.ServiceProvider.GetRequiredService<IUploadIncrementalDataManager>();

            await uploadIncrementalDataManager.UploadIncrementalDataAsync();

            Logger.LogInformation("Completed: UploadIncrementalDataWorker");
        }
    }
}
