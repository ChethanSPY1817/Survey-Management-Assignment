namespace SurveyManagement.Application.DTOs.SurveyDTOs
{
    public class CreateSurveyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public bool IsActive { get; set; }

    }
}
