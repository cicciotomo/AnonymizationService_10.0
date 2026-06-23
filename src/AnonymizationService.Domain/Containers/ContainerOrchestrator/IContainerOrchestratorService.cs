using AnonymizationService.ContainerImages;
using System.Threading.Tasks;

namespace AnonymizationService.Containers.ContainerOrchestrator
{
    public interface IContainerOrchestratorService
    {
        Task<bool> StopContainerAsync(string containerId);
        Task<Container> TryRunContainerForImageAsync(ContainerImage containerImage, ContainerNetworkConfiguration containerNetworkConfiguration = null);
        Task<Container> EnsureOneContainerIsRunningAsync(ContainerImage containerImage, ContainerNetworkConfiguration containerNetworkConfiguration = null);
        Task<string> CreateNetworkAsync(string networkName);
        Task DeleteNetworkAsync(string networkId);
    }
}