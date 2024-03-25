using Bookery.User.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookery.User.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserEntity>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<UserEntity>()
            .Property(x => x.Email)
            .IsRequired();
        modelBuilder
            .Entity<UserEntity>()
            .Property(x => x.FirstName)
            .IsRequired();
        modelBuilder
            .Entity<UserEntity>()
            .Property(x => x.LastName)
            .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}