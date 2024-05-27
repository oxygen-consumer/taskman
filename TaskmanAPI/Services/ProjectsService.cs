using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;
using TaskmanAPI.Exceptions;
using TaskmanAPI.Models;

namespace TaskmanAPI.Services;

public class ProjectsService
{
    private readonly DefaultContext _context;
    private readonly PrivilegeChecker _privilegeChecker;
    private readonly ClaimsPrincipal _user;

    public ProjectsService(DefaultContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _user = httpContextAccessor.HttpContext!.User;
        _privilegeChecker = new PrivilegeChecker(_context, _user);
    }

    private bool HasAccess(int projectId)
    {
        return _privilegeChecker.HasPrivilege(projectId, Role.User);
    }

    public async Task<IEnumerable<Project>> GetUserProjects()
    {
        var userId = _user.FindFirstValue(ClaimTypes.NameIdentifier);
        var projectIds = await _context.RolePerProjects
            .Where(rp => rp.UserId == userId && rp.ProjectId != null)
            .Select(rp => rp.ProjectId)
            .Distinct()
            .ToListAsync();

        var projects = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync();

        return projects;
    }

    public async Task<Project> GetProject(int id)
    {
        // we will throw a NotFound exception even if the project exists but the user does not have access
        if (!HasAccess(id))
            throw new EntityNotFoundException("Project does not exist");
        return (await _context.Projects.FindAsync(id))!;
    }

    public async Task<Project> CreateProject(Project project)
    {
        _context.Projects.Add(project);

        var owner = await _context.Users.FindAsync(_user.FindFirstValue(ClaimTypes.NameIdentifier));

        var rolePerProject = new RolePerProject
        {
            Project = project,
            User = owner,
            RoleName = Role.Owner.ToString()
        };
        _context.RolePerProjects.Add(rolePerProject);

        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> EditProject(Project project)
    {
        if (!HasAccess(project.Id))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(project.Id, Role.Admin))
            throw new InsufficientMemoryException("You do not have the required privileges to edit this project");

        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteProject(int id)
    {
        if (!HasAccess(id))
            throw new EntityNotFoundException("Project does not exist");
        var project = await _context.Projects.FindAsync(id);

        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            throw new InsufficientPrivilegesException("You do not have the required privileges to delete this project");

        // remove all roles associated with the project
        var roles = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == id)
            .ToListAsync();
        _context.RolePerProjects.RemoveRange(roles);

        _context.Projects.Remove(project!);
        await _context.SaveChangesAsync();
    }

    public async Task<Project> AddUser(int projectId, string userId)
    {
        if (!HasAccess(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Admin))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to add a user to this project");

        var project = await _context.Projects.FindAsync(projectId);

        var userToAdd = await _context.Users.FindAsync(userId);
        if (userToAdd == null)
            throw new EntityNotFoundException("User does not exist");

        var rolePerProject = new RolePerProject
        {
            ProjectId = projectId,
            User = userToAdd,
            RoleName = Role.User.ToString()
        };
        _context.RolePerProjects.Add(rolePerProject);

        await _context.SaveChangesAsync();
        return project!;
    }

    public async Task<Project?> RemoveUser(int projectId, string userId)
    {
        if (!HasAccess(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Admin))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to remove a user from this project");

        var project = await _context.Projects.FindAsync(projectId);

        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        // users should not be able to remove other users with a higher or equal role, except if they try to remove themselves
        if (userRole.UserId != _user.FindFirstValue(ClaimTypes.NameIdentifier))
        {
            var currentUserRole = await _context.RolePerProjects
                .Where(rp => rp.ProjectId == projectId && rp.UserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
                .FirstOrDefaultAsync();

            var userRoleEnum = Enum.Parse<Role>(userRole.RoleName);
            var currentUserRoleEnum = Enum.Parse<Role>(currentUserRole!.RoleName);
            if (userRoleEnum >= currentUserRoleEnum)
                throw new InsufficientPrivilegesException("You cannot remove a user with a higher or equal role");
        }

        // if the user is the owner of the project, give the role to the first admin
        if (userRole.RoleName == Role.Owner.ToString())
        {
            var newOwner = await _context.RolePerProjects
                .Where(rp => rp.ProjectId == projectId && rp.RoleName == Role.Admin.ToString())
                .FirstOrDefaultAsync();

            // if there are no admins, give the role to the first user
            if (newOwner == null)
                newOwner = await _context.RolePerProjects
                    .Where(rp => rp.ProjectId == projectId && rp.RoleName == Role.User.ToString())
                    .FirstOrDefaultAsync();

            // if there are no users, delete the project
            if (newOwner == null)
            {
                await DeleteProject(projectId);
                return default;
            }

            newOwner.RoleName = Role.Owner.ToString();
            _context.Entry(newOwner).State = EntityState.Modified;
        }

        _context.RolePerProjects.Remove(userRole);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> PromoteUser(int projectId, string userId)
    {
        if (!HasAccess(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to promote a user in this project");

        var project = await _context.Projects.FindAsync(projectId);

        // check if user is part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        userRole.RoleName = Role.Admin.ToString();
        _context.Entry(userRole).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project!;
    }

    public async Task<Project> DemoteUser(int projectId, string userId)
    {
        if (!HasAccess(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to demote a user in this project");

        var project = await _context.Projects.FindAsync(projectId);

        // check if user is part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        userRole.RoleName = Role.User.ToString();
        _context.Entry(userRole).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project!;
    }

    public async Task<Project> TransferOwnership(int projectId, string userId)
    {
        if (!HasAccess(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to transfer ownership of this project");

        var project = await _context.Projects.FindAsync(projectId);

        var currentOwnerRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
            .FirstOrDefaultAsync();

        // check if user is part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        // promote new owner
        userRole.RoleName = Role.Owner.ToString();
        _context.Entry(userRole).State = EntityState.Modified;

        // demote old owner
        currentOwnerRole!.RoleName = Role.Admin.ToString();
        _context.Entry(currentOwnerRole).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return project!;
    }
}