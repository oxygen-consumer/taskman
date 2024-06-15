using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TaskmanAPI.Models;
using TaskStatus = TaskmanAPI.Enums.TaskStatus;

namespace TaskmanAPI.Model;

public class ProjTask
{
    [Key] public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ParentId { get; set; }

    public string Title { get; set; }

    //public string[] Labels { get; set; } 

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    public TaskStatus Status { get; set; }

    // make it easier for frontend
    [NotMapped] public string StatusString => Status.ToString();

    //TO DO - comments
    [JsonIgnore] public virtual ICollection<UserTasks> UserTasks { get; set; } = new List<UserTasks>();

    //subtasks
    //public virtual ICollection<ProjTask> ProjTasks { get; set; }
    [JsonIgnore] public virtual Project? Project { get; set; }
}