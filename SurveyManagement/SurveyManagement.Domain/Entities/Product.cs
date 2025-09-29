namespace SurveyManagement.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    }
}