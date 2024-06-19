using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
        if (!_privilegeChecker.HasAccessToProject(id))
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
            UserId = owner.Id,
            RoleName = Role.Owner.ToString()
        };
        _context.RolePerProjects.Add(rolePerProject);

        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> EditProject(Project project)
    {
        if (!_privilegeChecker.HasAccessToProject(project.Id))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(project.Id, Role.Admin))
            throw new InsufficientMemoryException("You do not have the required privileges to edit this project");

        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteProject(int id)
    {
        if (!_privilegeChecker.HasAccessToProject(id))
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

    public async Task AddUser(int projectId, string userId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Admin))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to add a user to this project");

        var userToAdd = await _context.Users.FindAsync(userId);
        if (userToAdd == null)
            throw new EntityNotFoundException("User does not exist");

        // check if user is already part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole != null)
            throw new EntityAlreadyExistsException("User is already part of the project");

        var rolePerProject = new RolePerProject
        {
            ProjectId = projectId,
            UserId = userId,
            RoleName = Role.User.ToString()
        };
        _context.RolePerProjects.Add(rolePerProject);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveUser(int projectId, string userId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Admin))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to remove a user from this project");

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
                return;
            }

            newOwner.RoleName = Role.Owner.ToString();
            _context.Entry(newOwner).State = EntityState.Modified;
        }

        _context.RolePerProjects.Remove(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task PromoteUser(int projectId, string userId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to promote a user in this project");

        // check if user is part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        userRole.RoleName = Role.Admin.ToString();
        _context.Entry(userRole).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DemoteUser(int projectId, string userId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to demote a user in this project");

        // check if user is part of the project
        var userRole = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new EntityNotFoundException("User is not part of the project");

        userRole.RoleName = Role.User.ToString();
        _context.Entry(userRole).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task TransferOwnership(int projectId, string userId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");
        if (!_privilegeChecker.HasPrivilege(projectId, Role.Owner))
            throw new InsufficientPrivilegesException(
                "You do not have the required privileges to transfer ownership of this project");

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
    }

    public async Task<IEnumerable<object>> GetProjectUsers(int projectId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");

        var users = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId)
            .Select(rp => new
            {
                rp.User!.UserName,
                rp.RoleName
            })
            .ToListAsync();

        return users;
    }

    public async Task<string> GetMyRole(int projectId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");

        var role = await _context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
            .Select(rp => rp.RoleName)
            .FirstOrDefaultAsync();

        return role!;
    }
    
}