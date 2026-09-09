using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Profile> Profiles { get; set; }

    public DbSet<Photo> Photos { get; set; }


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // User - Profile
        modelBuilder.Entity<Profile>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Profile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // Profile - Photos
        modelBuilder.Entity<Photo>()
            .HasOne(x => x.Profile)
            .WithMany(x => x.Photos)
            .HasForeignKey(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);


        // Email unique
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();


        // Phone unique
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Phone)
            .IsUnique();
    }
}