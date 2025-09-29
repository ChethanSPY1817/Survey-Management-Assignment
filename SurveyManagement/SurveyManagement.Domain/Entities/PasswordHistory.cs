namespace SurveyManagement.Domain.Entities
{
    public class PasswordHistory
    {
        public Guid PasswordHistoryId { get; set; }
        public Guid UserId { get; set; }
        public string PlainPassword { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
