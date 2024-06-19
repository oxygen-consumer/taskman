using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Contexts;

// RENAME THIS IF USING MORE CONTEXTS
public class DefaultContext : IdentityDbContext<User>
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjTask> ProjTasks { get; set; }
    public DbSet<RolePerProject> RolePerProjects { get; set; }



    protected override void OnModelCreating(ModelBuilder
        modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RolePerProject>()
            .HasKey(ab => new { ab.UserId, ab.ProjectId });




        //relatia one-to-many (Project-RolePerProject)
        modelBuilder.Entity<RolePerProject>()
            .HasOne(t => t.Project)
            .WithMany(t => t.RolePerProjects) // vectorul de RolePerProject din Project
            .HasForeignKey(t => t.ProjectId);

        modelBuilder.Entity<RolePerProject>()
            .HasOne(t => t.User)
            .WithMany(t => t.RolePerProjects)
            .HasForeignKey(t => t.UserId);

        // many-to-many (User-Task)
        modelBuilder.Entity<UserTasks>()
            .HasOne(t => t.User)
            .WithMany(t => t.UserTasks)
            .HasForeignKey(t => t.UserId);

        modelBuilder.Entity<UserTasks>()
            .HasOne(t => t.Task)
            .WithMany(t => t.UserTasks)
            .HasForeignKey(t => t.TaskId);

        /*keep this if needed in the future
         *
         * modelBuilder.Entity<Project>()
            .HasMany(t => t.RolePerProjects)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);*/

        //modelBuilder.Entity<RolePerProject>().HasOne(ab => ab.User).WithMany(ab => ab.RolePerProjects)
    }
}