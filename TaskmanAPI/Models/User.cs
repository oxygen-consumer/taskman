using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskmanAPI.Models;

namespace TaskmanAPI.Model
{
    public class User
    {
        [Key]
        public String Id { get; set; }
        [Required(ErrorMessage = "Numele de utilizator este obligatoriu")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Parola este obligatorie")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Adresa de email este obligatorie")]
        public string Email { get; set; }


        //to add - notification, comment, reaction (?)
        public virtual ICollection<ProjTask>? Tasks { get; set; }
        public virtual ICollection<RolePerProject>? RolePerProjects { get; set; }
    }
}