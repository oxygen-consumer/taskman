using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskmanAPI.Models;

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

    //TO DO - status changes, comments
    [JsonIgnore] public virtual ICollection<User> Users { get; set; } = new List<User>();

    //subtasks
    //public virtual ICollection<ProjTask> ProjTasks { get; set; }
    [JsonIgnore] public virtual Project? Project { get; set; }
}