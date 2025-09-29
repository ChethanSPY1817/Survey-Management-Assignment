namespace SurveyManagement.Application.DTOs.ResponseDTOs
{
    public class CreateResponseDto
    {
        public Guid UserSurveyId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? OptionId { get; set; } // Nullable
        public string? TextAnswer { get; set; }
    }
}
