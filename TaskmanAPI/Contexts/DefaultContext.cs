using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Contexts;

public class DefaultContext(DbContextOptions<DefaultContext> options) : IdentityDbContext<User>(options)
{
    public override DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; init; }
    public DbSet<ProjTask> ProjTasks { get; init; }
    public DbSet<RolePerProject> RolePerProjects { get; init; }
    public DbSet<UserTasks> UserTasks { get; init; }

    protected override void OnModelCreating(ModelBuilder
        modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RolePerProject>()
            .HasKey(ab => new { ab.UserId, ab.ProjectId });

        modelBuilder.Entity<UserTasks>()
            .HasKey(ab => new { ab.UserId, ab.TaskId });

        // one-to-many (Project-RolePerProject)
        modelBuilder.Entity<RolePerProject>()
            .HasOne(t => t.Project)
            .WithMany(t => t.RolePerProjects) // vectorul de RolePerProject din Project
            .HasForeignKey(t => t.ProjectId);

        // one-to-many (User-RolePerProject)
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
    }
}