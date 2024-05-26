using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;
using TaskmanAPI.Models;
using TaskmanAPI.Services;

namespace TaskmanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : Controller
{
    private readonly DefaultContext _context;
    private readonly PrivilegeChecker _privilegeChecker;

    public ProjectsController(DefaultContext context)
    {
        _context = context;
        _privilegeChecker = new PrivilegeChecker(_context, User);
    }

    // GET: api/Projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var proles = _context.RolePerProjects.Where(rp => rp.UserId == userId).ToList();
        var projects = new List<Project>();
        var projectids = new List<int>();

        //project ids in which the user has roles
        foreach (var p in proles)
            if (p.ProjectId != null)
                if (!projects.Any(proj => proj.Id == p.ProjectId))
                    projectids.Add((int)p.ProjectId);

        foreach (var id in projectids)
        {
            var project = _context.Projects.Where(p => p.Id == id).First();
            projects.Add(project);
        }

        return projects;
    }

    // GET: api/Projects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound();

        //if user has role in project, then they're in it
        if (_privilegeChecker.HasPrivilege(id, Role.User))
            return project;

        return Forbid();
    }


    // PUT: api/Projects/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id) return BadRequest();

        //owner or admins can edit project
        if (!_privilegeChecker.HasPrivilege(id, Role.Admin))
            return Forbid();

        _context.Entry(project).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<Project>> New(Project project)
    {
        var ProjectOwner = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // check if user exists
        if (!_context.Users.Any(u => u.Id == ProjectOwner))
            return BadRequest("User does not exist");

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        //creating an owner role for the creator of the project
        var NewOwnerRole = new RolePerProject(ProjectOwner, project.Id, "Owner");
        _context.RolePerProjects.Add(NewOwnerRole);
        await _context.SaveChangesAsync();

        //ading new role to db
        project.RolePerProjects.Add(NewOwnerRole);

        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetProject", new { id = project.Id }, project);
    }

    // DELETE: api/Projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound();

        //only the owner can delete the project
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // add another user to project: api/Projects/{id}/adduser/{user_id}
    [HttpPost("{id}/adduser/{user_id}")]
    public async Task<ActionResult<Project>> AddUser(int id, string user_id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound("Project not found.");

        var user = await _context.Users.FindAsync(user_id);
        if (user == null) return BadRequest("User not found.");


        // check if user has privilege to add another user
        if (!_privilegeChecker.HasPrivilege(id, Role.Admin))
            return Forbid();

        var NewUserRole = new RolePerProject(user_id, id, "User");
        _context.RolePerProjects.Add(NewUserRole);
        await _context.SaveChangesAsync();

        project.RolePerProjects.Add(NewUserRole);
        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    // remove user from project: api/Projects/{id}/removeuser/{user_id}
    [HttpDelete("{id}/removeuser/{user_id}")]
    public async Task<ActionResult<Project>> RemoveUser(int id, string user_id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound("Project not found.");

        // check if user has privilege to remove another user
        // a user should be able to remove himself from a project
        if (_privilegeChecker.HasPrivilege(id, Role.Admin) && user_id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid();

        var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
        if (!userRole.Any())
            return NotFound();

        _context.RolePerProjects.Remove(userRole.First());
        await _context.SaveChangesAsync();
        return project;
    }

    // promote user to admin: api/Projects/{id}/promoteuser/{user_id}
    [HttpPost("{id}/promoteuser/{user_id}")]
    public async Task<ActionResult<Project>> PromoteUser(int id, string user_id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound("Project not found.");

        // check if user has privilege to promote another user
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
        if (!userRole.Any())
            return NotFound();

        userRole.First().RoleName = "Admin";
        _context.Entry(userRole.First()).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    // transfer ownership: api/Projects/{id}/transferownership/{user_id}
    [HttpPost("{id}/transferownership/{user_id}")]
    public async Task<ActionResult<Project>> TransferOwnership(int id, string user_id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound("Project not found.");

        // check if user has privilege to transfer ownership
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        var newOwnerRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
        if (!newOwnerRole.Any())
            return NotFound();

        var oldOwnerRole = _context.RolePerProjects.Where(rp =>
            rp.ProjectId == id && rp.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!oldOwnerRole.Any())
            return NotFound();

        oldOwnerRole.First().RoleName = "Admin";
        newOwnerRole.First().RoleName = "Owner";
        _context.Entry(oldOwnerRole.First()).State = EntityState.Modified;
        _context.Entry(newOwnerRole.First()).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    // demote user to regular user: api/projects/{id}/demoteuser/{user_id}
    [HttpPost("{id}/demoteuser/{user_id}")]
    public async Task<ActionResult<Project>> DemoteUser(int id, string user_id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound("Project not found.");

        // check if user has privilege to demote another user
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        var userRole = _context.RolePerProjects.Where(rp => rp.ProjectId == id && rp.UserId == user_id);
        if (!userRole.Any())
            return NotFound();

        userRole.First().RoleName = "User";
        _context.Entry(userRole.First()).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}