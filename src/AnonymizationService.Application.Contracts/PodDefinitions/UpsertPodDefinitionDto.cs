using AnonymizationService.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AnonymizationService.PodDefinitions
{
    public class UpsertPodDefinitionDto : IValidatableObject
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string EntryPoint { get; set; }
        public int MaxNumberOfAllowedInstances { get; set; }
        public List<PodDefinitionContainerDto> ContainerList { get; set; }
        public List<PodEnvironmentVariableDto> EnvironmentVariables { get; set; }
        [Required]
        public bool Enabled { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer = validationContext.GetService<IStringLocalizer<AnonymizationServiceApplicationContractsResource>>();

            if (ContainerList is null || ContainerList.Count == 0)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidContainerListForPodDefinitionValidationMessage]);
            }

            if (!ContainerList.Any(p => p.ShortName == EntryPoint))
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidEntryPointForPodDefinitionValidationMessage]);
            }

            foreach (var container in ContainerList)
            {
                if (!string.IsNullOrEmpty(container.PodEnvironmentVariablesUsed))
                {
                    var environmentVariablesUsed = container.PodEnvironmentVariablesUsed.Split(',');
                    foreach (var environmentVariable in environmentVariablesUsed)
                    {
                        if (!EnvironmentVariables.Any(e => e.Name == environmentVariable))
                        {
                            yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidEnvironmentVariableInContainerForPodDefinitionValidationMessage, container.ShortName]);
                        }
                    }
                }
            }

            if (EnvironmentVariables != null && EnvironmentVariables.Count > 0)
            {
                foreach (var environmentVariable in EnvironmentVariables)
                {
                    if (!ContainerList.Any(c => c.ShortName == environmentVariable.ProvidedByContainer))
                    {
                        yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.InvalidEnvironmentVariableForPodDefinitionValidationMessage]);
                    }
                }
            }
        }
    }
}
