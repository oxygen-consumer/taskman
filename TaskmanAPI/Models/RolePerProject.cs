using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskmanAPI.Model;

namespace TaskmanAPI.Models
{
    public class RolePerProject
    {
        public RolePerProject() { }
        public RolePerProject(string? userid, int projid, string v)
        {
            UserId = userid;
            ProjectId = projid;
            RoleName = v;
        }

        public String? UserId { get; set; }

        public int? ProjectId { get; set; }

        [Required]
        public string RoleName { get; set; }

        [JsonIgnore]
        public virtual Project? Project { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
