using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskmanAPI.Contexts;
using TaskmanAPI.Models;
using TaskmanAPI.Services;

namespace TaskmanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController(DefaultContext context, IHttpContextAccessor httpContextAccessor)
    : Controller
{
    private readonly ProjectsService _projectsService = new(context, httpContextAccessor);
    private readonly UserService _userService = new(context);

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
        return Ok(await _projectsService.GetProject(id));
    }


    // PUT: api/Projects/5
    [HttpPut("{id}")]
    public async Task<ActionResult<Project>> Edit(int id, Project project)
    {
        // TODO: remove id from url and use only project.Id
        if (id != project.Id) return BadRequest();

        return Ok(await _projectsService.EditProject(project));
    }

    // POST: api/Projects
    [HttpPost]
    public async Task<ActionResult<Project>> New(Project project)
    {
        return Ok(await _projectsService.CreateProject(project));
    }

    // DELETE: api/Projects/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _projectsService.DeleteProject(id);
        return NoContent();
    }

    // TODO: methods like these should not return the project
    // add another user to project: api/Projects/{id}/adduser/{user_id}
    [HttpPost("{id}/add_user/{username}")]
    public async Task<ActionResult<Project>> AddUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projectsService.AddUser(id, userId));
    }

    // remove user from project: api/Projects/{id}/removeuser/{user_id}
    [HttpDelete("{id}/remove_user/{username}")]
    public async Task<ActionResult<Project>> RemoveUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projectsService.RemoveUser(id, userId));
    }

    // promote user to admin: api/Projects/{id}/promoteuser/{user_id}
    [HttpPost("{id}/promote_user/{username}")]
    public async Task<ActionResult<Project>> PromoteUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projectsService.PromoteUser(id, userId));
    }

    // transfer ownership: api/Projects/{id}/transferownership/{user_id}
    [HttpPost("{id}/transfer_ownership/{username}")]
    public async Task<ActionResult<Project>> TransferOwnership(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projectsService.TransferOwnership(id, userId));
    }

    // demote user to regular user: api/projects/{id}/demoteuser/{user_id}
    [HttpPost("{id}/demote_user/{username}")]
    public async Task<ActionResult<Project>> DemoteUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projectsService.DemoteUser(id, userId));
    }
}