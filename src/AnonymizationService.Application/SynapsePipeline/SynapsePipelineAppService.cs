using AnonymizationService.IntensiveCare;
using AnonymizationService.Permissions;
using AnonymizationService.Redcap;
using AnonymizationService.Services.SynapsePipeline;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnonymizationService.SynapsePipeline
{
    //[Authorize(AnonymizationServicePermissions.PlatformAdministrationPermission)]
    public class SynapsePipelineAppService : AnonymizationServiceAppService, ISynapsePipelineAppService
    {
        private readonly ISynapsePipelinesService _synapsePipelineService;

        public SynapsePipelineAppService(ISynapsePipelinesService synapsePipelineService)
        {
            _synapsePipelineService = synapsePipelineService;
        }


        public async Task<string> GetPipelinesAsync()
        {
            return await _synapsePipelineService.GetPipelinesAsync();
        }

        public async Task<string> GetPipelineRunAsync(string runId)
        {
            return await _synapsePipelineService.GetPipelineRunAsync(runId.ToLower());
        }

        public async Task<string> CreatePipelineRunAsync(PipelineParameterDto parameters)
        {
            return await _synapsePipelineService.CreatePipelineRunAsync(parameters);
        }

        public async Task<string> CreateRedcapPipelineRunAsync(RedcapPipelineParameterDto parameters)
        {
            return await _synapsePipelineService.CreateRedcapPipelineRunAsync(parameters);
        }
    }
}
