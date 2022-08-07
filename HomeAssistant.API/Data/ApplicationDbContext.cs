using HomeAssistant.API.Entities;
using HomeAssistant.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
        .ToTable("Users");


        builder.Entity<Conversation>()
        .HasMany(c => c.Messages)
        .WithOne(m => m.Conversation);

        builder.Entity<Conversation>()
        .HasOne(c => c.Creator)
        .WithMany(u => u.Conversations);

        builder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(m => m.Messages);

        builder.Entity<Message>()
          .HasOne(m => m.Sender)
          .WithMany(m => m.Messages);

        builder.Entity<UserConversation>()
            .ToTable("UserConversations")
            .HasKey(t => new { t.ConversationId, t.UserId });

        builder.Entity<UserConversation>()
            .HasOne(t => t.Conversation)
            .WithMany(t => t.UserConversations)
            .HasForeignKey(t => t.ConversationId);

        builder.Entity<UserConversation>()
            .HasOne(t => t.User)
            .WithMany(t => t.UserConversations)
            .HasForeignKey(t => t.UserId);
    }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}