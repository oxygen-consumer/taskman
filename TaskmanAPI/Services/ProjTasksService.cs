using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Exceptions;
using TaskmanAPI.Model;

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
        var tasks = await _context.ProjTasks.Where(t => t.ProjectId == projectId).Include(projTask => projTask.Users)
            .ToListAsync();

        return tasks.Where(t => t.Users.Any(u => u.Id == userid));
    }

    public async Task<IEnumerable<ProjTask>> GetAllTasks(int projectId)
    {
        if (!_privilegeChecker.HasAccessToProject(projectId))
            throw new EntityNotFoundException("Project does not exist");

        return await _context.ProjTasks.Where(t => t.ProjectId == projectId).Include(projTask => projTask.Users)
            .ToListAsync();
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
        var task = await _context.ProjTasks.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new EntityNotFoundException("User does not exist");

        if (task.Users.Any(u => u.Id == userId))
            throw new EntityAlreadyExistsException("User already assigned to task");

        task.Users.Add(user);
        await _context.SaveChangesAsync();
        return task;
    }
    
    public async Task<ProjTask> RemoveUser(int id, string userId)
    {
        var task = await _context.ProjTasks.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null)
            throw new EntityNotFoundException("Task does not exist");

        if (!_privilegeChecker.HasAccessToProject(task.ProjectId))
            throw new EntityNotFoundException("Project does not exist");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new EntityNotFoundException("User does not exist");

        if (task.Users.All(u => u.Id != userId))
            throw new EntityNotFoundException("User not assigned to task");

        task.Users.Remove(user);
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
}