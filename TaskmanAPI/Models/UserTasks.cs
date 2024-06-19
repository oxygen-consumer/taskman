using System.ComponentModel.DataAnnotations;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models;

public class UserTasks
{
    public UserTasks()
    {
    }

    public UserTasks(string userId, int taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }

    [MaxLength(36)] public required string UserId { get; set; }
    public required int TaskId { get; set; }

    public virtual User? User { get; set; }
    public virtual ProjTask? Task { get; set; }
}