using System.Security.Claims;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;

namespace TaskmanAPI.Services;

public class PrivilegeChecker(DefaultContext context, ClaimsPrincipal user)
{
    public bool HasAccessToProject(int projectId)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return context.RolePerProjects
            .Any(rp => rp.ProjectId == projectId && rp.UserId == userId);
    }

    public bool HasPrivilege(int projectId, Role role)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        var userRole = context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .Select(rp => rp.RoleName)
            .FirstOrDefault();

        if (userRole == null) return false;

        var userRoleEnum = Enum.Parse<Role>(userRole);

        return userRoleEnum >= role;
    }
}