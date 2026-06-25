using AnonymizationService.ClinicalDocuments;
using AnonymizationService.ContainerImages;
using AnonymizationService.DbUriData;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.DicomData;
using AnonymizationService.ExternalServiceRequestLimits;
using AnonymizationService.HospitalPatients;
using AnonymizationService.IntensiveCareData;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.PathoxData;
using AnonymizationService.PatientUploadRequests;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Redcap;
using AnonymizationService.ScheduledUploadParameters;
using AnonymizationService.StateMachines.Clinical;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Dicom;
using AnonymizationService.StateMachines.IntensiveCare;
using AnonymizationService.StateMachines.Laboratory;
using AnonymizationService.StateMachines.LoadPatientData;
using AnonymizationService.StateMachines.Pathox;
using AnonymizationService.StateMachines.Redcap;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace AnonymizationService.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("AnonymizationServiceDbConnectionString")]
public class AnonymizationServiceDbContext :
    AbpDbContext<AnonymizationServiceDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }


    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }


    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    public DbSet<IntensiveCarePatientData> IntensiveCarePatientDatas { get; set; }

    #endregion

    public AnonymizationServiceDbContext(DbContextOptions<AnonymizationServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var converter = new ValueConverter<ClinicalDocumentTypeFilter[], string>(
    v => SerializeArray(v),
    v => DeserializeArraySafe(v)
);

var comparer = new ValueComparer<ClinicalDocumentTypeFilter[]>(
    (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions)null),
    v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions)null).GetHashCode(),
    v => v == null
        ? Array.Empty<ClinicalDocumentTypeFilter>()
        : JsonSerializer.Deserialize<ClinicalDocumentTypeFilter[]>(
            JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            (JsonSerializerOptions)null
        )
);

        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        builder.Entity<LaboratoryExam>(e =>
        {
            e.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "LaboratoryExams", AnonymizationServiceConsts.DB_SCHEMA);
            e.HasOne<HospitalPatient>()
                .WithMany(p => p.LaboratoryExams)
                .HasForeignKey(l => l.CloudPatientId)
                .OnDelete(DeleteBehavior.Cascade);

            e.ConfigureByConvention();
        });

        builder.Entity<ClinicalDocument>(e =>
        {
            e.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ClinicalDocuments", AnonymizationServiceConsts.DB_SCHEMA);
            e.HasOne<HospitalPatient>()
                .WithMany(p => p.ClinicalDocuments)
                .HasForeignKey(c => c.CloudPatientId)
                .OnDelete(DeleteBehavior.Cascade);

            e.Property(x => x.Id).ValueGeneratedNever();
            //e.Property(e => e.ClinicalDocumentTypeFilters).HasConversion(converter);
            e.ConfigureByConvention();
        });


        builder.Entity<ClinicalDocumentType>(e =>
        {
            e.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ClinicalDocumentTypes", AnonymizationServiceConsts.DB_SCHEMA);
            e.ConfigureByConvention();
        });




        builder.Entity<LaboratoryExamResult>(e =>
        {
            e.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "LaboratoryExamResults", AnonymizationServiceConsts.DB_SCHEMA);
            e.HasOne<LaboratoryExam>()
                .WithMany(p => p.ValueResults)
                .OnDelete(DeleteBehavior.Cascade);

            e.ConfigureByConvention();
        });

        builder.Entity<StateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "StateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.CurrentState);
        });

        builder.Ignore(typeof(ContextStateMachine<>));

        builder.Entity<DicomStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DicomStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

        builder.Entity<LaboratoryStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "LaboratoryStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

        builder.Entity<ClinicalStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ClinicalStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

		builder.Entity<DbUriStateMachine>(b =>
		{
			b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DbUriStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
			b.ConfigureByConvention();
			b.Ignore(s => s.ContextData);
		});

		builder.Entity<PathoxStateMachine>(b =>
		{
			b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PathoxStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
			b.ConfigureByConvention();
			b.Ignore(s => s.ContextData);
		});

        builder.Entity<IntensiveCareStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "IntensiveCareStateMachine", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

        builder.Entity<RedcapStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "RedcapStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

        builder.Entity<LoadPatientDataStateMachine>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "LoadPatientDataStateMachines", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
            b.Ignore(s => s.ContextData);
        });

        builder.Entity<HospitalPatient>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "HospitalPatients", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasKey(p => p.CloudPatientId);
            b.ConfigureByConvention();
        });

        builder.Entity<PatientUploadRequest>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PatientUploadRequests", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasOne<StateMachine>().WithOne().HasForeignKey<PatientUploadRequest>(p => p.StateMachineId);
            b.Property(e => e.ClinicalStateMachineDocumentType).HasConversion(converter);
            b.ConfigureByConvention();
        });

        builder.Entity<PatientBatchUploadRequest>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PatientBatchUploadRequests", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<ContainerImage>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ContainerImages", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasMany(x => x.Variables)
                .WithOne(x => x.ContainerImage)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Volumes)
                .WithOne(x => x.ContainerImage)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.InvocationHeaders)
                .WithOne(x => x.ContainerImage)
                .OnDelete(DeleteBehavior.Cascade);
            b.ConfigureByConvention();
        });

        builder.Entity<ContainerImageVariable>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ContainerImageVariables", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<ContainerInvocationHeader>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ContainerInvocationHeaders", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<ContainerImageVolume>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ContainerImageVolumes", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<DicomStudy>(e =>
        {
            e.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DicomStudy", AnonymizationServiceConsts.DB_SCHEMA);
            e.ConfigureByConvention();
        });

        builder.Entity<DicomSerie>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DicomSeries", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<LaboratoryCodeMap>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "LaboratoryCodeMaps", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasKey(e => new { e.SourceCode, e.DestinationSystem });
            b.ConfigureByConvention();
        });

        builder.Entity<PodDefinition>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PodDefinitions", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasMany(e => e.ContainerList)
                .WithOne(p => p.PodDefinition)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(e => e.EnvironmentVariables)
                .WithOne(p => p.PodDefinition)
                .OnDelete(DeleteBehavior.Cascade);
            b.Property(e => e.Enabled).HasDefaultValue(true);
            b.ConfigureByConvention();
        });

        builder.Entity<PodDefinitionContainer>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PodDefinitionContainers", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<PodEnvironmentVariable>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PodEnvironmentVariables", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<DefaultImportParameter>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DefaultImportParameters", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        builder.Entity<ExternalServiceRequestLimit>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ExternalServiceRequestLimits", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

        
        builder.Entity<ScheduledIncrementalUploadParameter>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "ScheduledIncrementalUploadParameters", AnonymizationServiceConsts.DB_SCHEMA);
            b.ConfigureByConvention();
        });

		builder.Entity<DbUriEvent>(b =>
		{
			b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DbUriEvents", AnonymizationServiceConsts.DB_SCHEMA);
			b.HasOne<HospitalPatient>()
				.WithMany(p => p.DbUriEvents)
				.HasForeignKey(c => c.CloudPatientId)
				.OnDelete(DeleteBehavior.Cascade);
			b.Property(x => x.Id).ValueGeneratedNever();
			b.ConfigureByConvention();
		});


		builder.Entity<DbUriFUpItem>(b =>
		{
			b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "DbUriFUpItems", AnonymizationServiceConsts.DB_SCHEMA);
			b.HasOne<HospitalPatient>()
				.WithMany(p => p.DbUriFUpItems)
				.HasForeignKey(c => c.CloudPatientId)
				.OnDelete(DeleteBehavior.Cascade);
			b.Property(x => x.Id).ValueGeneratedNever();
			b.ConfigureByConvention();
		});

		builder.Entity<PathoxExam>(b =>
		{
			b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "PathoxExams", AnonymizationServiceConsts.DB_SCHEMA);
			b.HasOne<HospitalPatient>()
				.WithMany(p => p.PathoxExams)
				.HasForeignKey(c => c.CloudPatientId)
				.OnDelete(DeleteBehavior.Cascade);
			b.Property(x => x.Id).ValueGeneratedNever();
			b.ConfigureByConvention();
		});

        builder.Entity<RedcapStudyConfiguration>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "RedcapStudiesConfigurations", AnonymizationServiceConsts.DB_SCHEMA);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.ConfigureByConvention();
        });

        builder.Entity<RedcapPatientData>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "RedcapPatientData", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasOne<RedcapStudyConfiguration>()
            .WithMany()
            .HasForeignKey(c => c.RedcapStudyId);
            b.Property(x => x.Id).ValueGeneratedNever();
			b.ConfigureByConvention();
		});

        builder.Entity<TransmissionToUseCase>(b =>
        {
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "TransmissionsToUseCases", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasOne<RedcapStudyConfiguration>()
            .WithMany()
            .HasForeignKey(c => c.RedcapStudyConfigurationId);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.ConfigureByConvention();
        });

        builder.Entity<IntensiveCarePatientData>(b =>
        {
            b.HasKey(x => x.Id);
            b.ToTable(AnonymizationServiceConsts.DB_TABLE_PREFIX + "IntensiveCarePatientDatas", AnonymizationServiceConsts.DB_SCHEMA);
            b.HasOne<HospitalPatient>()
                .WithMany(p => p.IntensiveCarePatientDatas)
                .HasForeignKey(c => c.CloudPatientId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Property(x => x.Id).ValueGeneratedOnAdd();
            b.ConfigureByConvention();
        });
    }

    private static string SerializeArray(ClinicalDocumentTypeFilter[] value)
    {
        return JsonSerializer.Serialize(value, (JsonSerializerOptions)null);
    }

    private static ClinicalDocumentTypeFilter[] DeserializeArraySafe(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<ClinicalDocumentTypeFilter>();

        try
        {
            return JsonSerializer.Deserialize<ClinicalDocumentTypeFilter[]>(
                value,
                (JsonSerializerOptions)null
            ) ?? Array.Empty<ClinicalDocumentTypeFilter>();
        }
        catch
        {
            return Array.Empty<ClinicalDocumentTypeFilter>();
        }
    }
}
