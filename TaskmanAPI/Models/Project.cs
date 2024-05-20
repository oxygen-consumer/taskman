using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public ICollection<RolePerProject>? RolePerProjects { get; set; }

    [JsonIgnore]
    public ICollection<ProjTask>? Task {  get; set; }
  
}