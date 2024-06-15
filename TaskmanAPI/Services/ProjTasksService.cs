using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Exceptions;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Services;

// FIXME: this is not final and some stuff is a bit off
public class ProjTasksService
{
    private readonly DefaultContext _context;
    private readonly PrivilegeChecker _privilegeChecker;
    private readonly ClaimsPrincipal _user;

    public ProjTasksService(DefaultContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _user = httpContextAccessor.HttpContext!.User;
        _privilegeChecker = new PrivilegeChecker(_context, _user);
    }

    public async Task<IEnumerable<ProjTask>> GetUserTasks(int projectId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");

        var userid = _user.FindFirstValue(ClaimTypes.NameIdentifier);
        // select only tasks where user is assigned and parent id is null to avoid returning subtasks
        var tasks = await _context.UserTasks
            .Where(ut => ut.UserId == userid)
            .Select(ut => ut.Task)
            .Where(t => t!.ProjectId == projectId && t.ParentId == default)
            .ToListAsync();

        return tasks!;
    }

    public async Task<IEnumerable<ProjTask>> GetAllTasks(int projectId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");

        // return only tasks where parent id is null to avoid returning subtasks
        return await _context.ProjTasks.Where(t => t.ProjectId == projectId && t.ParentId == default).ToListAsync();
    }

    public async Task<ProjTask> GetTask(int id)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Task does not exist");

        return task;
    }

    public async Task<ProjTask> CreateTask(ProjTask task)
    {
        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        // if parent id is set, call the get method on parent task to avoid privilege exceptions
        if (task.ParentId != default)
            try
            {
                var parentTask = await GetTask(task.ParentId);
                if (parentTask.ProjectId != task.ProjectId)
                    throw new EntityNotFoundException("Parent task does not exist");
            }
            catch (EntityNotFoundException)
            {
                throw new EntityNotFoundException("Parent task does not exist");
            }

        _context.ProjTasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<ProjTask> EditTask(ProjTask task)
    {
        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        var existingTask = await _context.ProjTasks.FindAsync(task.Id);
        if (existingTask == null)
            throw new EntityNotFoundException("Task does not exist");
        if (existingTask.ProjectId != task.ProjectId)
            throw new EntityNotFoundException("Task does not exist");

        // sanity checks: can't change parent id or project id to avoid privilege escalation
        task.ParentId = existingTask.ParentId;
        task.ProjectId = existingTask.ProjectId;

        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteTask(int id)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        _context.ProjTasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<ProjTask> AddUser(int id, string userId)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new EntityNotFoundException("User does not exist");

        if (await _context.UserTasks.AnyAsync(ut => ut.TaskId == id && ut.UserId == userId))
            throw new EntityAlreadyExistsException("User already assigned to task");

        var userTask = new UserTasks { TaskId = id, UserId = userId };
        _context.UserTasks.Add(userTask);

        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<ProjTask> RemoveUser(int id, string userId)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new EntityNotFoundException("User does not exist");

        var userTask = await _context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == id && ut.UserId == userId);
        if (userTask == null)
            throw new EntityNotFoundException("User not assigned to task");

        _context.UserTasks.Remove(userTask);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<ProjTask> ChangeStatus(int id, string status)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        if (!Enum.TryParse<TaskStatus>(status, out var taskStatus))
            throw new EntityNotFoundException("Invalid status");

        task.Status = taskStatus;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<ProjTask>> GetSubtasks(int id)
    {
        var task = await _context.ProjTasks.FindAsync(id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        return await _context.ProjTasks.Where(t => t.ParentId == id).ToListAsync();
    }
}