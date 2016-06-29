using System;
using System.Collections.Generic;
using System.Text;

namespace RequestLogger
{
    public class ConsoleRequestLogger : IRequestLogger
    {
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

        private static void LogRequestData(RequestData requestData)
        {
            WriteHeader("Request");
            WriteKeyValuePair("HttpMethod", requestData.HttpMethod);
            WriteKeyValuePair("Url", requestData.Url.ToString());
            WriteHeaderValues(requestData.Headers);
            WriteKeyValuePair("Content", DecodeData(requestData.Content));
        }

        private static void LogResponseData(ResponseData responseData)
        {
            WriteHeader("Response");
            WriteKeyValuePair("StatusCode", Convert.ToString(responseData.StatusCode));
            WriteKeyValuePair("ReasonPhrase", responseData.ReasonPhrase);
            WriteHeaderValues(responseData.Headers);
            WriteKeyValuePair("Content", DecodeData(responseData.Content));
        }

        private static void LogException(Exception ex)
        {
            WriteHeader("Error");
            WriteKeyValuePair("Message", ex.Message);
            WriteKeyValuePair("StackTrace", ex.StackTrace);
        }

        private static void WriteHeader(string header)
        {
            ColourConsole.WriteLine(ConsoleColor.Cyan, header);
        }

        private static void WriteKeyValuePair(string key, string value)
        {
            ColourConsole.Write(ConsoleColor.Blue, key);
            ColourConsole.Write(ConsoleColor.White, ":");
            ColourConsole.Write(ConsoleColor.Yellow, value);
            ColourConsole.WriteLine(ConsoleColor.Black, "");
        }

        private static void WriteHeaderValues(IDictionary<string, string[]> headers)
        {
            ColourConsole.WriteLine(ConsoleColor.Blue, "Headers");

            foreach (var key in headers.Keys)
            {
                ColourConsole.WriteLine(ConsoleColor.Blue, "\t{0}", key);

                foreach (var value in headers[key])
                {
                    ColourConsole.WriteLine(ConsoleColor.Yellow, "\t\t{0}", value);
                }
            }
        }

        private static string DecodeData(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
