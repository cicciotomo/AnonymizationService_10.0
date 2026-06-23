using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Containers
{
    public class ContainerImageVolumeDto : EntityDto
    {
        public string HostPath { get; set; }
        public string ContainerPath { get; set; }
    }
}