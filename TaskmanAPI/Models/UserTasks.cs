using TaskmanAPI.Model;

namespace TaskmanAPI.Models;

public class UserTasks
{
    public UserTasks()
    {
    }

    public UserTasks(string? userId, int taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }

    public string? UserId { get; set; }
    public int TaskId { get; set; }

    public virtual User? User { get; set; }
    public virtual ProjTask? Task { get; set; }
}