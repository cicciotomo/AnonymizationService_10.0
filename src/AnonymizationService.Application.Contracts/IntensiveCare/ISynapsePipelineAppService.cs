using AnonymizationService.IntensiveCare;
using AnonymizationService.Redcap;
using System.Threading.Tasks;

namespace AnonymizationService.SynapsePipeline
{
    public interface ISynapsePipelineAppService
    {
        Task<string> CreatePipelineRunAsync(PipelineParameterDto parameters);
        Task<string> CreateRedcapPipelineRunAsync(RedcapPipelineParameterDto parameters);
        Task<string> GetPipelineRunAsync(string runId);
        Task<string> GetPipelinesAsync();
    }
}
