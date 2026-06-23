using AnonymizationService.Localization;
using Volo.Abp.Account;
using Volo.Abp.Account.Localization;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.VirtualFileSystem;

namespace AnonymizationService;

[DependsOn(
    typeof(AnonymizationServiceDomainSharedModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpLocalizationModule)
)]
public class AnonymizationServiceApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AnonymizationServiceDtoExtensions.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AnonymizationServiceApplicationContractsModule>();
        });


        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<AnonymizationServiceApplicationContractsResource>()
                .AddBaseTypes(typeof(AccountResource))
                .AddBaseTypes(typeof(AnonymizationServiceResource))
                .AddVirtualJson("/Localization/AnonymizationServiceApplicationContracts");

            options.DefaultResourceType = typeof(AnonymizationServiceApplicationContractsResource);
        });
    }
}
