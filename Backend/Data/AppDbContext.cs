using Microsoft.EntityFrameworkCore;
using TestService.Models;

namespace TestService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Creator)
                      .WithMany(u => u.CreatedTests)
                      .HasForeignKey(e => e.CreatorId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.Questions)
                      .WithOne(q => q.Test)
                      .HasForeignKey(q => q.TestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Answers)
                      .WithOne(a => a.Question)
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.TestResults)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.CreatedTests)
                      .WithOne(t => t.Creator)
                      .HasForeignKey(t => t.CreatorId);
                entity.HasMany(e => e.TestResults)
                      .WithOne(r => r.User)
                      .HasForeignKey(r => r.UserId);
            });
        }
    }
}