using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace RequestLogger.NLog
{
    public class NLogLogger : IRequestLogger
    {
        private readonly NLogLoggerConfiguration _configuration;

        public NLogLogger(NLogLoggerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Log(RequestData requestData, ResponseData responseData)
        {
            var info = new LogEventInfo
            {
                Level = LogLevel.Info,
                LoggerName = _configuration.Logger.Name
            };

            PopulateRequestProperties(requestData, info);
            PopulateResponseProperties(responseData, info);

            if (_configuration.BeforeLogHook != null)
                _configuration.BeforeLogHook(info);

            _configuration.Logger.Log(info);
        }

        public void LogError(RequestData requestData, ResponseData responseData, Exception ex)
        {
            var info = new LogEventInfo
            {
                Level = LogLevel.Error,
                LoggerName = _configuration.Logger.Name,
                Message = "An error occurred",
                Exception = ex
            };

            PopulateRequestProperties(requestData, info);
            PopulateResponseProperties(responseData, info);

            if (_configuration.BeforeLogErrorHook != null)
                _configuration.BeforeLogErrorHook(info);

            _configuration.Logger.Log(info);
        }

        private void PopulateRequestProperties(RequestData requestData, LogEventInfo info)
        {
            info.Properties[_configuration.Keys.HttpMethod] = requestData.HttpMethod;
            info.Properties[_configuration.Keys.Uri] = requestData.Url;
            info.Properties[_configuration.Keys.RequestHeader] = ParseHeader(requestData.Header);
            info.Properties[_configuration.Keys.RequestBody] = ParseContent(requestData.Content);
        }

        private void PopulateResponseProperties(ResponseData responseData, LogEventInfo info)
        {
            info.Properties[_configuration.Keys.StatusCode] = responseData.StatusCode;
            info.Properties[_configuration.Keys.ReasonPhrase] = responseData.ReasonPhrase;
            info.Properties[_configuration.Keys.ResponseHeader] = ParseHeader(responseData.Header);
            info.Properties[_configuration.Keys.ResponseBody] = ParseContent(responseData.Content);
        }

        private string ParseHeader(IDictionary<string, string[]> header)
        {
            return _configuration.HeaderFormatter.Format(header);
        }

        private static string ParseContent(byte[] content)
        {
            return Encoding.UTF8.GetString(content ?? new byte[] {});
        }
    }
}
