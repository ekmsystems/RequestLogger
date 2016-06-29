using System;
using NLog;
using RequestLogger.Formatters;

namespace RequestLogger.NLog
{
    public class NLogLoggerConfiguration
    {
        public NLogPropertyKeys Keys { get; private set; }
        public ILogger Logger { get; set; }
        public IHeaderFormatter HeaderFormatter { get; set; }
        public Action<LogEventInfo> BeforeLogHook { get; set; }
        public Action<LogEventInfo> BeforeLogErrorHook { get; set; }

        public NLogLoggerConfiguration()
        {
            Keys = new NLogPropertyKeys();
            Logger = LogManager.GetLogger("RequestLogger");
            HeaderFormatter = new DefaultHeaderFormatter();
        }
    }
}
