using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskmanAPI.Contexts;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Controllers;

// placeholder to test authorization
[Route("api/projects")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly DefaultContext _context;

    public ProjectController(DefaultContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public string Get()
    {
        return "Hello from ProjectController!";
    }

    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project project)
    {
        // get the user id from the token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // create a new RolePerProject entry for the owner
        var rolePerProject = new RolePerProject
        {
            UserId = userId,
            ProjectId = project.Id,
            RoleName = "Owner"
        };

        project.RolePerProjects = new List<RolePerProject> { rolePerProject };

        // add the project and the RolePerProject entry to the database
        _context.Projects.Add(project);
        _context.RolePerProjects.Add(rolePerProject);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProject", new { id = project.Id }, project);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null) return NotFound();

        return project;
    }
}