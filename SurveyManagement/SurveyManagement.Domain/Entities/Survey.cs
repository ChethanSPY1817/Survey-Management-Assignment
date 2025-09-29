namespace SurveyManagement.Domain.Entities
{
    public class Survey
    {
        public Guid SurveyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid? ProductId { get; set; }

        public User? CreatedByUser { get; set; }
        public Product? Product { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<UserSurvey> UserSurveys { get; set; } = new List<UserSurvey>();
    }
}