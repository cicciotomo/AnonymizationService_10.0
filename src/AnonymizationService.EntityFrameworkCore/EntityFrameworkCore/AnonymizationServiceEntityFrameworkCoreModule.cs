
using AnonymizationService.ContainerImages;
using AnonymizationService.EntityFrameworkCore.IntensiveCare;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Redcap;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace AnonymizationService.EntityFrameworkCore;

[DependsOn(
    typeof(AnonymizationServiceDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class AnonymizationServiceEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AnonymizationServiceEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            var defaultConnectionString = options.GetConnectionStringOrNull("AnonymizationServiceDbConnectionString");
            options.ConnectionStrings.Default = defaultConnectionString;
        });

        context.Services.AddAbpDbContext<AnonymizationServiceDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<TransmissionToUseCase, EfCoreRepository<AnonymizationServiceDbContext, TransmissionToUseCase, Guid>>();
        });
        context.Services.AddAbpDbContext<IntensiveCareDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            //options.AddRepository<IntensiveCareRepository,Encounter>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also AnonymizationServiceMigrationsDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });

        Configure<AbpEntityOptions>(options =>
        {
            options.Entity<ContainerImage>(containerImage =>
            {
                containerImage.DefaultWithDetailsFunc = query => query
                .Include(o => o.Variables)
                .Include(o => o.Volumes)
                .Include(o => o.InvocationHeaders);
            });

            options.Entity<PodDefinition>(podDefinition =>
            {
                podDefinition.DefaultWithDetailsFunc = query => query
                .Include(o => o.ContainerList)
                .Include(o => o.EnvironmentVariables);
            });

            options.Entity<LaboratoryExam>(orderOptions =>
            {
                orderOptions.DefaultWithDetailsFunc = query => query.Include(o => o.ValueResults);
            });
        });
    }
}
