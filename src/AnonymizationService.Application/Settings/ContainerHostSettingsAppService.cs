using AnonymizationService.Containers.ContainerHosts;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.Settings
{
    public class ContainerHostSettingsAppService : AnonymizationServiceAppService
    {
        private readonly ConfigurationClient _configurationClient;
        private readonly ContainerHostSettings _containerHostSettings;

        public ContainerHostSettingsAppService(
            IOptionsSnapshot<ContainerHostSettings> containerHostSettings,
            ConfigurationClient configurationClient)
        {
            _configurationClient = configurationClient;
            _containerHostSettings = containerHostSettings.Value;
        }

        public ContainerHostSettings GetHostSettingsAsync() => _containerHostSettings;


        public async Task UpdateHostSettingsAsync(ContainerHostSettings containerHostSettings)
        {
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.DockerEnginePort)}", containerHostSettings.DockerEnginePort.ToString());
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.BaseTcpPort)}", containerHostSettings.BaseTcpPort.ToString());
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.CpuAvailable)}", containerHostSettings.CpuAvailable.ToString());
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.RamAvailable)}", containerHostSettings.RamAvailable.ToString());
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.IsLocalHost)}", containerHostSettings.IsLocalHost.ToString());
            await _configurationClient.SetConfigurationSettingAsync($"{ContainerHostSettings.SettingsName}:{nameof(ContainerHostSettings.RemoteUri)}", containerHostSettings.RemoteUri);

            //Refresh the sentinel key to trigger a refresh of the application settings
            await _configurationClient.SetConfigurationSettingAsync($"GlobalSettings:SentinelKey", Guid.NewGuid().ToString());
        }
    }
}
