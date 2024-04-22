using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Model;
using TaskmanAPI.Models;

namespace TaskmanAPI.Contexts;

public class DefaultContext : IdentityDbContext<User>
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    // public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjTask> ProjTasks { get; set; }
    public DbSet<RolePerProject> RolePerProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder
       modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RolePerProject>()
            .HasKey(rp => new { rp.UserId, rp.ProjectId });

        modelBuilder.Entity<RolePerProject>()
            .HasOne(rp => rp.User)
            .WithMany(u => u.RolePerProjects)
            .HasForeignKey(rp => rp.UserId);

        modelBuilder.Entity<RolePerProject>()
            .HasOne(rp => rp.Project)
            .WithMany(p => p.RolePerProjects)
            .HasForeignKey(rp => rp.ProjectId);
    }
}