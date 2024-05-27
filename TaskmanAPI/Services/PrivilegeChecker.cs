using System.Security.Claims;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;

namespace TaskmanAPI.Services;

public class PrivilegeChecker
{
    private readonly DefaultContext _context;
    private readonly ClaimsPrincipal _user;

    public PrivilegeChecker(DefaultContext context, ClaimsPrincipal user)
    {
        _context = context;
        _user = user;
    }

    public bool HasPrivilege(int projectId, Role role)
    {
        var userId = _user.FindFirstValue(ClaimTypes.NameIdentifier);

        var userRole = _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .Select(rp => rp.RoleName)
            .FirstOrDefault();

        if (userRole == null) return false;

        var userRoleEnum = Enum.Parse<Role>(userRole);

        return userRoleEnum >= role;
    }
}