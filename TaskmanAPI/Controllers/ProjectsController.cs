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

    // add another user to project: api/Projects/{id}/add_user/{user_id}
    [HttpPost("{id}/add_user/{username}")]
    public async Task<ActionResult> AddUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        await _projectsService.AddUser(id, userId);
        return Ok();
    }

    // remove user from project: api/Projects/{id}/remove_user/{user_id}
    [HttpDelete("{id}/remove_user/{username}")]
    public async Task<ActionResult> RemoveUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        await _projectsService.RemoveUser(id, userId);
        return Ok();
    }

    // promote user to admin: api/Projects/{id}/promote_user/{user_id}
    [HttpPost("{id}/promote_user/{username}")]
    public async Task<ActionResult> PromoteUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        await _projectsService.PromoteUser(id, userId);
        return Ok();
    }

    // transfer ownership: api/Projects/{id}/transfer_ownership/{user_id}
    [HttpPost("{id}/transfer_ownership/{username}")]
    public async Task<ActionResult> TransferOwnership(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        await _projectsService.TransferOwnership(id, userId);
        return Ok();
    }

    // demote user to regular user: api/projects/{id}/demote_user/{user_id}
    [HttpPost("{id}/demote_user/{username}")]
    public async Task<ActionResult> DemoteUser(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        await _projectsService.DemoteUser(id, userId);
        return Ok();
    }

    // get project users: api/projects/{id}/users
    [HttpGet("{id}/users")]
    public async Task<ActionResult<IEnumerable<object>>> GetProjectUsers(int id)
    {
        return Ok(await _projectsService.GetProjectUsers(id));
    }
}