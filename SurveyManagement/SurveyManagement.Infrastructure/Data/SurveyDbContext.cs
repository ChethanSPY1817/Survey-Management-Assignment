using Microsoft.EntityFrameworkCore;
using SurveyManagement.Domain.Entities;

namespace SurveyManagement.Infrastructure.Data
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options)
            : base(options)
        {
        }

        public DbSet<PasswordHistory> PasswordHistories { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<UserSurvey> UserSurveys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -------------------
            // Relationships
            // -------------------
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            modelBuilder.Entity<UserSurvey>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSurveys)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSurvey>()
                .HasOne(us => us.Survey)
                .WithMany(s => s.UserSurveys)
                .HasForeignKey(us => us.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId);

            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Question)
                .WithMany(q => q.Responses)
                .HasForeignKey(r => r.QuestionId);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Option)
                .WithMany(o => o.Responses)
                .HasForeignKey(r => r.OptionId)
                .IsRequired(false);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.UserSurvey)
                .WithMany(us => us.Responses)
                .HasForeignKey(r => r.UserSurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Survey>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Surveys)
                .HasForeignKey(s => s.ProductId)
                .IsRequired(false);

            modelBuilder.Entity<Survey>()
                .HasOne(s => s.CreatedByUser)
                .WithMany(u => u.CreatedSurveys)
                .HasForeignKey(s => s.CreatedByUserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
