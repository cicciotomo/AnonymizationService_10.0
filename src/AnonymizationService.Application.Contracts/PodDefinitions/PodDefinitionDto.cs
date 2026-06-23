using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.PodDefinitions
{
    public class PodDefinitionDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string EntryPoint { get; set; }
        public int MaxNumberOfAllowedInstances { get; set; }
        public List<PodDefinitionContainerDto> ContainerList { get; set; }
        public List<PodEnvironmentVariableDto> EnvironmentVariables { get; set; }
        public bool Enabled { get; set; }
    }
}
