using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Containers
{
    public class ContainerImageDto : AuditedEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DefaultContainerName { get; set; }
        public int? MinRam { get; set; }
        public int? MinCpu { get; set; }
        public int? MaxRam { get; set; }
        public int? MaxCpu { get; set; }
        public int? MaxAllowedContainers { get; set; }
        public int? ForwardedPort { get; set; }
        public int? ExposedPort { get; set; }
        public string ApiActionUrl { get; set; }
        public string ShortName { get; set; }
        public bool IsBinaryDataContent { get; set; }
        public IEnumerable<ContainerImageVariableDto> Variables { get; set; }
        public IEnumerable<ContainerImageVolumeDto> Volumes { get; set; }
        public IEnumerable<ContainerInvocationHeaderDto> InvocationHeaders { get; set; }
    }
}
