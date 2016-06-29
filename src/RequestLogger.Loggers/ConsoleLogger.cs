using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RequestLogger.Loggers.Wrappers;

namespace RequestLogger.Loggers
{
    public class ConsoleLogger : IRequestLogger
    {
        private ISystemConsole SystemConsole
        {
            get { return _configuration.SystemConsole; }
        }

        private readonly ConsoleLoggerConfiguration _configuration;

        public ConsoleLogger(ConsoleLoggerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Log(RequestData requestData, ResponseData responseData)
        {
            LogRequestData(requestData);
            LogResponseData(responseData);
        }

        public void LogError(RequestData requestData, ResponseData responseData, Exception ex)
        {
            LogRequestData(requestData);
            LogResponseData(responseData);
            LogException(ex);
        }

        private void LogRequestData(RequestData requestData)
        {
            SystemConsole.WriteLine("RequestData.HttpMethod: {0}", requestData.HttpMethod);
            SystemConsole.WriteLine("RequestData.Url: {0}", requestData.Url);
            SystemConsole.WriteLine("RequestData.Header: {0}", ParseHeader(requestData.Header));
            SystemConsole.WriteLine("RequestData.Content: {0}", ParseContent(requestData.Content));
        }

        private void LogResponseData(ResponseData responseData)
        {
            SystemConsole.WriteLine("ResponseData.StatusCode: {0}", responseData.StatusCode);
            SystemConsole.WriteLine("ResponseData.ReasonPhrase: {0}", responseData.ReasonPhrase);
            SystemConsole.WriteLine("ResponseData.Header: {0}", ParseHeader(responseData.Header));
            SystemConsole.WriteLine("ResponseData.Content: {0}", ParseContent(responseData.Content));
        }

        private void LogException(Exception ex)
        {
            SystemConsole.WriteError(ex);
        }

        private static string ParseHeader(IDictionary<string, string[]> header)
        {
            var headerValues = string.Join(", ", (header ?? new Dictionary<string, string[]>()).Keys
                .Select(x => string.Format("{0}: [{1}]", x, string.Join(", ", header[x])))
                .ToArray());

            return string.Format("{{{0}}}", headerValues);
        }

        private static string ParseContent(byte[] content)
        {
            return Encoding.UTF8.GetString(content ?? new byte[] {});
        }
    }
}
