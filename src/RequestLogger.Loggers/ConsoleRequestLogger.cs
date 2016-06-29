using System;
using System.Collections.Generic;
using System.Text;
using RequestLogger.Loggers.Wrappers;

namespace RequestLogger.Loggers
{
    public class ConsoleRequestLogger : IRequestLogger
    {
        private ISystemConsole SystemConsole
        {
            get { return _configuration.SystemConsole; }
        }

        private readonly ConsoleRequestLoggerConfiguration _configuration;

        public ConsoleRequestLogger(ConsoleRequestLoggerConfiguration configuration)
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
            WriteHeader("Request");
            WriteKeyValuePair("HttpMethod", requestData.HttpMethod);
            WriteKeyValuePair("Url", requestData.Url.ToString());
            WriteHeaderValues(requestData.Headers);
            WriteKeyValuePair("Content", DecodeData(requestData.Content));
        }

        private void LogResponseData(ResponseData responseData)
        {
            WriteHeader("Response");
            WriteKeyValuePair("StatusCode", Convert.ToString(responseData.StatusCode));
            WriteKeyValuePair("ReasonPhrase", responseData.ReasonPhrase);
            WriteHeaderValues(responseData.Headers);
            WriteKeyValuePair("Content", DecodeData(responseData.Content));
        }

        private void LogException(Exception ex)
        {
            WriteHeader("Error");
            WriteException(ex);
        }

        private void WriteHeader(string header)
        {
            UseColor(ConsoleColor.Cyan, () => SystemConsole.WriteLine(header));
        }

        private void WriteKeyValuePair(string key, string value)
        {
            UseColor(ConsoleColor.Blue, () => SystemConsole.Write(key));
            UseColor(ConsoleColor.White, () => SystemConsole.Write(": "));
            UseColor(ConsoleColor.Yellow, () => SystemConsole.Write(value));
            UseColor(ConsoleColor.Black, () => SystemConsole.WriteLine(""));
        }

        private void WriteHeaderValues(IDictionary<string, string[]> header)
        {
            UseColor(ConsoleColor.Blue, () => SystemConsole.WriteLine("Headers"));

            foreach (var key in header.Keys)
            {
                UseColor(ConsoleColor.Blue, () => SystemConsole.Write("\t{0}", key));
                UseColor(ConsoleColor.White, () => SystemConsole.Write(": "));
                UseColor(ConsoleColor.Blue, () => SystemConsole.WriteLine(string.Join(";", header[key])));
            }
        }

        private void WriteException(Exception ex)
        {
            UseColor(ConsoleColor.Red, () => SystemConsole.Error.WriteLine("{0}", ex));
        }

        private void UseColor(ConsoleColor color, Action action)
        {
            var pColor = SystemConsole.ForegroundColor;

            try
            {
                SystemConsole.ForegroundColor = color;

                action.Invoke();
            }
            finally
            {
                SystemConsole.ForegroundColor = pColor;
            }
        }

        private static string DecodeData(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
