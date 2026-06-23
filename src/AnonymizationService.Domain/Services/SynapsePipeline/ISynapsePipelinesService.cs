using AnonymizationService.IntensiveCare;
using AnonymizationService.Redcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.SynapsePipeline
{
    public interface ISynapsePipelinesService
    {
        Task<string> GetPipelinesAsync();
        Task<string> CreatePipelineRunAsync(PipelineParameterDto parameters);
        Task<string> CreateRedcapPipelineRunAsync(RedcapPipelineParameterDto parameters);
        Task<string> GetPipelineRunAsync(string runId);
    }
}
