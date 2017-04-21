using System;

namespace RequestLogger.Loggers
{
    public sealed class NullRequestLogger : IRequestLogger
    {
        public void Log(RequestData requestData, ResponseData responseData)
        {
            // Do Nothing
        }

        public void LogError(RequestData requestData, ResponseData responseData, Exception ex)
        {
            // Do Nothing
        }
    }
}
