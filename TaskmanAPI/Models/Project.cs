using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskmanAPI.Models;

namespace TaskmanAPI.Model;

public class Project
{
    // PLACEHOLDER
    [Key]
    public int Id { get; set; }

    public virtual ICollection<RolePerProject>? RolePerProjects { get; set; }
}