using RequestLogger.Loggers.LogWriters;

namespace RequestLogger.Loggers
{
    public class ConsoleRequestLoggerConfiguration
    {
        public IConsoleLogWriter ConsoleLogWriter { get; set; }

        public ConsoleRequestLoggerConfiguration()
        {
            ConsoleLogWriter = new ConsoleLogLogWriter();
        }
    }
}
