using Bookery.Node.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookery.Node.Data;

public class AppDbContext : DbContext
{
    public DbSet<Models.Node> Nodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserNode> UserNodes { get; set; }
    public DbSet<AccessType> AccessTypes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Models.Node>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<Models.Node>()
            .Property(x => x.ParentId).IsRequired(false);
        modelBuilder
            .Entity<Models.Node>()
            .Property(x => x.Name).IsRequired();
        modelBuilder
            .Entity<Models.Node>()
            .Property(x => x.IsDirectory).IsRequired();
        modelBuilder
            .Entity<Models.Node>()
            .Property(x => x.Size);
        modelBuilder
            .Entity<Models.Node>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<Models.Node>()
            .HasOne(x => x.Owner)
            .WithMany(x => x.OwnedNodes)
            .HasForeignKey(x => x.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder
            .Entity<Models.Node>()
            .HasOne(x => x.ModifiedBy)
            .WithMany(x => x.ModifiedNodes)
            .HasForeignKey(x => x.ModifiedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<Models.Node>()
            .HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedNodes)
            .HasForeignKey(x => x.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<User>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<User>()
            .Property(x => x.Email).IsRequired();
        modelBuilder
            .Entity<User>()
            .Property(x => x.LastName).IsRequired();
        modelBuilder
            .Entity<User>()
            .Property(x => x.FirstName).IsRequired();

        modelBuilder
            .Entity<UserNode>()
            .HasKey(x => new { x.UserId, x.NodeId });
        modelBuilder
            .Entity<UserNode>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserNodes)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<UserNode>()
            .Property(x => x.AccessTypeId)
            .HasConversion<int>()
            .IsRequired();

        modelBuilder
            .Entity<AccessType>()
            .Property(x => x.AccessTypeId)
            .HasConversion<int>()
            .IsRequired();
        modelBuilder
            .Entity<AccessType>()
            .Property(x => x.Name)
            .IsRequired();
        modelBuilder
            .Entity<AccessType>()
            .HasData(Enum.GetValues(typeof(AccessTypeId))
                .Cast<AccessTypeId>()
                .Select(x => new AccessType
                {
                    AccessTypeId = x,
                    Name = x.ToString()
                }));

        base.OnModelCreating(modelBuilder);
    }
}