namespace SurveyManagement.Domain.Entities
{
    public class UserSurvey
    {
        public Guid UserSurveyId { get; set; }
        public Guid UserId { get; set; }
        public Guid SurveyId { get; set; }
        public Guid CreatedById { get; set; } // Added to track which admin created it
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public User? User { get; set; }
        public Survey? Survey { get; set; }
        public ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}
