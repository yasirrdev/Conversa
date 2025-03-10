using Conversa.Models.Databases.Entities;
using Microsoft.EntityFrameworkCore;

namespace Conversa.Models.Databases;

public class DataContext : DbContext
{

    private const string DATABASE_PATH = "conversa.db";

    public DbSet<Groups> Groups { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<Users_Groups> Users_Groups { get; set; }
    public DbSet<Contacts> Contacts { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        optionsBuilder.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contacts>()
            .HasOne(c => c.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contacts>()
            .HasOne(c => c.ContactUser)
            .WithMany()
            .HasForeignKey(c => c.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Messages>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Messages>()
            .HasOne(m => m.Receiver)  
            .WithMany()              
            .HasForeignKey(m => m.ReceiverId) 
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Messages>()
            .HasOne(m => m.Group)
            .WithMany(g => g.Messages)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Users_Groups>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Users_Groups>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Groups>()
            .HasOne(g => g.Creator)
            .WithMany()
            .HasForeignKey(g => g.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}

