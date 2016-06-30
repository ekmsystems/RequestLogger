using RequestLogger.Formatters;
using RequestLogger.Wrappers;

namespace RequestLogger.Loggers
{
    public class ConsoleLoggerConfiguration
    {
        public ISystemConsole SystemConsole { get; set; }
        public IHeaderFormatter HeaderFormatter { get; set; }

        public ConsoleLoggerConfiguration()
        {
            SystemConsole = new ConsoleWrapper();
            HeaderFormatter = new DefaultHeaderFormatter();
        }
    }
}