namespace SurveyManagement.Domain.Exceptions
{
    // Example: Domain-specific
    public class InvalidSurveyStateException : Exception
    {
        public InvalidSurveyStateException(string message) : base(message) { }
    }
}
