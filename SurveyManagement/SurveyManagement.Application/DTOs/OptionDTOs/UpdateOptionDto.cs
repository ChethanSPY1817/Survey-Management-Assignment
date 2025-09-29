namespace SurveyManagement.Application.DTOs.OptionDTOs
{
    public class UpdateOptionDto
    {
        public Guid OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
