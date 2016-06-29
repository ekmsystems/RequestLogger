using RequestLogger.Loggers.Wrappers;

namespace RequestLogger.Loggers
{
    public class ConsoleRequestLoggerConfiguration
    {
        public ISystemConsole SystemConsole { get; set; }

        public ConsoleRequestLoggerConfiguration()
        {
            SystemConsole = new ConsoleWrapper();
        }
    }
}