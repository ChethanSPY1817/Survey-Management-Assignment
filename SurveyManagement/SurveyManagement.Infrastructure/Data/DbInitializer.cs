using SurveyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace SurveyManagement.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SurveyDbContext context)
        {
            // Apply pending migrations
            context.Database.Migrate();

            // Check if users already exist
            if (context.Users.Any())
                return; // DB has been seeded already

            var createdAt = DateTime.UtcNow;

            // -------------------
            // Seed Products
            // -------------------
            var products = new[]
            {
                new Product { ProductId = Guid.NewGuid(), Name = "Product A", Description = "Description A" },
                new Product { ProductId = Guid.NewGuid(), Name = "Product B", Description = "Description B" },
                new Product { ProductId = Guid.NewGuid(), Name = "Product C", Description = "Description C" },
                new Product { ProductId = Guid.NewGuid(), Name = "Product D", Description = "Description D" },
                new Product { ProductId = Guid.NewGuid(), Name = "Product E", Description = "Description E" }
            };
            context.Products.AddRange(products);

            // -------------------
            // Seed Users
            // -------------------
            var users = new[]
            {
                new { Username = "admin1", Email = "admin1@example.com", Password = "Admin@123", Role = UserRole.Admin },
                new { Username = "user1", Email = "user1@example.com", Password = "User1@123", Role = UserRole.Respondent }
            };

            foreach (var u in users)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(u.Password);

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = u.Username,
                    Email = u.Email,
                    PasswordHash = hashedPassword,
                    Role = u.Role,
                    CreatedAt = createdAt
                };

                context.Users.Add(user);

                // Store the actual password in PasswordHistory
                var passwordHistory = new PasswordHistory
                {
                    PasswordHistoryId = Guid.NewGuid(),
                    UserId = user.UserId,
                    PlainPassword = u.Password, // Plain password or you can encrypt it
                    CreatedAt = createdAt
                };
                context.PasswordHistories.Add(passwordHistory);

                // Also create a basic profile
                var profile = new UserProfile
                {
                    UserProfileId = Guid.NewGuid(),
                    UserId = user.UserId,
                    FirstName = u.Username.Split('1')[0], // just basic demo
                    LastName = u.Username.Split('1')[1] ?? "",
                    Phone = "0000000000",
                    Address = "Default Address"
                };
                context.UserProfiles.Add(profile);
            }

            context.SaveChanges();
        }
    }
}
