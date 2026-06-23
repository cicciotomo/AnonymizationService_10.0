using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.PodDefinitions
{
    public class PodDefinition : Entity<Guid>
    {
        public string Name { get; set; }
        public string EntryPoint { get; set; }
        public int MaxNumberOfAllowedInstances { get; set; }
        public List<PodDefinitionContainer> ContainerList { get; set; }
        public List<PodEnvironmentVariable> EnvironmentVariables { get; set; }
        public bool Enabled { get; set; }
    }
}
