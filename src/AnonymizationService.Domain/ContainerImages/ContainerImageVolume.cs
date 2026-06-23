using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ContainerImages
{
    public class ContainerImageVolume : Entity<Guid>
    {
        public string HostPath { get; set; }
        public string ContainerPath { get; set; }
        public ContainerImage ContainerImage { get; set; }
    }
}
