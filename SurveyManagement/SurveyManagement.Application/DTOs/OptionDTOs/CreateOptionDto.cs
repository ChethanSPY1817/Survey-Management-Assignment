namespace SurveyManagement.Application.DTOs.OptionDTOs
{
    public class CreateOptionDto
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
