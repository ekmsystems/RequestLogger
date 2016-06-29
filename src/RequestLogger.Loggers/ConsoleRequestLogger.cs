using System;
using System.Collections.Generic;
using System.Text;

namespace RequestLogger.Loggers
{
    public class ConsoleRequestLogger : IRequestLogger
    {
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
            WriteKeyValuePair("Message", ex.Message);
            WriteKeyValuePair("StackTrace", ex.StackTrace);
        }

        private void WriteHeader(string header)
        {
            _configuration.ConsoleLogWriter.WriteLine(ConsoleColor.Cyan, header);
        }

        private void WriteKeyValuePair(string key, string value)
        {
            _configuration.ConsoleLogWriter.Write(ConsoleColor.Blue, key);
            _configuration.ConsoleLogWriter.Write(ConsoleColor.White, ":");
            _configuration.ConsoleLogWriter.Write(ConsoleColor.Yellow, value);
            _configuration.ConsoleLogWriter.WriteLine(ConsoleColor.Black, "");
        }

        private void WriteHeaderValues(IDictionary<string, string[]> headers)
        {
            _configuration.ConsoleLogWriter.WriteLine(ConsoleColor.Blue, "Headers");

            foreach (var key in headers.Keys)
            {
                _configuration.ConsoleLogWriter.WriteLine(ConsoleColor.Blue, "\t{0}", key);

                foreach (var value in headers[key])
                {
                    _configuration.ConsoleLogWriter.WriteLine(ConsoleColor.Yellow, "\t\t{0}", value);
                }
            }
        }

        private static string DecodeData(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
