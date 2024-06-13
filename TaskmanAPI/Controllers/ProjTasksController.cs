using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskmanAPI.Contexts;
using TaskmanAPI.Model;
using TaskmanAPI.Services;

namespace TaskmanAPI.Controllers;

[Route("api/Tasks")]
[ApiController]
[Authorize]
public class ProjTasksController(DefaultContext context, IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private readonly ProjTasksService _projTasksService = new(context, httpContextAccessor);
    private readonly UserService _userService = new(context);

    // GET: api/Tasks/get_user_tasks/{projectId}
    [HttpGet("get_user_tasks/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProjTask>>> GetUserTasks(int projectId)
    {
        return Ok(await _projTasksService.GetUserTasks(projectId));
    }

    // GET: api/Tasks/get_all_tasks/{projectId}
    [HttpGet("get_all_tasks/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProjTask>>> GetAllTasks(int projectId)
    {
        return Ok(await _projTasksService.GetAllTasks(projectId));
    }

    // GET: api/Tasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjTask>> GetProjTask(int id)
    {
        return Ok(await _projTasksService.GetTask(id));
    }

    // PUT: api/Tasks/5
    [HttpPut]
    public async Task<ActionResult<ProjTask>> PutProjTask(ProjTask projTask)
    {
        return Ok(await _projTasksService.EditTask(projTask));
    }

    // POST: api/Tasks/{projId}
    [HttpPost]
    public async Task<ActionResult<ProjTask>> PostProjTask(ProjTask projTask)
    {
        return Ok(await _projTasksService.CreateTask(projTask));
    }

    // DELETE: api/Tasks/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProjTask(int id)
    {
        await _projTasksService.DeleteTask(id);
        return NoContent();
    }

    // api/Tasks/{id}/assign_user/{userid}
    [HttpPost("{id}/assign_user/{username}")]
    public async Task<ActionResult<ProjTask>> AssignUsers(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projTasksService.AddUser(id, userId));
    }
    
    // api/Tasks/{id}/remove_user/{username}
    [HttpDelete("{id}/remove_user/{username}")]
    public async Task<ActionResult<ProjTask>> RemoveUsers(int id, string username)
    {
        var userId = await _userService.GetUserIdByUsername(username);
        return Ok(await _projTasksService.RemoveUser(id, userId));
    }
    
    // api/Tasks/{id}/change_status/{status}
    [HttpPut("{id}/change_status/{status}")]
    public async Task<ActionResult<ProjTask>> ChangeStatus(int id, string status)
    {
        return Ok(await _projTasksService.ChangeStatus(id, status));
    }
}