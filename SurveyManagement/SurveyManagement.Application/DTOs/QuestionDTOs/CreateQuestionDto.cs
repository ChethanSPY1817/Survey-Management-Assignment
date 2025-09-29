namespace SurveyManagement.Application.DTOs.QuestionDTOs
{
    public class CreateQuestionDto
    {
        public Guid SurveyId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}

