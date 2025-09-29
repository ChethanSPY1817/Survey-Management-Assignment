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
            // User <-> UserProfile (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // User <-> UserSurvey (1:M)
            modelBuilder.Entity<UserSurvey>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSurveys)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict); // <--- Change here

            // Survey <-> UserSurvey (1:M)
            modelBuilder.Entity<UserSurvey>()
                .HasOne(us => us.Survey)
                .WithMany(s => s.UserSurveys)
                .HasForeignKey(us => us.SurveyId)
                .OnDelete(DeleteBehavior.Cascade); // or .Restrict

            // Survey <-> Questions (1:M)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId);

            // Question <-> Options (1:M)
            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId);

            // Question <-> Responses (1:M)
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Question)
                .WithMany(q => q.Responses)
                .HasForeignKey(r => r.QuestionId);

            // Option <-> Responses (1:M, nullable)
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Option)
                .WithMany(o => o.Responses)
                .HasForeignKey(r => r.OptionId)
                .IsRequired(false);

            // UserSurvey <-> Responses (1:M)
            modelBuilder.Entity<Response>()
                .HasOne(r => r.UserSurvey)
                .WithMany(us => us.Responses)
                .HasForeignKey(r => r.UserSurveyId)
                .OnDelete(DeleteBehavior.Restrict); // Change from Cascade to Restrict

            // Survey <-> Product (M:1, optional)
            modelBuilder.Entity<Survey>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Surveys)
                .HasForeignKey(s => s.ProductId)
                .IsRequired(false);

            // Survey <-> User (CreatedBy, 1:M)
            modelBuilder.Entity<Survey>()
                .HasOne(s => s.CreatedByUser)
                .WithMany(u => u.CreatedSurveys)
                .HasForeignKey(s => s.CreatedByUserId);

            // Product GUIDs
            var productAId = new Guid("f10759ab-87c2-462c-9115-9b8ed43d4dde");
            var productBId = new Guid("ae38f26b-ff84-4569-bea1-cfe210becda4");
            var productCId = new Guid("f02d136d-e7c6-4881-bb7a-c43070c179c3");
            var productDId = new Guid("87c332a2-87d3-4788-9cbc-0dd540c59e87");
            var productEId = new Guid("c97446f9-f252-44d4-b6a9-36e7a9f3720f");

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = productAId, Name = "Product A", Description = "Description A" },
                new Product { ProductId = productBId, Name = "Product B", Description = "Description B" },
                new Product { ProductId = productCId, Name = "Product C", Description = "Description C" },
                new Product { ProductId = productDId, Name = "Product D", Description = "Description D" },
                new Product { ProductId = productEId, Name = "Product E", Description = "Description E" }
            );

            // User GUIDs
            var adminId = new Guid("921fe07b-8706-4a4d-b14a-67d3f656019c");
            var user1Id = new Guid("b0b2ef16-cfb4-4e46-bb57-a81dc9889fe6");
            var user2Id = new Guid("dfde638b-5f4e-4f12-8f77-42125f5224a1");
            var user3Id = new Guid("4536d792-7ece-49f4-96d9-7be228ab6a09");
            var user4Id = new Guid("7e595c7f-1e5b-4b17-9692-a4df53985a1e");

            var createdAt = new DateTime(2025, 9, 29, 5, 30, 27, 928, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User { UserId = adminId, Username = "admin1", Email = "admin1@example.com", PasswordHash = "hash1", Role = UserRole.Admin, CreatedAt = createdAt },
                new User { UserId = user1Id, Username = "user1", Email = "user1@example.com", PasswordHash = "hash2", Role = UserRole.Respondent, CreatedAt = createdAt },
                new User { UserId = user2Id, Username = "user2", Email = "user2@example.com", PasswordHash = "hash3", Role = UserRole.Respondent, CreatedAt = createdAt },
                new User { UserId = user3Id, Username = "user3", Email = "user3@example.com", PasswordHash = "hash4", Role = UserRole.Respondent, CreatedAt = createdAt },
                new User { UserId = user4Id, Username = "user4", Email = "user4@example.com", PasswordHash = "hash5", Role = UserRole.Respondent, CreatedAt = createdAt }
            );

            // UserProfile GUIDs
            var adminProfileId = new Guid("b537dc63-cac6-4baf-b0d3-c20ba4d04db4");
            var user1ProfileId = new Guid("9cf17099-7275-471f-8822-af8cc92ddc7c");
            var user2ProfileId = new Guid("b7b2c4dc-04d4-46db-b637-552a02d7140b");
            var user3ProfileId = new Guid("9a9b26c2-072f-4457-a2d4-ee61f00ba816");
            var user4ProfileId = new Guid("17de54da-b918-47e5-affd-ade34970751c");

            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile { UserProfileId = adminProfileId, UserId = adminId, FirstName = "Admin", LastName = "One", Phone = "1111111111", Address = "Admin Address" },
                new UserProfile { UserProfileId = user1ProfileId, UserId = user1Id, FirstName = "User", LastName = "One", Phone = "2222222222", Address = "User1 Address" },
                new UserProfile { UserProfileId = user2ProfileId, UserId = user2Id, FirstName = "User", LastName = "Two", Phone = "3333333333", Address = "User2 Address" },
                new UserProfile { UserProfileId = user3ProfileId, UserId = user3Id, FirstName = "User", LastName = "Three", Phone = "4444444444", Address = "User3 Address" },
                new UserProfile { UserProfileId = user4ProfileId, UserId = user4Id, FirstName = "User", LastName = "Four", Phone = "5555555555", Address = "User4 Address" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}