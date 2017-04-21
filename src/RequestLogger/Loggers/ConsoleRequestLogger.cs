using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RequestLogger.Loggers
{
    public class ConsoleRequestLogger : IRequestLogger
    {
        private readonly ISystemConsole _systemConsole;

        public ConsoleRequestLogger(ISystemConsole systemConsole = null)
        {
            _systemConsole = systemConsole;
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
            _systemConsole.WriteLine("RequestData.HttpMethod: {0}", requestData.HttpMethod);
            _systemConsole.WriteLine("RequestData.Url: {0}", requestData.Url);
            _systemConsole.WriteLine("RequestData.Header: {0}", ParseHeader(requestData.Header));
            _systemConsole.WriteLine("RequestData.Content: {0}", ParseContent(requestData.Content));
        }

        private void LogResponseData(ResponseData responseData)
        {
            _systemConsole.WriteLine("ResponseData.StatusCode: {0}", responseData.StatusCode);
            _systemConsole.WriteLine("ResponseData.ReasonPhrase: {0}", responseData.ReasonPhrase);
            _systemConsole.WriteLine("ResponseData.Header: {0}", ParseHeader(responseData.Header));
            _systemConsole.WriteLine("ResponseData.Content: {0}", ParseContent(responseData.Content));
        }

        private void LogException(Exception ex)
        {
            _systemConsole.WriteError(ex);
        }

        private static string ParseHeader(IDictionary<string, string[]> header)
        {
            var values = (header ?? new Dictionary<string, string[]>())
               .Select(x => string.Format("{0}: [{1}]", x.Key, string.Join(", ", x.Value)))
               .ToArray();

            return string.Join(", ", values);
        }

        private static string ParseContent(byte[] content)
        {
            return Encoding.UTF8.GetString(content ?? new byte[] {});
        }
    }
}
