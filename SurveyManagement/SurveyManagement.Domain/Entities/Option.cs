namespace SurveyManagement.Domain.Entities
{
    public class Option
    {
        public Guid OptionId { get; set; }
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Order { get; set; }

        public Question? Question { get; set; }
        public ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}