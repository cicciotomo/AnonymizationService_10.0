using AnonymizationService.ContainerImages;
using AnonymizationService.Containers.ContainerHosts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Containers.ContainerOrchestrator
{
    public class ContainerOrchestratorService : ITransientDependency, IContainerOrchestratorService
    {
        private readonly IContainerStore _containerStore;
        private readonly ContainerHost _containersHost;
        private readonly ILogger<ContainerOrchestratorService> _logger;
        private readonly IConfiguration _configuration;

        public ContainerOrchestratorService(IContainerStore containerStore, ContainerHost containerHost, ILogger<ContainerOrchestratorService> logger, IConfiguration configuration)
        {
            _containerStore = containerStore;
            _containersHost = containerHost;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> StopContainerAsync(string containerId)
        {
            if (await _containersHost.StopContainerAsync(containerId))
            {
                var containerStopped = _containerStore.RemoveById(containerId) > 0;
                _logger.LogInformation($"Container with Id {containerId} stopped");
                return containerStopped;
            }

            _logger.LogError($"Unable to stop container with Id {containerId}");

            return false;
        }

        public async Task<Container> EnsureOneContainerIsRunningAsync(ContainerImage containerImage, ContainerNetworkConfiguration containerNetworkConfiguration = null)
        {
            var runningContainersForImage = _containerStore.GetRunningContainersByImageName(containerImage.Name);
            if (runningContainersForImage?.Any() == false)
            {
                return await TryRunContainerForImageAsync(containerImage, containerNetworkConfiguration);
            }

            return runningContainersForImage.FirstOrDefault();
        }

        public async Task<Container> TryRunContainerForImageAsync(ContainerImage containerImage, ContainerNetworkConfiguration containerNetworkConfiguration = null)
        {
            var containerToStart = new Container
            {
                Name = containerImage.DefaultContainerName,
                AllocatedCpus = containerImage.MinCpu,
                AllocatedRam = containerImage.MinRam,
                EnvVariables = containerImage.Variables?.ToDictionary(
                    x => x.Name,
                    x =>
                    {
                        if (!x.FromAppConfiguration)
                        {
                            return x.Value;
                        }

                        var value = _configuration.GetValue<string>(x.Value);
                        if (value is null)
                        {
                            _logger.LogError($"The {x.Value} variable does not exist in App Configuration");
                            throw new Exception($"The {x.Value} variable does not exist in App Configuration");
                        }

                        return value;
                    }
                ),
                ImageName = containerImage.Name,
                ImageShortName = containerImage.ShortName,
                MaxAllowedContainers = containerImage.MaxAllowedContainers,
                ExposedPort = containerImage.ExposedPort,
                Volumes = containerImage.Volumes?.ToDictionary(x => x.HostPath, x => x.ContainerPath),
                NetworkId = containerNetworkConfiguration?.Id,
                NetworkName = containerNetworkConfiguration?.Name
            };

            if (containerImage.ForwardedPort.HasValue)
            {
                containerToStart.PortMapping = new ContainerPortMapping(GetNextPortForContainer(), containerImage.ForwardedPort.Value);
            }

            if (!CanSpinUpContainer(containerToStart))
            {
                return null;
            }

            (string containerId, string containerName, var warnings) = await _containersHost.CreateContainerInstanceAsync(containerToStart);

            if (warnings != null && warnings.Count > 0)
            {
                var exc = new UserFriendlyException(string.Join(',', warnings));
                _logger.LogException(exc);
                throw exc;
            }

            containerToStart.ContainerId = containerId;
            containerToStart.Name = containerName;

            if (await _containersHost.StartContainerAsync(containerId))
            {
                containerToStart.StartTime = DateTime.Now;
                if (containerToStart.PortMapping is not null)
                {
                    containerToStart.Url = string.Format(containerImage.ApiActionUrl, $"{_containersHost.HostUrl}:{containerToStart.PortMapping.HostPort}");
                }
                _containerStore.Add(containerToStart);
                return containerToStart;
            }

            _logger.LogWarning("Unable to start container with Id {containerId}", containerId);

            return null;
        }

        public Task<string> CreateNetworkAsync(string networkName)
            => _containersHost.CreateNetworkAsync(networkName);

        public Task DeleteNetworkAsync(string networkId)
            => _containersHost.DeleteNetworkAsync(networkId);

        private int GetNextPortForContainer()
        {
            var portsInUse = _containerStore.PortsInUseByRunningContainers();
            var rng = new Random();

            var nextPort = _containersHost.BaseTcpPort + rng.Next(0, 100);

            while (portsInUse.Contains(nextPort))
            {
                nextPort = _containersHost.BaseTcpPort + rng.Next(0, 100);
            }

            return nextPort;
        }

        private bool CanSpinUpContainer(Container containerToSpinUp)
        {
            if (_containerStore.CpuInUseByContainers() + (containerToSpinUp.AllocatedCpus ?? 0) > _containersHost.CpuAvailable)
            {
                throw new UserFriendlyException(message: $"Not enough Cpus on host machine to start a new container for image {containerToSpinUp.ImageName}");
            }

            if (_containerStore.RamInUseByContainers() + (containerToSpinUp.AllocatedRam ?? 0) > _containersHost.RamAvailable)
            {
                throw new UserFriendlyException(message: $"Not enough Ram on host machine to start a new container for image {containerToSpinUp.ImageName}");
            }

            var runningContainersForImage = _containerStore.RunningContainerCountByImage(containerToSpinUp.ImageName);
            var maxContainersAllowedForImage = containerToSpinUp.MaxAllowedContainers;

            if (maxContainersAllowedForImage.HasValue && runningContainersForImage >= maxContainersAllowedForImage)
            {
                throw new UserFriendlyException(message: $"There are already {runningContainersForImage} containers running for image {containerToSpinUp.ImageName}");
            }

            return true;
        }

    }
}
