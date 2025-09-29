namespace SurveyManagement.Domain.Entities
{
    public enum UserRole
    {
        Admin,
        Respondent
    }

    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserProfile? Profile { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; } = new List<UserSurvey>();
        public ICollection<Survey> CreatedSurveys { get; set; } = new List<Survey>();
    }
}