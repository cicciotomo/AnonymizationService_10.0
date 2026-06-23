using AnonymizationService.Enums;
using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.PodDefinitions
{
    public class PodEnvironmentVariable : Entity<Guid>
    {
        public string Name { get; set; }
        public string ProvidedByContainer { get; set; }
        public PodEnvironmentVariableProvisionMode ProvisionMode { get; set; }
        public PodDefinition PodDefinition { get; set; }
    }
}
