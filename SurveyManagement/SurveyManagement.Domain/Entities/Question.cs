namespace SurveyManagement.Domain.Entities
{
    public enum QuestionType
    {
        SingleChoice,
        MultipleChoice,
        Text,
        Rating
    }

    public class Question
    {
        public Guid QuestionId { get; set; }
        public Guid SurveyId { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int Order { get; set; }

        public Survey? Survey { get; set; }
        public ICollection<Option> Options { get; set; } = new List<Option>();
        public ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}