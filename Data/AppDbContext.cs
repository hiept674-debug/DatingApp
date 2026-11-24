using DatingAppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Like> Likes { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Block> Blocks { get; set; }

    public DbSet<Report> Reports { get; set; }

    public DbSet<StoredFile> StoredFiles { get; set; }

    public DbSet<RefreshToken> RefreshTokens
    => Set<RefreshToken>();

    public DbSet<PasswordResetCode> PasswordResetCodes
    => Set<PasswordResetCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // USER


        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique();





        // LIKE - FROM USER
        //  

        modelBuilder.Entity<Like>()
            .HasOne(l => l.FromUser)
            .WithMany()
            .HasForeignKey(l => l.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // LIKE - TO USER
        //  

        modelBuilder.Entity<Like>()
            .HasOne(l => l.ToUser)
            .WithMany()
            .HasForeignKey(l => l.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Một User không thể Like cùng một User
        // nhiều lần
        modelBuilder.Entity<Like>()
            .HasIndex(l => new
            {
                l.FromUserId,
                l.ToUserId
            })
            .IsUnique();


        //  
        // MATCH - USER 1
        //  

        modelBuilder.Entity<Match>()
            .HasOne(m => m.User1)
            .WithMany()
            .HasForeignKey(m => m.User1Id)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // MATCH - USER 2
        //  

        modelBuilder.Entity<Match>()
            .HasOne(m => m.User2)
            .WithMany()
            .HasForeignKey(m => m.User2Id)
            .OnDelete(DeleteBehavior.Restrict);


        // Một cặp User chỉ có một Match
        modelBuilder.Entity<Match>()
            .HasIndex(m => new
            {
                m.User1Id,
                m.User2Id
            })
            .IsUnique();


        //  
        // MATCH - CONVERSATION
        // 1 Match = 1 Conversation
        //  

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Match)
            .WithOne(m => m.Conversation)
            .HasForeignKey<Conversation>(c => c.MatchId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Conversation>()
            .HasIndex(c => c.MatchId)
            .IsUnique();


        //  
        // CONVERSATION - MESSAGE
        // 1 Conversation = nhiều Message
        //  

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);


        //  
        // MESSAGE - SENDER
        //  

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // BLOCK - BLOCKER
        //  

        modelBuilder.Entity<Block>()
            .HasOne(b => b.Blocker)
            .WithMany()
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // BLOCK - BLOCKED USER
        //  

        modelBuilder.Entity<Block>()
            .HasOne(b => b.BlockedUser)
            .WithMany()
            .HasForeignKey(b => b.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Một User chỉ Block một User một lần
        modelBuilder.Entity<Block>()
            .HasIndex(b => new
            {
                b.BlockerId,
                b.BlockedUserId
            })
            .IsUnique();


        //  
        // REPORT - REPORTER
        //  

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reporter)
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // REPORT - REPORTED USER
        //  

        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReportedUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedUserId)
            .OnDelete(DeleteBehavior.Restrict);


        //  
        // REPORT - REVIEWER
        // Moderator / Admin
        //  

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}