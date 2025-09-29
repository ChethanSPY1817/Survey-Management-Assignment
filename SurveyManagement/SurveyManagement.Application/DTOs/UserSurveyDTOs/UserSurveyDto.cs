namespace SurveyManagement.Application.DTOs.UserSurveyDTOs
{
    public class UserSurveyDto
    {
        public Guid UserSurveyId { get; set; }
        public Guid UserId { get; set; }
        public Guid SurveyId { get; set; }
        public Guid CreatedById { get; set; } // Added
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
