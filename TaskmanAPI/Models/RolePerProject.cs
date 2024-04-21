using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models
{
    public class RolePerProject
    {
        public String UserId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public string RoleName { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}

