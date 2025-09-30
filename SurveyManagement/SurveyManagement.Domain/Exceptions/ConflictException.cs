namespace SurveyManagement.Domain.Exceptions
{
    // Conflict / Invalid Operation
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
