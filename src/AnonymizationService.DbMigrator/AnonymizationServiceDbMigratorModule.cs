using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace AnonymizationService.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AnonymizationServiceEntityFrameworkCoreModule),
    typeof(AnonymizationServiceApplicationContractsModule)
    )]
public class AnonymizationServiceDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
