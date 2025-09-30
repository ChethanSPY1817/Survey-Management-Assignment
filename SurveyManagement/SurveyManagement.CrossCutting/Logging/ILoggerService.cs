namespace SurveyManagement.CrossCutting.Logging
{
    public interface IServiceLogger<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }
}
