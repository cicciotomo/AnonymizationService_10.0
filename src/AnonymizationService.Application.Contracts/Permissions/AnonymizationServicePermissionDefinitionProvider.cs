using AnonymizationService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AnonymizationService.Permissions;

public class AnonymizationServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(
            AnonymizationServicePermissions.GroupName
            , L("Permissions:AnonymizationServiceGroupName"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.PatientManagementPermission
            , L("Permissions:PatientManagementPermission"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.ContainerImageManagementPermission
            , L("Permissions:ContainerImageManagementPermission"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.PodDefinitionManagementPermission
            , L("Permissions:PodDefinitionManagementPermission"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.PlatformAdministrationPermission
            , L("Permissions:PlatformAdministrationPermission"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.DefaultImportParameterManagementPermission
            , L("Permissions:DefaultImportParameterManagementPermission"));

        myGroup.AddPermission(
            AnonymizationServicePermissions.ExternalServiceRequestLimitManagementPermission
            , L("Permissions:ExternalServiceRequestLimitManagementPermission"));

        myGroup.AddPermission(
          AnonymizationServicePermissions.ScheduledIncrementalUploadParameterManagementPermission
          , L("Permissions:ScheduledIncrementalUploadParameterManagementPermission"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AnonymizationServiceApplicationContractsResource>(name);
    }
}
