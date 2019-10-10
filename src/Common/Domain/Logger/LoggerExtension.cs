using Microsoft.Extensions.Logging;

namespace Domain.Logger
{
    public static class LoggerExtension
    {
        public static void Info(this ILogger logger, LoggerEvent logEvent)
        {
            logger.Log(LogLevel.Information,
            default,
            logEvent,
            null,
            LoggerEvent.Formatter);
        }

        public static void Info(this ILogger logger, StructLoggerEvent data)
        {
            logger.Log(LogLevel.Information, data);
        }

        public static void Log(this ILogger logger, LogLevel logLevel, StructLoggerEvent data)
        {
            logger.Log(logLevel, "{data}", data);
        }
    }
}
