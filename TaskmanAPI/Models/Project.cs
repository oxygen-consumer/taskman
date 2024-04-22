using System.ComponentModel.DataAnnotations;

namespace TaskmanAPI.Models;

public class Project
{
    public int Id { get; set; }
    public int ProjectOwner { get; set; }
    public string Name { get; set; }
    public ICollection<RolePerProject> Comenzi { get; set; }
    public ICollection<Task> Task {  get; set; }
}