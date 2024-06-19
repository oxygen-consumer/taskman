using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskmanAPI.Models;

public class Project
{
    [Key] public int Id { get; set; }

    [MaxLength(50)] public required string Name { get; set; }

    [MaxLength(200)] public required string Description { get; set; }

    [JsonIgnore] public virtual ICollection<RolePerProject> RolePerProjects { get; set; } = new List<RolePerProject>();

    [JsonIgnore] public virtual ICollection<ProjTask> Tasks { get; set; } = new List<ProjTask>();
}