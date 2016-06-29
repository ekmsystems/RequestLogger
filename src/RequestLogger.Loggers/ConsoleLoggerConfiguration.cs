using RequestLogger.Loggers.Wrappers;

namespace RequestLogger.Loggers
{
    public class ConsoleLoggerConfiguration
    {
        public ISystemConsole SystemConsole { get; set; }

        public ConsoleLoggerConfiguration()
        {
            SystemConsole = new ConsoleWrapper();
        }
    }
}