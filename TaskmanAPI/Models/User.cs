using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using TaskmanAPI.Models;

namespace TaskmanAPI.Model;

public class User : IdentityUser
{
    // [Key]
    // public String Id { get; set; }
    // [Required(ErrorMessage = "Numele de utilizator este obligatoriu")]
    // public string UserName { get; set; }
    // [Required(ErrorMessage = "Parola este obligatorie")]
    // public string Password { get; set; }
    // [Required(ErrorMessage = "Adresa de email este obligatorie")]
    // public string Email { get; set; }


    //to add - notification, comment, reaction (?)
    public virtual ICollection<UserTasks> UserTasks { get; set; } = new List<UserTasks>();

    [JsonIgnore] public virtual ICollection<RolePerProject>? RolePerProjects { get; set; }
}