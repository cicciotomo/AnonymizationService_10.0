using AnonymizationService.ContainerImages;
using AnonymizationService.Containers;
using AnonymizationService.Containers.ContainerOrchestrator;
using AnonymizationService.Enums;
using AnonymizationService.PodDefinitions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Pods
{
    public class PodOrchestrator : IPodOrchestrator, ISingletonDependency
    {
        private readonly IContainerOrchestratorService _containerOrchestratorService;
        private readonly IRepository<ContainerImage, Guid> _containerImageRepository;
        private readonly IRepository<PodDefinition> _podDefinitionRepository;
        private readonly ILogger<PodOrchestrator> _logger;
        private readonly ConcurrentDictionary<string, List<Pod>> _runningPods;

        public PodOrchestrator(IContainerOrchestratorService containerOrchestratorService, IRepository<ContainerImage, Guid> containerImageRepository, IRepository<PodDefinition> podDefinitionRepository, ILogger<PodOrchestrator> logger)
        {
            _containerOrchestratorService = containerOrchestratorService;
            _containerImageRepository = containerImageRepository;
            _podDefinitionRepository = podDefinitionRepository;
            _logger = logger;
            _runningPods = new ConcurrentDictionary<string, List<Pod>>();
        }

        public async Task<Pod> ExecutePodAsync(PodDefinition podDefinition, List<ContainerImage> containerImages)
        {
            _logger.LogInformation("Executing pod {podName}", podDefinition.Name);

            // Retrieve PodList from RunningPods
            if (!_runningPods.TryGetValue(podDefinition.Name, out var runningPods))
            {
                _logger.LogError("Unable to find requested pod in pod list");
                throw new Exception("Unable to find requested pod in pod list");
            }
            if (podDefinition.MaxNumberOfAllowedInstances > 1
                && runningPods.Count < podDefinition.MaxNumberOfAllowedInstances)
            {
                _logger.LogInformation("Spinning up new pod for {podName} as maximum number of replicas has not been reached", podDefinition.Name);

                var pod = await SpinUpNewPodAsync(containerImages, podDefinition);

                runningPods.Add(pod);
                return pod;
            }
            else
            {
                _logger.LogInformation("Returning base pod for {podName} as maximum number of replicas have been reached", podDefinition.Name);
                // Return base pod address
                var basePod = runningPods.SingleOrDefault(p => p.IsBase);
                basePod.RunningRequests++;
                return basePod;
            }

        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing pods");

            var podDefinitions = await _podDefinitionRepository.GetListAsync(includeDetails: true);
            var containerImages = await _containerImageRepository.GetListAsync(includeDetails: true);

            foreach (var podDefinition in podDefinitions.Where(x => x.Enabled))
            {
                var isPodAlreadyRunning = false;

                if (isPodAlreadyRunning)
                {
                    // TODO: Handle Pod reconciliation from existing containers
                }
                else
                {
                    try
                    {
                        var pod = await SpinUpNewPodAsync(containerImages, podDefinition, isBase: true);
                        _runningPods.TryAdd(pod.PodDefinitionName, new List<Pod>() { pod });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Unable to spin up Pod with name {podDefinition.Name}");
                    }
                }
            }
        }

        public async Task DisposePodAsync(Pod pod)
        {
            if (pod.IsBase)
            {
                pod.RunningRequests--;
                return;
            }

            foreach (var container in pod.RunningContainers)
            {
                await _containerOrchestratorService.StopContainerAsync(container.ContainerId);
            }

            if (pod.NetworkConfiguration is not null)
            {
                await _containerOrchestratorService.DeleteNetworkAsync(pod.NetworkConfiguration.Id);
            }

            var runningPodsByPodNameList = _runningPods[pod.PodDefinitionName];
            runningPodsByPodNameList.Remove(pod);
        }

        private async Task<Pod> SpinUpNewPodAsync(IReadOnlyCollection<ContainerImage> containerImages, PodDefinition podDefinition, bool isBase = false)
        {
            var pod = new Pod()
            {
                Id = Guid.NewGuid(),
                IsBase = isBase,
                PodDefinitionName = podDefinition.Name,
                RunningRequests = 0,
                RunningContainers = new List<Container>()
            };

            var podEnvironmentVariables = new Dictionary<string, string>();

            if (podDefinition.ContainerList?.Count > 1)
            {
                _logger.LogInformation($"Creating network for pod {podDefinition.Name}");

                var networkName = $"customNetwork{pod.Id:N}";
                var networkId = await _containerOrchestratorService.CreateNetworkAsync(networkName);

                pod.NetworkConfiguration = new ContainerNetworkConfiguration()
                {
                    Name = networkName,
                    Id = networkId
                };
            }

            // Spin up containers associated with pod
            foreach (var containerToSpinUp in podDefinition.ContainerList)
            {
                _logger.LogInformation($"Spinning up container {containerToSpinUp.ShortName} for pod {podDefinition.Name}");

                var environmentVariablesToProvide = new List<string>();

                if (!string.IsNullOrWhiteSpace(containerToSpinUp.PodEnvironmentVariablesUsed))
                {
                    environmentVariablesToProvide = containerToSpinUp.PodEnvironmentVariablesUsed.Split(',').Select(e => e.Trim()).ToList();
                }

                var containerImage = containerImages.FirstOrDefault(x => x.ShortName == containerToSpinUp.ShortName);
                if (containerImage == null)
                {
                    _logger.LogError($"{containerToSpinUp.ShortName} image not found");
                    throw new NotImplementedException($"{containerToSpinUp.ShortName} image not found");
                }

                foreach (var environmentVariableUsed in environmentVariablesToProvide)
                {
                    containerImage.Variables.Add(new ContainerImageVariable()
                    {
                        FromAppConfiguration = false,
                        Name = environmentVariableUsed,
                        Value = podEnvironmentVariables[environmentVariableUsed]
                    });
                }

                Container container;

                if (!isBase)
                {
                    container = await _containerOrchestratorService.TryRunContainerForImageAsync(containerImage, pod.NetworkConfiguration);
                }
                else
                {
                    container = await _containerOrchestratorService.EnsureOneContainerIsRunningAsync(containerImage, pod.NetworkConfiguration);
                }

                foreach (var environmentVariable in podDefinition.EnvironmentVariables.Where(e => e.ProvidedByContainer == containerToSpinUp.ShortName))
                {
                    var variableValue = environmentVariable.ProvisionMode switch
                    {
                        PodEnvironmentVariableProvisionMode.ContainerName => container.Name,
                        PodEnvironmentVariableProvisionMode.ContainerPort => container.PortMapping?.HostPort.ToString(),
                        _ => throw new Exception("Unable to handle environment variable retrieve mode")
                    };

                    podEnvironmentVariables.Add(environmentVariable.Name, variableValue);
                }

                pod.RunningContainers.Add(container);
            }

            var containerEntryPoint = pod.RunningContainers.SingleOrDefault(c => c.ImageShortName == podDefinition.EntryPoint);
            pod.ContainerEntryPoint = containerEntryPoint;

            return pod;
        }
    }
}