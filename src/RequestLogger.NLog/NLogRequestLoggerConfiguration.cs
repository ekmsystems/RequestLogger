using System;
using NLog;

namespace RequestLogger.NLog
{
    public class NLogRequestLoggerConfiguration
    {
        public NLogPropertyKeys Keys { get; private set; }
        public ILogger Logger { get; set; }
        public Action<LogEventInfo> BeforeLogHook { get; set; }
        public Action<LogEventInfo> BeforeLogErrorHook { get; set; }

        public NLogRequestLoggerConfiguration()
        {
            Keys = new NLogPropertyKeys();
            Logger = LogManager.GetLogger("NLogRequestLogger");
        }
    }
}
