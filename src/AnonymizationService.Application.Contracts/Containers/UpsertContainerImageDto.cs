using AnonymizationService.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnonymizationService.Containers
{
    public class UpsertContainerImageDto : IValidatableObject
    {
        [Required]
        public string Name { get; set; }
        public string DefaultContainerName { get; set; }
        public int? MinRam { get; set; }
        public int? MinCpu { get; set; }
        public int? MaxRam { get; set; }
        public int? MaxCpu { get; set; }
        public int? MaxAllowedContainers { get; set; }
        public int? ExposedPort { get; set; }
        public int? ForwardedPort { get; set; }

        [Required]
        public string ApiActionUrl { get; set; }

        [Required]
        public string ShortName { get; set; }
        public IEnumerable<ContainerImageVariableDto> Variables { get; set; }
        public IEnumerable<ContainerImageVolumeDto> Volumes { get; set; }
        public IEnumerable<ContainerInvocationHeaderDto> InvocationHeaders { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer = validationContext.GetService<IStringLocalizer<AnonymizationServiceApplicationContractsResource>>();

            if (MaxCpu.HasValue && MinCpu.HasValue
                && MaxCpu.Value < MinCpu.Value)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidCpuForContainerValidationMessage]);
            }

            if (MaxRam.HasValue && MinRam.HasValue
                && MaxRam.Value < MinRam.Value)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidRamForContainerValidationMessage]);
            }

            if (!ForwardedPort.HasValue && !ExposedPort.HasValue)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidPortsForContainerValidationMessage]);
            }
        }
    }
}
