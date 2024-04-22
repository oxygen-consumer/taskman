using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models
{
    public class RolePerProject
    {
        public String UserId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public string RoleName { get; set; }

        [JsonIgnore]
        public virtual Project Project { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
