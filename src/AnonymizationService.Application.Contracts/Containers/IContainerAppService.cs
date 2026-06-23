using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Containers
{
    public interface IContainerAppService
    {
        Task<List<string>> TryRunPipelineExecutorAsync(string filePath);
    }
}