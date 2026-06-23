using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace AnonymizationService;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        //Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Information()
        //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        //    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
        //    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
        //    .MinimumLevel.Override("OpenIddict", LogEventLevel.Information)
        //    .MinimumLevel.Override("Volo", LogEventLevel.Information)
        //    .Enrich.FromLogContext()
        //    .WriteTo.Async(c => c.Console())
        //    .CreateLogger();

        Log.Logger = new LoggerConfiguration().CreateLogger();

        try
        {
            Log.Information("Starting AnonymizationService.HttpApi.Host.");
            var builder = WebApplication
                .CreateBuilder(args);

            var appConfigurationEndpoint = builder.Configuration.GetValue<string>("AppConfig:Endpoint");

            builder.Host.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(appConfigurationEndpoint), new DefaultAzureCredential())
                        .ConfigureKeyVault(kv =>
                        {
                            // Azure credentials are taken from ENV VARS
                            kv.SetCredential(new DefaultAzureCredential());
                        })
                        .ConfigureRefresh(refresh =>
                        {
                            refresh
                                .Register("GlobalSettings:SentinelKey", refreshAll: true)
                                .SetCacheExpiration(TimeSpan.FromSeconds(30));
                        })
                        .Select("Azure:*", LabelFilter.Null)
                        .Select("DataProtection:*", LabelFilter.Null)
                        .Select("Settings:*", LabelFilter.Null)
                        .Select("GlobalSettings:*", LabelFilter.Null)
                        .Select("DefaultImportParameterKeys:*", LabelFilter.Null)
                        .Select("ConnectionStrings:*", LabelFilter.Null)
                        .Select("CStoreSettings:*", LabelFilter.Null)
                        .Select("ApplicationSettings:*", LabelFilter.Null)
                        .Select("AnonimyzationService:*", LabelFilter.Null)
                        .Select("App:*", LabelFilter.Null)
                        .Select("Logging:*", LabelFilter.Null)
                        .Select("AuthServer:*", LabelFilter.Null)
                        .Select("StringEncryption:*", LabelFilter.Null)
                        .Select("Serilog:*", LabelFilter.Null);
                });
            });
            builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckMiddleware", LogLevel.None);

            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Host.AddAppSettingsSecretsJson().UseAutofac();

            builder.Host.UseSerilog((ctx, services, config) =>
            {
                config.ReadFrom.Configuration(ctx.Configuration)
                      .ReadFrom.Services(services)
                      .Enrich.FromLogContext()
                      .WriteTo.Console();
            });

            await builder.AddApplicationAsync<AnonymizationServiceHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
