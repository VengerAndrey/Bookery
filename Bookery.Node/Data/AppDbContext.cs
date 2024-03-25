using Bookery.Node.Common.Enums;
using Bookery.Node.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Data;

public class AppDbContext : DbContext
{
    public DbSet<NodeEntity> Nodes => Set<NodeEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<UserNodeEntity> UserNodes => Set<UserNodeEntity>();
    public DbSet<AccessTypeEntity> AccessTypes => Set<AccessTypeEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureNodeEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
        ConfigureUserNodeEntity(modelBuilder);
        ConfigureAccessTypeEntity(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigureNodeEntity(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<NodeEntity>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<NodeEntity>()
            .Property(x => x.ParentId)
            .IsRequired(false);
        modelBuilder
            .Entity<NodeEntity>()
            .Property(x => x.Name)
            .IsRequired();
        modelBuilder
            .Entity<NodeEntity>()
            .Property(x => x.IsDirectory)
            .IsRequired();
        modelBuilder
            .Entity<NodeEntity>()
            .Property(x => x.Size)
            .IsRequired(false);
        modelBuilder
            .Entity<NodeEntity>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<NodeEntity>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.OwnedNodes)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<NodeEntity>()
            .HasOne(x => x.ModifiedBy)
            .WithMany(x => x.ModifiedNodes)
            .HasForeignKey(x => x.ModifiedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<NodeEntity>()
            .HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedNodes)
            .HasForeignKey(x => x.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }

    private void ConfigureUserEntity(ModelBuilder modelBuilder)
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
    }

    private void ConfigureUserNodeEntity(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserNodeEntity>()
            .HasKey(x => new { x.UserId, x.NodeId });
        modelBuilder
            .Entity<UserNodeEntity>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserNodes)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<UserNodeEntity>()
            .Property(x => x.AccessTypeId)
            .HasConversion<int>()
            .IsRequired();
    }

    private void ConfigureAccessTypeEntity(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<AccessTypeEntity>()
            .HasKey(x => x.AccessTypeId);
        modelBuilder
            .Entity<AccessTypeEntity>()
            .Property(x => x.AccessTypeId)
            .HasConversion<int>()
            .IsRequired();
        modelBuilder
            .Entity<AccessTypeEntity>()
            .Property(x => x.Name)
            .IsRequired();
        modelBuilder
            .Entity<AccessTypeEntity>()
            .HasData(Enum.GetValues(typeof(AccessTypeId))
                .Cast<AccessTypeId>()
                .Select(x => new AccessTypeEntity
                {
                    AccessTypeId = x,
                    Name = x.ToString()
                }));
    }
}