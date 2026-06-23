using AnonymizationService.ContainerImages;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Containers.ContainerHosts
{
    public abstract class ContainerHost : ISingletonDependency
    {
        private static int KB_IN_MB = 1024;
        private static int KB_IN_B = 1024;

        protected DockerClient _dockerClient;

        private readonly ILogger<ContainerHost> _logger;

        public int CpuAvailable { get; }
        public int RamAvailable { get; }
        public int BaseTcpPort { get; }
        public abstract string HostUrl { get; }

        public ContainerHost(ContainerHostSettings containerHostSettings, ILogger<ContainerHost> logger)
        {
            _logger = logger;

            CpuAvailable = containerHostSettings.CpuAvailable;
            RamAvailable = containerHostSettings.RamAvailable;
            BaseTcpPort = containerHostSettings.BaseTcpPort;
        }

        public async Task<(string Id, string Name, IList<string> Warnings)> CreateContainerInstanceAsync(Container inputContainerData)
        {
            var portMappingDictionary = new Dictionary<string, IList<PortBinding>>();
            var exposedPorts = new Dictionary<string, EmptyStruct>();

            if (inputContainerData.PortMapping is not null)
            {
                exposedPorts.Add(inputContainerData.PortMapping.ContainerPort.ToString(), new EmptyStruct());
                portMappingDictionary.Add(inputContainerData.PortMapping.ContainerPort.ToString(), new List<PortBinding>() { new PortBinding() { HostIP = "0.0.0.0", HostPort = inputContainerData.PortMapping.HostPort.ToString() } });
            }

            if (inputContainerData.ExposedPort.HasValue
                && inputContainerData.ExposedPort.Value != inputContainerData.PortMapping?.ContainerPort)
            {
                exposedPorts.Add(inputContainerData.ExposedPort.Value.ToString(), new EmptyStruct());
            }

            var createContainerParams = new CreateContainerParameters()
            {
                Name = inputContainerData.Name,
                Image = inputContainerData.ImageName,
                ExposedPorts = exposedPorts,
                HostConfig = new HostConfig()
                {
                    PortBindings = portMappingDictionary,
                    Binds = inputContainerData.Volumes?.Select(x => $"{x.Key}:{x.Value}").ToList()
                },
                Env = inputContainerData.EnvVariables?.Select(x => $"{x.Key}={x.Value}").ToList(),
            };

            if (!string.IsNullOrEmpty(inputContainerData.NetworkId))
            {
                createContainerParams.NetworkingConfig = new NetworkingConfig
                {
                    EndpointsConfig = new Dictionary<string, EndpointSettings>
                    {
                        {
                            inputContainerData.NetworkName,
                            new EndpointSettings
                            {
                                NetworkID = inputContainerData.NetworkId
                            }
                        }
                    }
                };
            }

            if (inputContainerData.AllocatedRam.HasValue && inputContainerData.AllocatedRam.Value > 0)
            {
                createContainerParams.HostConfig.Memory = inputContainerData.AllocatedRam.Value * KB_IN_MB * KB_IN_B;
            }

            if (inputContainerData.AllocatedCpus.HasValue && inputContainerData.AllocatedCpus.Value > 0)
            {
                createContainerParams.HostConfig.CPUCount = inputContainerData.AllocatedCpus.Value;
            }

            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(createContainerParams);

            var container = await _dockerClient.Containers.InspectContainerAsync(createContainerResponse.ID);

            return (createContainerResponse.ID, container.Name.TrimStart('/'), createContainerResponse.Warnings);
        }

        public async Task<bool> StartContainerAsync(string containerId)
        {
            return await _dockerClient.Containers.StartContainerAsync(containerId, null);
        }

        public async Task<bool> StopContainerAsync(string containerId, bool remove = true)
        {
            var stopResult = await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            if (stopResult && remove)
            {
                await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true });
            }
            return stopResult;
        }

        public async Task<string> CreateNetworkAsync(string networkName)
        {
            var network = await _dockerClient.Networks.CreateNetworkAsync(new NetworksCreateParameters
            {
                Name = networkName,
                Driver = "bridge",
                Scope = "local"
            });

            return network.ID;
        }

        public Task DeleteNetworkAsync(string networkId)
            => _dockerClient.Networks.DeleteNetworkAsync(networkId);

        public async Task SyncWithStoreAsync(IContainerStore containerStore, IRepository<ContainerImage, Guid> repository)
        {
            containerStore.Clear();

            var containerImages = await repository.GetListAsync();

            foreach (var containerImage in containerImages)
            {
                containerStore.Add(new Container()
                {
                    ImageName = containerImage.Name,
                    Name = containerImage.ShortName,
                    AllocatedCpus = containerImage.MinCpu,
                    AllocatedRam = containerImage.MinRam,
                    EnvVariables = containerImage.Variables?.ToDictionary(x => x.Name, x => x.Value),
                    MaxAllowedContainers = containerImage.MaxAllowedContainers,
                    ImageShortName = containerImage.ShortName,
                    Url = containerImage.ApiActionUrl
                });
            }
        }
    }
}
