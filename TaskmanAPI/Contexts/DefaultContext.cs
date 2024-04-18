using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Model;

namespace TaskmanAPI.Contexts;

// RENAME THIS IF USING MORE CONTEXTS
public class DefaultContext : DbContext
{
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}