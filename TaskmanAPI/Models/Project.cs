using System.ComponentModel.DataAnnotations;

namespace TaskmanAPI.Model;

public class Project
{
    [Key]
    public int Id { get; set; }
}