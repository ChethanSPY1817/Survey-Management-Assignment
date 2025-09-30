using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace SurveyManagement.CrossCutting.Logging
{
    public static class LoggerConfigurator
    {
        public static void ConfigureLogging(IConfiguration configuration)
        {
            // Configure Serilog to read settings from appsettings.json
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // ✅ IConfiguration here
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/general-.log",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .CreateLogger();
        }
    }
}
