using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Contexts;

// RENAME THIS IF USING MORE CONTEXTS
public class DefaultContext : DbContext
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjTask> ProjTasks { get; set; }
    public DbSet<RolePerProject> RolePerProjects { get; set; }
    public DbSet<Notification> Notifications{ get; set; }

    protected override void OnModelCreating(ModelBuilder
       modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RolePerProject>().HasKey(ab => new { ab.UserId, ab.ProjectId });
        
        //relatie one-to-many ( User-Notification )
        modelBuilder.Entity<Notification>()
           .HasOne(t => t.User)
           .WithMany(u => u.Notifications)
           .HasForeignKey(t => t.UserId)
           .OnDelete(DeleteBehavior.Restrict); 

        //relatia one-to-many (PRoject-RolePerProject)
        modelBuilder.Entity<RolePerProject>()
            .HasOne(t => t.Project)
           .WithMany(u => u.Comenzi) // vectorul de RolePerProject din Project
           .HasForeignKey(t => t.ProjectId)
           .OnDelete(DeleteBehavior.Restrict);


        //modelBuilder.Entity<RolePerProject>().HasOne(ab => ab.User).WithMany(ab => ab.RolePerProjects)
    }
}