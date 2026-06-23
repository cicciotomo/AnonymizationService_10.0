using AnonymizationService.ContainerImages;
using AnonymizationService.PodDefinitions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Pods
{
    public interface IPodOrchestrator
    {
        Task DisposePodAsync(Pod pod);
        Task<Pod> ExecutePodAsync(PodDefinition podDefinition, List<ContainerImage> containerImages);
        Task InitializeAsync();
    }
}