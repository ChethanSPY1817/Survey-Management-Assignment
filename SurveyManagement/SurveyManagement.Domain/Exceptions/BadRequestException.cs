namespace SurveyManagement.Domain.Exceptions
{
    // Generic 400 Bad Request  
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
