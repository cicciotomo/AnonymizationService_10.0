using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.PodDefinitions
{
    public class PodDefinitionContainerDto : EntityDto<Guid>
    {
        public string ShortName { get; set; }
        public string Version { get; set; }
        public string PodEnvironmentVariablesUsed { get; set; }
    }
}