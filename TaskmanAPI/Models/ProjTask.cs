namespace TaskmanAPI.Model;
using System.ComponentModel.DataAnnotations;

public class ProjTask
{
    [Key]
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ParentId { get; set; }

    public string Title { get; set; }

    //public string[] Labels { get; set; } 

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    //TO DO - status changes, comments

    public virtual ICollection<User> Users { get; set; }

    //subtaskuri
    //public virtual ICollection<ProjTask> ProjTasks { get; set; }

    public virtual Project Project { get; set; }
}