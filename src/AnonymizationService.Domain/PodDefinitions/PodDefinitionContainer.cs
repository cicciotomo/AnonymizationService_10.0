using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.PodDefinitions
{
    public class PodDefinitionContainer : Entity<Guid>
    {
        public string ShortName { get; set; }
        public string Version { get; set; }
        public string PodEnvironmentVariablesUsed { get; set; }
        public PodDefinition PodDefinition { get; set; }
    }
}
