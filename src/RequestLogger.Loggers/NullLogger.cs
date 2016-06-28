using System;

namespace RequestLogger
{
    public sealed class NullLogger : IRequestLogger
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
