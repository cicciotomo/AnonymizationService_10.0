using AnonymizationService.Permissions;
using AnonymizationService.Roles;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace AnonymizationService.Data
{
    public class AnonymizationServiceRolesDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IPermissionManager _permissionManager;

        public AnonymizationServiceRolesDataSeederContributor(IdentityRoleManager identityRoleManager, IGuidGenerator guidGenerator, IIdentityRoleRepository identityRoleRepository, ILookupNormalizer lookupNormalizer, IPermissionManager permissionManager)
        {
            _identityRoleManager = identityRoleManager;
            _guidGenerator = guidGenerator;
            _identityRoleRepository = identityRoleRepository;
            _lookupNormalizer = lookupNormalizer;
            _permissionManager = permissionManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            List<RoleWithPermissionsToSeed> rolesToSeed = new List<RoleWithPermissionsToSeed>()
            {
                new(AnonymizationServiceRoles.PatientManager, new List<string>
                {
                    AnonymizationServicePermissions.PatientManagementPermission
                }),
                new(AnonymizationServiceRoles.Admin, new List<string>
                {
                    AnonymizationServicePermissions.ContainerImageManagementPermission,
                    AnonymizationServicePermissions.PlatformAdministrationPermission,
                    AnonymizationServicePermissions.PodDefinitionManagementPermission
                })
            };

            foreach (var roleToSeed in rolesToSeed)
            {
                var dbRole = await _identityRoleRepository.FindByNormalizedNameAsync(
                    _lookupNormalizer.NormalizeName(roleToSeed.RoleName));

                if (dbRole is null)
                {
                    dbRole = new Volo.Abp.Identity.IdentityRole(
                        _guidGenerator.Create()
                        , roleToSeed.RoleName)
                    {
                        IsPublic = true,
                        IsStatic = true
                    };

                    (await _identityRoleManager.CreateAsync(dbRole)).CheckErrors();
                }

                foreach (var rolePermission in roleToSeed.PermissionsToSet)
                {
                    await _permissionManager.SetForRoleAsync(roleToSeed.RoleName, rolePermission, true);
                }
            }
        }
    }
}
