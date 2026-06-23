using AnonymizationService.IntensiveCareData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace AnonymizationService.EntityFrameworkCore.IntensiveCare
{

    [ConnectionStringName("IntensiveCareDbConnectionString")]
    public class IntensiveCareDbContext : AbpDbContext<IntensiveCareDbContext>
    {      

        public DbSet<IntensiveCarePatientStringAttribute> PatientStringAttributes { get; set; }
        public DbSet<IntensiveCarePatientAdminState> PatientAdminStates { get; set; }
        public IntensiveCareDbContext(DbContextOptions<IntensiveCareDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IntensiveCarePatientStringAttribute>()
                .ToTable("_Export.PatientStringAttribute_")
                .HasKey(e => new { e.PatientId, e.Timestamp, e.Name });

            builder.Entity<IntensiveCarePatientAdminState>()
                .ToTable("_Export.Patient_")
                .HasKey(e => new { e.Id, e.Timestamp });

            
        }
    }


}
