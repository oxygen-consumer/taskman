using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models;

public class RolePerProject
{
    [MaxLength(36)] public required string UserId { get; set; }

    public required int ProjectId { get; set; }

    [MaxLength(5)] public required string RoleName { get; set; }

    [JsonIgnore] public virtual Project? Project { get; set; }

    [JsonIgnore] public virtual User? User { get; set; }
}