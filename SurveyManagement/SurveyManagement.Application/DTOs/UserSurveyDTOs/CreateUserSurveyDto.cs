namespace SurveyManagement.Application.DTOs.UserSurveyDTOs
{
    public class CreateUserSurveyDto
    {
        public Guid UserId { get; set; }
        public Guid SurveyId { get; set; }
        public Guid CreatedById { get; set; } // Added
    }
}
