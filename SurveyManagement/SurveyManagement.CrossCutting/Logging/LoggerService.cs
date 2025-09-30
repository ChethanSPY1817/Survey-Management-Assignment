using Serilog;

namespace SurveyManagement.CrossCutting.Logging
{
    public class ServiceLogger<T> : IServiceLogger<T>
    {
        private readonly ILogger _logger;

        public ServiceLogger()
        {
            // Each service has its own log file
            var serviceName = typeof(T).Name;
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"Logs/{serviceName}-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message) => _logger.Information(message);

        public void LogWarning(string message) => _logger.Warning(message);

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger.Error(ex, message);
            else
                _logger.Error(message);
        }
    }
}
