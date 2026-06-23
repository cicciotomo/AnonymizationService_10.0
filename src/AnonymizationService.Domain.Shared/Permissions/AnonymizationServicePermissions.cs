namespace AnonymizationService.Permissions;

public static class AnonymizationServicePermissions
{
    public const string GroupName = "AnonymizationService";
    public const string PatientManagementPermission = GroupName + ".Patient.Manage";
    public const string ContainerImageManagementPermission = GroupName + ".ContainerImage.Manage";
    public const string PodDefinitionManagementPermission = GroupName + ".PodDefinition.Manage";
    public const string PlatformAdministrationPermission = GroupName + ".PlatformAdministration.Manage";
    public const string DefaultImportParameterManagementPermission = GroupName + ".DefaultImportParameter.Manage";
    public const string ExternalServiceRequestLimitManagementPermission = GroupName + ".ExternalServiceRequestLimit.Manage";
    public const string ScheduledIncrementalUploadParameterManagementPermission = GroupName + ".ScheduledIncrementalUploadParameter.Manage";
}
