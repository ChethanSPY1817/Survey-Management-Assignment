using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyManagement.Infrastructure.Data;
using SurveyManagement.API;

class Program
{
    static async Task Main()
    {
        // 1. Build configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = config.GetConnectionString("SurveyDb");

        // 2. Configure DbContext
        var options = new DbContextOptionsBuilder<SurveyDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var db = new SurveyDbContext(options);

        // 3. Define user passwords to hash
        var userPasswords = new Dictionary<string, string>
        {
            { "admin1@example.com", "Admin@123" },
            { "user1@example.com", "User1@123" },
            { "user2@example.com", "User2@123" },
            { "user3@example.com", "User3@123" },
            { "user4@example.com", "User4@123" }
        };

        // 4. Update password hashes
        foreach (var kvp in userPasswords)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == kvp.Key);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(kvp.Value);
                db.Users.Update(user);
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("Password hashes updated successfully.");
    }
}
