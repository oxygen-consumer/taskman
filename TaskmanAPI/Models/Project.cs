using System.ComponentModel.DataAnnotations;
using System.Data;

namespace TaskmanAPI.Models;

public class Project
{
    public int Id { get; set; }
    public int ProjectOwner { get; set; }
    public string Name { get; set; }
    public ICollection<RolePerProject> Comenzi { get; set; }
    public ICollection<Task> Task {  get; set; }

    public IEnumerable<Project> GetProjectsForUser(String userId)
    {
        var projectsForUser = new List<Project>();

        foreach (var rolePerProject in Comenzi)
        {
            // Verific�m dac� utilizatorul dat este �n lista de utilizatori a proiectului
            if (rolePerProject.UserId == userId)
            {
                // Dac� g�sim utilizatorul �n lista de utilizatori a proiectului, ad�ug�m proiectul la lista de proiecte pentru utilizatorul respectiv
                projectsForUser.Add(rolePerProject.Project);
            }
        }

        return projectsForUser;
    }
}