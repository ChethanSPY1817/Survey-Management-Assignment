namespace SurveyManagement.Application.DTOs.ResponseDTOs
{
    public class ResponseDto
    {
        public Guid ResponseId { get; set; }
        public Guid UserSurveyId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? OptionId { get; set; } // Nullable
        public string? TextAnswer { get; set; }
        public DateTime AnsweredAt { get; set; }
    }
}
