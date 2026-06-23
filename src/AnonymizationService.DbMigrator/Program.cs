using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AnonymizationService.DbMigrator;

class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
                .MinimumLevel.Override("AnonymizationService", LogEventLevel.Debug)
#else
                .MinimumLevel.Override("AnonymizationService", LogEventLevel.Information)
#endif
                .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, build) =>
            {
                build
                    .AddJsonFile("appsettings.json", optional: false, true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            })
            .AddAppSettingsSecretsJson()
            .ConfigureLogging((context, logging) => logging.ClearProviders())
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DbMigratorHostedService>();
            });

        var localConfig = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddEnvironmentVariables()
         .AddJsonFile("appsettings.json", false, true)
         .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
         .Build();

        hostBuilder.ConfigureHostConfiguration((configurationBuilder) =>
        {
            configurationBuilder
                .AddConfiguration(localConfig)
                .AddAzureAppConfiguration(options =>
                {
                    var appConfigurationEndpoint = localConfig.GetValue<string>("AppConfig:Endpoint");

                    options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential())
                        .ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(new DefaultAzureCredential());
                        })
                        .Select("Azure:*", LabelFilter.Null)
                        .Select("DataProtection:*", LabelFilter.Null)
                        .Select("Settings:*", LabelFilter.Null)
                        .Select("GlobalSettings:*", LabelFilter.Null)
                        .Select("DefaultImportParameterKeys:*", LabelFilter.Null)
                        .Select("ConnectionStrings:*", LabelFilter.Null)
                        .Select("CStoreSettings:*", LabelFilter.Null)
                        .Select("ApplicationSettings:*", LabelFilter.Null)
                        .Select("OpenIddict:Applications:*", LabelFilter.Null);
                })
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);
        });

        return hostBuilder;
    }
}
