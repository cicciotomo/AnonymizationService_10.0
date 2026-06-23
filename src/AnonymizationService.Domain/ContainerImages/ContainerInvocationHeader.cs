using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ContainerImages
{
    public class ContainerInvocationHeader : Entity<Guid>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool FromAppConfiguration { get; set; }
        public ContainerImage ContainerImage { get; set; }
    }
}