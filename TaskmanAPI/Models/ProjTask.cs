using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TaskStatus = TaskmanAPI.Enums.TaskStatus;

namespace TaskmanAPI.Models;

public class ProjTask
{
    [Key] public int Id { get; set; }

    public required int ProjectId { get; set; }

    public int ParentId { get; set; }

    [MaxLength(50)] public required string Title { get; set; }

    [MaxLength(200)] public required string Description { get; set; }

    public required DateTime Deadline { get; set; }

    public required TaskStatus Status { get; set; }

    // make it easier for frontend
    [NotMapped] public string StatusString => Status.ToString();

    //TO DO - comments
    [JsonIgnore] public virtual ICollection<UserTasks> UserTasks { get; set; } = new List<UserTasks>();

    [JsonIgnore] public virtual Project? Project { get; set; }
}