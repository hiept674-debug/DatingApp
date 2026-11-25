using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // ========================================
    // DB SETS
    // ========================================

    public DbSet<User> Users => Set<User>();

    public DbSet<Like> Likes => Set<Like>();

    public DbSet<Match> Matches => Set<Match>();

    public DbSet<Conversation> Conversations
        => Set<Conversation>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<Block> Blocks => Set<Block>();

    public DbSet<Report> Reports => Set<Report>();

    public DbSet<StoredFile> StoredFiles
        => Set<StoredFile>();

    public DbSet<DemoProfile> DemoProfiles
        => Set<DemoProfile>();

    public DbSet<Interest> Interests
        => Set<Interest>();

    public DbSet<UserInterest> UserInterests
        => Set<UserInterest>();

    public DbSet<RefreshToken> RefreshTokens
        => Set<RefreshToken>();

    public DbSet<PasswordResetCode> PasswordResetCodes
        => Set<PasswordResetCode>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // 1. USER
        // ========================================

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique();

        // ========================================
        // 2. LIKE
        // ========================================

        modelBuilder.Entity<Like>()
            .HasOne(l => l.FromUser)
            .WithMany()
            .HasForeignKey(l => l.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.ToUser)
            .WithMany()
            .HasForeignKey(l => l.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasIndex(l => new
            {
                l.FromUserId,
                l.ToUserId
            })
            .IsUnique();

        // ========================================
        // 3. MATCH
        // ========================================

        modelBuilder.Entity<Match>()
            .HasOne(m => m.User1)
            .WithMany()
            .HasForeignKey(m => m.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.User2)
            .WithMany()
            .HasForeignKey(m => m.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasIndex(m => new
            {
                m.User1Id,
                m.User2Id
            })
            .IsUnique();

        // ========================================
        // 4. CONVERSATION
        // ========================================

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Match)
            .WithOne(m => m.Conversation)
            .HasForeignKey<Conversation>(
                c => c.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasIndex(c => c.MatchId)
            .IsUnique();

        // ========================================
        // 5. MESSAGE
        // ========================================

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // ========================================
        // 6. BLOCK
        // ========================================

        modelBuilder.Entity<Block>()
            .HasOne(b => b.Blocker)
            .WithMany()
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Block>()
            .HasOne(b => b.BlockedUser)
            .WithMany()
            .HasForeignKey(b => b.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Block>()
            .HasIndex(b => new
            {
                b.BlockerId,
                b.BlockedUserId
            })
            .IsUnique();

        // ========================================
        // 7. REPORT
        // ========================================

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reporter)
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportedUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // ========================================
        // 8. STORED FILE
        // ========================================

        modelBuilder.Entity<StoredFile>()
            .HasOne(f => f.Owner)
            .WithMany()
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoredFile>()
            .HasIndex(f => new
            {
                f.OwnerId,
                f.Purpose
            });

        // ========================================
        // 9. DEMO PROFILE
        // ========================================

        modelBuilder.Entity<DemoProfile>()
            .HasIndex(p => p.DisplayName);

        modelBuilder.Entity<DemoProfile>()
            .HasIndex(p => p.Interest);

        modelBuilder.Entity<DemoProfile>()
            .HasIndex(p => new
            {
                p.IsActive,
                p.CreatedAt
            });

        // ========================================
        // 10. INTEREST
        // ========================================

        modelBuilder.Entity<Interest>()
            .HasIndex(i => i.Name)
            .IsUnique();

        modelBuilder.Entity<Interest>()
            .HasIndex(i => i.Category);

        // ========================================
        // 11. USER INTEREST
        // ========================================

        // Composite Primary Key
        modelBuilder.Entity<UserInterest>()
            .HasKey(ui => new
            {
                ui.UserId,
                ui.InterestId
            });

        // Relationship: User - UserInterest
        modelBuilder.Entity<UserInterest>()
            .HasOne(ui => ui.User)
            .WithMany(u => u.UserInterests)
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Interest - UserInterest
        modelBuilder.Entity<UserInterest>()
            .HasOne(ui => ui.Interest)
            .WithMany(i => i.UserInterests)
            .HasForeignKey(ui => ui.InterestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserInterest>()
            .HasIndex(ui => ui.InterestId);
    }
}