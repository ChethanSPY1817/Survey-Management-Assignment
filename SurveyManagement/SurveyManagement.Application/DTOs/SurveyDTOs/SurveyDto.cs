namespace SurveyManagement.Application.DTOs.SurveyDTOs
{
    public class SurveyDto
    {
        public Guid SurveyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public bool IsActive { get; set; }

        [System.Text.Json.Serialization.JsonIgnore] // hides in request body
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
