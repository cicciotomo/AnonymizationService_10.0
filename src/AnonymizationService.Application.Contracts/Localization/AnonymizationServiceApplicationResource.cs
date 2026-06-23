using Volo.Abp.Localization;

namespace AnonymizationService.Localization
{
    [LocalizationResourceName("AnonymizationServiceApplicationContracts")]
    public class AnonymizationServiceApplicationContractsResource
    {
        public const string CreatePatientBatchRowErrorValidationMessage = "CreatePatientBatch:RowErrorValidationMessage";
        public const string CreatePatientInputParametersValidationMessage = "CreatePatient:InputParametersValidationMessage";
        public const string InvalidCpuForContainerValidationMessage = "ContainerImage:InvalidCpuForContainerValidationMessage";
        public const string InvalidRamForContainerValidationMessage = "ContainerImage:InvalidRamForContainerValidationMessage";
        public const string InvalidPortsForContainerValidationMessage = "ContainerImage:InvalidPortsForContainerValidationMessage";
        public const string InvalidEntryPointForPodDefinitionValidationMessage = "PodDefinition:InvalidEntryPointForPodDefinitionValidationMessage";
        public const string InvalidContainerListForPodDefinitionValidationMessage = "PodDefinition:InvalidContainerListForPodDefinitionValidationMessage";
        public const string InvalidEnvironmentVariableInContainerForPodDefinitionValidationMessage = "PodDefinition:InvalidEnvironmentVariableInContainerForPodDefinitionValidationMessage";
        public const string InvalidEnvironmentVariableForPodDefinitionValidationMessage = "PodDefinition:InvalidEnvironmentVariableForPodDefinitionValidationMessage";
    }
}
