﻿using Microsoft.AspNetCore.Mvc;
using TaskmanAPI.Contexts;
using TaskmanAPI.Model;
using TaskmanAPI.Services;

namespace TaskmanAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjTasksController(DefaultContext context, IHttpContextAccessor httpContextAccessor)
    : ControllerBase
{
    private readonly ProjTasksService _projTasksService = new(context, httpContextAccessor);

    // GET: api/ProjTasks/get_user_tasks/{projectId}
    [HttpGet("get_user_tasks/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProjTask>>> GetUserTasks(int projectId)
    {
        return Ok(await _projTasksService.GetUserTasks(projectId));
    }

    // GET: api/ProjTasks/get_all_tasks/{projectId}
    [HttpGet("get_all_tasks/{projectId}")]
    public async Task<ActionResult<IEnumerable<ProjTask>>> GetAllTasks(int projectId)
    {
        return Ok(await _projTasksService.GetAllTasks(projectId));
    }

    // GET: api/ProjTasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjTask>> GetProjTask(int id)
    {
        return Ok(await _projTasksService.GetTask(id));
    }

    // PUT: api/ProjTasks/5
    [HttpPut]
    public async Task<ActionResult<ProjTask>> PutProjTask(ProjTask projTask)
    {
        return Ok(await _projTasksService.EditTask(projTask));
    }

    // POST: api/ProjTasks/{projId}
    [HttpPost]
    public async Task<ActionResult<ProjTask>> PostProjTask(ProjTask projTask)
    {
        return Ok(await _projTasksService.CreateTask(projTask));
    }

    // DELETE: api/ProjTasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjTask(int id)
    {
        await _projTasksService.DeleteTask(id);
        return Ok();
    }

    // api/ProjTasks/{id}/assign_user/{userid}
    [HttpPost("{id}/assign_user/{userId}")]
    public async Task<ActionResult<ProjTask>> AssignUsers(int id, string userId)
    {
        return Ok(await _projTasksService.AddUser(id, userId));
    }
}