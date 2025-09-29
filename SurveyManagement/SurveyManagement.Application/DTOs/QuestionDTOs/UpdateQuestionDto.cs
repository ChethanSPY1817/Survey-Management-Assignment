
namespace SurveyManagement.Application.DTOs.QuestionDTOs
{
    public class UpdateQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
