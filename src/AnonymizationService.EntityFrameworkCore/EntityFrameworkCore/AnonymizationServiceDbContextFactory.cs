using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AnonymizationService.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class AnonymizationServiceDbContextFactory : IDesignTimeDbContextFactory<AnonymizationServiceDbContext>
{
    public AnonymizationServiceDbContext CreateDbContext(string[] args)
    {
        AnonymizationServiceEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<AnonymizationServiceDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SRaceAnonymizationServiceABP7;Integrated Security=True;").EnableSensitiveDataLogging();

        return new AnonymizationServiceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AnonymizationService.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
