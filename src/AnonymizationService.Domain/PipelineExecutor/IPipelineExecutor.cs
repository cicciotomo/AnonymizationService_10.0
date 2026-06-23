using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.PipelineExecutor
{
    public interface IPipelineExecutor
    {
        Task<List<PipelineExecutionResult>> ExecuteAsync(PipelineDefinition[] pipelineDefinition, int documentId);
    }
}