using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ProjectsService _projectsService;

    public ProjectsController(DefaultContext context)
    {
        _context = context;
        _privilegeChecker = new PrivilegeChecker(_context, User);
        _projectsService = new ProjectsService(_context, User);
    }

    // GET: api/Projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetUserProjects()
    {
        return Ok(await _projectsService.GetUserProjects());
    }

    // GET: api/Projects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        if (!_privilegeChecker.HasPrivilege(id, Role.User))
            return Forbid();

        return await _projectsService.GetProject(id) switch
        {
            { } project => Ok(project),
            _ => NotFound()
        };
    }


    // PUT: api/Projects/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Project>> Edit(int id, Project project)
    {
        // TODO: remove id from url and use only project.Id
        if (id != project.Id) return BadRequest();

        if (!_privilegeChecker.HasPrivilege(id, Role.Admin))
            return Forbid();

        try
        {
            return Ok(await _projectsService.EditProject(project));
        }
        catch (ArgumentException e)
        {
            return NotFound(e.Message);
        }
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<Project>> New(Project project)
    {
        var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // check if user exists
        // TODO: authorize controllers and remove shit like this
        if (!_context.Users.Any(u => u.Id == owner))
            return BadRequest("User does not exist");

        return Ok(await _projectsService.CreateProject(project, owner));
    }

    // DELETE: api/Projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        //only the owner can delete the project
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        try
        {
            await _projectsService.DeleteProject(id);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
    }

    // add another user to project: api/Projects/{id}/adduser/{user_id}
    [HttpPost("{id}/adduser/{user_id}")]
    public async Task<ActionResult<Project>> AddUser(int id, string user_id)
    {
        if (!_privilegeChecker.HasPrivilege(id, Role.Admin))
            return Forbid();

        try
        {
            return Ok(await _projectsService.AddUser(id, user_id));
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
    }

    // remove user from project: api/Projects/{id}/removeuser/{user_id}
    [HttpDelete("{id}/removeuser/{user_id}")]
    public async Task<ActionResult<Project>> RemoveUser(int id, string user_id)
    {
        if (!_privilegeChecker.HasPrivilege(id, Role.Admin))
            return Forbid();
        
        try
        {
            return Ok(await _projectsService.RemoveUser(id, user_id));
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
        catch (InvalidOperationException e)
        {
            return Forbid();
        }
    }

    // promote user to admin: api/Projects/{id}/promoteuser/{user_id}
    [HttpPost("{id}/promoteuser/{user_id}")]
    public async Task<ActionResult<Project>> PromoteUser(int id, string user_id)
    {
        // check if user has privilege to promote another user
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        try
        {
            return Ok(await _projectsService.PromoteUser(id, user_id));
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
    }

    // transfer ownership: api/Projects/{id}/transferownership/{user_id}
    [HttpPost("{id}/transferownership/{user_id}")]
    public async Task<ActionResult<Project>> TransferOwnership(int id, string user_id)
    {
        // check if user has privilege to transfer ownership
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        try
        {
            return Ok(await _projectsService.TransferOwnership(id, user_id));
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
    }

    // demote user to regular user: api/projects/{id}/demoteuser/{user_id}
    [HttpPost("{id}/demoteuser/{user_id}")]
    public async Task<ActionResult<Project>> DemoteUser(int id, string user_id)
    {
        // check if user has privilege to demote another user
        if (!_privilegeChecker.HasPrivilege(id, Role.Owner))
            return Forbid();

        try
        {
            return Ok(await _projectsService.DemoteUser(id, user_id));
        }
        catch (ArgumentException e)
        {
            return NotFound(e);
        }
    }
}