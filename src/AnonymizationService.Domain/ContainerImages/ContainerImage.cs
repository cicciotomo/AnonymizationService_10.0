using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace AnonymizationService.ContainerImages
{
    public class ContainerImage : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string DefaultContainerName { get; set; }
        public int? MinRam { get; set; }
        public int? MinCpu { get; set; }
        public int? MaxRam { get; set; }
        public int? MaxCpu { get; set; }
        public int? MaxAllowedContainers { get; set; }
        public int? ExposedPort { get; set; }
        public int? ForwardedPort { get; set; }
        public string ApiActionUrl { get; set; }
        public string ShortName { get; set; }
        public bool IsBinaryDataContent { get; set; }
        public List<ContainerImageVariable> Variables { get; set; } = new List<ContainerImageVariable>();
        public List<ContainerImageVolume> Volumes { get; set; }
        public List<ContainerInvocationHeader> InvocationHeaders { get; set; }
    }
}
