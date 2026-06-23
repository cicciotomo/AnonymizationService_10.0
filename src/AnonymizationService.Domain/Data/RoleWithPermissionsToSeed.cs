using System.Collections.Generic;

namespace AnonymizationService.Data
{
    public record RoleWithPermissionsToSeed(string RoleName, List<string> PermissionsToSet);
}
