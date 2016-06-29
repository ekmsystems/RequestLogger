namespace RequestLogger.Loggers
{
    public class ConsoleRequestLoggerConfiguration
    {
        public IConsoleRequestLogWriter LogWriter { get; set; }

        public ConsoleRequestLoggerConfiguration()
        {
            LogWriter = new ConsoleRequestLogLogWriter();
        }
    }
}
