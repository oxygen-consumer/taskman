using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;
using TaskmanAPI.Models;

namespace TaskmanAPI.Services;

public class ProjectsService(DefaultContext context, ClaimsPrincipal user)
{
    public async Task<IEnumerable<Project>> GetUserProjects()
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var projectIds = await context.RolePerProjects
            .Where(rp => rp.UserId == userId && rp.ProjectId != null)
            .Select(rp => rp.ProjectId)
            .Distinct()
            .ToListAsync();

        var projects = await context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToListAsync();

        return projects;
    }

    public async Task<Project?> GetProject(int id)
    {
        return await context.Projects.FindAsync(id);
    }

    public async Task<Project> CreateProject(Project project, string? ownerId)
    {
        context.Projects.Add(project);

        var owner = await context.Users.FindAsync(ownerId);

        var rolePerProject = new RolePerProject
        {
            Project = project,
            User = owner,
            RoleName = Role.Owner.ToString()
        };
        context.RolePerProjects.Add(rolePerProject);

        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> EditProject(Project project)
    {
        if (!context.Projects.Any(p => p.Id == project.Id))
            throw new ArgumentException("Project does not exist");

        context.Entry(project).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteProject(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }

    public async Task<Project> AddUser(int projectId, string userId)
    {
        var project = await context.Projects.FindAsync(projectId);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        var userToAdd = await context.Users.FindAsync(userId);
        if (userToAdd == null)
            throw new ArgumentException("User does not exist");

        var rolePerProject = new RolePerProject
        {
            ProjectId = projectId,
            User = userToAdd,
            RoleName = Role.User.ToString()
        };
        context.RolePerProjects.Add(rolePerProject);

        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> RemoveUser(int projectId, string userId)
    {
        var project = await context.Projects.FindAsync(projectId);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        var userRole = await context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new ArgumentException("User is not part of the project");
        
        if (userRole.RoleName != Role.User.ToString())
            // TODO: implement custom exceptions
            throw new InvalidOperationException("Admins and owners can't be removed");

        context.RolePerProjects.Remove(userRole);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> PromoteUser(int projectId, string userId)
    {
        var project = await context.Projects.FindAsync(projectId);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        var userRole = await context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new ArgumentException("User is not part of the project");

        userRole.RoleName = Role.Admin.ToString();
        context.Entry(userRole).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> DemoteUser(int projectId, string userId)
    {
        var project = await context.Projects.FindAsync(projectId);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        var userRole = await context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new ArgumentException("User is not part of the project");

        userRole.RoleName = Role.User.ToString();
        context.Entry(userRole).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> TransferOwnership(int projectId, string userId)
    {
        var project = await context.Projects.FindAsync(projectId);
        if (project == null)
            throw new ArgumentException("Project does not exist");

        var currentOwnerRole = await context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == user.FindFirstValue(ClaimTypes.NameIdentifier))
            .FirstOrDefaultAsync();
        if (currentOwnerRole == null)
            throw new ArgumentException("Current user is not part of the project");

        var userRole = await context.RolePerProjects
            .Where(rp => rp.ProjectId == projectId && rp.UserId == userId)
            .FirstOrDefaultAsync();
        if (userRole == null)
            throw new ArgumentException("User is not part of the project");

        // promote new owner
        userRole.RoleName = Role.Owner.ToString();
        context.Entry(userRole).State = EntityState.Modified;

        // demote current owner
        currentOwnerRole.RoleName = Role.Admin.ToString();
        context.Entry(currentOwnerRole).State = EntityState.Modified;

        await context.SaveChangesAsync();
        return project;
    }
}