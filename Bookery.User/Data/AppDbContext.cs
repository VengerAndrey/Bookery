using Microsoft.EntityFrameworkCore;

namespace Bookery.User.Data;

public class AppDbContext : DbContext
{
    public DbSet<Models.User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Models.User>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<Models.User>()
            .Property(x => x.Email).IsRequired();
        modelBuilder
            .Entity<Models.User>()
            .Property(x => x.Password).IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}