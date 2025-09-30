namespace SurveyManagement.Domain.Exceptions
{
    // Unauthorized / Forbidden
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
