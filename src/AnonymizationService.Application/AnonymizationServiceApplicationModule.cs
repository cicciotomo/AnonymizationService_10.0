using AnonymizationService.EventTracker;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace AnonymizationService;

[DependsOn(
    typeof(AnonymizationServiceDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(AnonymizationServiceApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AnonymizationServiceEventTrackerModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpAutoMapperModule)
    )]
public class AnonymizationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<AnonymizationServiceApplicationModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AnonymizationServiceApplicationModule>();
        });

    }
}