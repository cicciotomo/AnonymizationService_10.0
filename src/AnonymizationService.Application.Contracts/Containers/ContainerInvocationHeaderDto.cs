using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Containers
{
    public class ContainerInvocationHeaderDto : EntityDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool FromAppConfiguration { get; set; }
    }
}