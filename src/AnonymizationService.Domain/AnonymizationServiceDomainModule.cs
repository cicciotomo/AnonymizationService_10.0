using AnonymizationService.ContainerImages;
using AnonymizationService.Containers;
using AnonymizationService.Containers.ContainerHosts;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.MultiTenancy;
using AnonymizationService.Pods;
using AnonymizationService.Services.DataPlatform;
using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.Dicom;
using AnonymizationService.Services.DicomWeb;
using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.Galileo;
using AnonymizationService.Services.Pathox;
using AnonymizationService.Services.PatientLoadManager;
using AnonymizationService.Services.PlatformManagementConsole;
using AnonymizationService.Services.SynapsePipeline;
using AnonymizationService.Services.ThrottlingManager;
using AnonymizationService.Services.UploadIncrementalDataManager;
using AnonymizationService.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.Quartz;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;

namespace AnonymizationService;

[DependsOn(
    typeof(AnonymizationServiceDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpBackgroundJobsQuartzModule),
    typeof(StateMachineEngineModule)
)]

public class AnonymizationServiceDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpBackgroundJobQuartzOptions>(options =>
        {
            options.RetryCount = 100;
            options.RetryIntervalMillisecond = 10000;
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });

        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        Configure<GalileoSettings>(configuration.GetSection(GalileoSettings.SettingsName));
        Configure<DefaultImportParameterKeys>(configuration.GetSection(DefaultImportParameterKeys.KeyName));
        Configure<DicomStoreSettings>(configuration.GetSection(DicomStoreSettings.SettingsName));
        Configure<DicomSettings>(configuration.GetSection(DicomSettings.SettingsName));
        Configure<ContainerHostSettings>(configuration.GetSection(ContainerHostSettings.SettingsName));
        Configure<FhirServiceSettings>(configuration.GetSection(FhirServiceSettings.SettingsName));
        Configure<DicomWebServiceSettings>(configuration.GetSection(DicomWebServiceSettings.SettingsName));
        Configure<PlatformManagementConsoleClientSettings>(configuration.GetSection(PlatformManagementConsoleClientSettings.SettingsName));
        Configure<DataPlatformSettings>(configuration.GetSection(DataPlatformSettings.SettingsName));
        Configure<PathoxSettings>(configuration.GetSection(PathoxSettings.SettingsName));
        Configure<DbUriSettings>(configuration.GetSection(DbUriSettings.SettingsName));
        Configure<SynapseSettings>(configuration.GetSection(SynapseSettings.SettingsName));

        context.Services.AddHttpClient();
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(services =>
        {
            context.Services.GetConfiguration();
            var loggerContainerHost = services.GetRequiredService<ILogger<ContainerHost>>();
            var containerHostOptionsSnapshot = services.GetRequiredService<IOptionsSnapshot<ContainerHostSettings>>();
            var containerHostOptions = containerHostOptionsSnapshot.Value;

            var containerHost = ContainerHostFactory.Generate(
                containerHostSettings: new ContainerHostSettings()
                {
                    BaseTcpPort = containerHostOptions.BaseTcpPort,
                    CpuAvailable = containerHostOptions.CpuAvailable,
                    RamAvailable = containerHostOptions.RamAvailable,
                    IsLocalHost = containerHostOptions.IsLocalHost,
                    RemoteUri = containerHostOptions.RemoteUri,
                    RemoteUriScheme = containerHostOptions.RemoteUriScheme,
                    DockerEnginePort = containerHostOptions.DockerEnginePort,
                    RemoteEndpoint = containerHostOptions.RemoteEndpoint
                },
                logger: loggerContainerHost
            );

            return containerHost;
        });
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        PreConfigure<AbpQuartzOptions>(options =>
        {
            options.Configurator = configure =>
            {
                configure.AddJobListener<StateMachineJobListener>(GroupMatcher<JobKey>.AnyGroup());
                configure.UsePersistentStore(storeOptions =>
                {
                    storeOptions.UseJsonSerializer();
                    storeOptions.UseSqlServer(configuration.GetConnectionString("AnonymizationServiceDbConnectionString"));
                    storeOptions.UseClustering(c =>
                    {
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                        c.CheckinInterval = TimeSpan.FromSeconds(10);
                    });
                });
            };
        });
    }

    public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var schedulerFactory = context.ServiceProvider.GetService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();
        var triggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
        await scheduler.UnscheduleJobs(triggerKeys);

        var throttlingManager = context.ServiceProvider.GetRequiredService<IThrottlingManager>();
        await throttlingManager.RetrieveExternalServiceRequestLimits();
                
        var containerStore = context.ServiceProvider.GetRequiredService<IContainerStore>();
        var containerImageRepository = context.ServiceProvider.GetRequiredService<IRepository<ContainerImage, Guid>>();
        var containerHost = context.ServiceProvider.GetService<ContainerHost>();
        await containerHost.SyncWithStoreAsync(containerStore, containerImageRepository);

        var podOrchestrator = context.ServiceProvider.GetService<IPodOrchestrator>();
        await podOrchestrator.InitializeAsync();

        var uploadIncrementalDataManager = context.ServiceProvider.GetRequiredService<IUploadIncrementalDataManager>();
        await uploadIncrementalDataManager.Initialize();

        var patientLoadManager = context.ServiceProvider.GetRequiredService<IPatientLoadManager>();
        await patientLoadManager.RetrieveRunningLoadPatientDataStateMachines();
        await patientLoadManager.RetrievePendingLoadPatientDataStateMachines();
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<UploadIncrementalDataWorker.UploadIncrementalDataWorker>();
    }
}
