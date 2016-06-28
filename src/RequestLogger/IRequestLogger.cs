using System;

namespace RequestLogger
{
    public interface IRequestLogger
    {
        void Log(RequestData requestData, ResponseData responseData);
        void LogError(RequestData requestData, ResponseData responseData, Exception ex);
    }
}
