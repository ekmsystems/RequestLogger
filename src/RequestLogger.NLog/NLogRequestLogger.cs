﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace RequestLogger.NLog
{
    public class NLogRequestLogger : IRequestLogger
    {
        private readonly NLogRequestLoggerConfiguration _configuration;

        public NLogRequestLogger(NLogRequestLoggerConfiguration configuration)
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
            info.Properties[_configuration.Keys.RequestHeaders] = FormatHeader(requestData.Headers);
            info.Properties[_configuration.Keys.RequestBody] = FormatContent(requestData.Content);
        }

        private void PopulateResponseProperties(ResponseData responseData, LogEventInfo info)
        {
            info.Properties[_configuration.Keys.StatusCode] = responseData.StatusCode;
            info.Properties[_configuration.Keys.ReasonPhrase] = responseData.ReasonPhrase;
            info.Properties[_configuration.Keys.ResponseHeaders] = FormatHeader(responseData.Headers);
            info.Properties[_configuration.Keys.ResponseBody] = FormatContent(responseData.Content);
        }

        private static string FormatHeader(IDictionary<string, string[]> header)
        {
            return string.Join(",", header
                .Select(x => string.Format("{{{0}: [{1}]}}", x.Key, string.Join("][", x.Value)))
                .ToArray());
        }

        private static string FormatContent(byte[] content)
        {
            return Encoding.UTF8.GetString(content ?? new byte[] {});
        }
    }
}