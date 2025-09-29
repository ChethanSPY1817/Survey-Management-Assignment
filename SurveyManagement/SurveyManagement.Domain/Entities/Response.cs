namespace SurveyManagement.Domain.Entities
{
    public class Response
    {
        public Guid ResponseId { get; set; }
        public Guid UserSurveyId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? OptionId { get; set; } // Nullable for open-ended questions
        public string? TextAnswer { get; set; } // For text responses
        public DateTime AnsweredAt { get; set; }

        public UserSurvey? UserSurvey { get; set; }
        public Question? Question { get; set; }
        public Option? Option { get; set; }
    }
}