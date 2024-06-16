using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using TaskmanAPI.Models;

namespace TaskmanAPI.Model;

public class User : IdentityUser
{
    [JsonIgnore] public virtual ICollection<UserTasks> UserTasks { get; set; } = new List<UserTasks>();

    [JsonIgnore] public virtual ICollection<RolePerProject>? RolePerProjects { get; set; }
}