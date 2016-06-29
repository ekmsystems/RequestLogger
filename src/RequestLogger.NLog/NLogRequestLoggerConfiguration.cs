using System;
using NLog;
using RequestLogger.Formatters;

namespace RequestLogger.NLog
{
    public class NLogRequestLoggerConfiguration
    {
        public NLogPropertyKeys Keys { get; private set; }
        public ILogger Logger { get; set; }
        public IHeaderFormatter HeaderFormatter { get; set; }
        public Action<LogEventInfo> BeforeLogHook { get; set; }
        public Action<LogEventInfo> BeforeLogErrorHook { get; set; }

        public NLogRequestLoggerConfiguration()
        {
            Keys = new NLogPropertyKeys();
            Logger = LogManager.GetLogger("NLogRequestLogger");
            HeaderFormatter = new DefaultHeaderFormatter();
        }
    }
}
