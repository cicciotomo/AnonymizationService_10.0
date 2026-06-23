using Volo.Abp.Localization;

namespace AnonymizationService.Localization;

[LocalizationResourceName("AnonymizationService")]
public class AnonymizationServiceResource
{
    public const string MpiValidationFailedMessage = "MpiValidationFailedMessage";

    public const string PatientUploadSuccessNotificationMessage = "PatientUploadSuccessNotificationMessage";
    public const string PatientUploadFailedNotificationMessage = "PatientUploadFailedNotificationMessage";
    public const string PatientBatchUploadCompletedNotificationMessage = "PatientBatchUploadCompletedNotificationMessage";

    public const string EmptyFileExceptionMessage = "EmptyFileExceptionMessage";
    public const string InvalidFileExceptionMessage = "InvalidFileExceptionMessage";
    public const string UnparsableFileExceptionMessage = "UnparsableFileExceptionMessage";

    public const string CreateContainerImageAlreadyExistingShortNameValidationMessage = "CreateContainerImage:AlreadyExistingShortNameValidationMessage";
    public const string ContainerNotExistingValidationMessage = "ContainerImage:ContainerNotExistingValidationMessage";
    public const string InvalidContainerForPodDefinitionValidationMessage = "PodDefinition:InvalidContainerForPodDefinitionValidationMessage";
    public const string CreatePodDefinitionAlreadyExistingNameValidationMessage = "CreatePodDefinition:AlreadyExistingNameValidationMessage";
    public const string PodDefinitionNotExistingValidationMessage = "PodDefinition:NotExistingValidationMessage";
}
