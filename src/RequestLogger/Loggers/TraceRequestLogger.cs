using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RequestLogger.Loggers
{
    public class TraceRequestLogger : IRequestLogger
    {
        private readonly ITraceListener _traceListener;

        public TraceRequestLogger(ITraceListener traceListener)
        {
            _traceListener = traceListener;
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
            _traceListener.WriteLine("RequestData.HttpMethod: {0}", requestData.HttpMethod);
            _traceListener.WriteLine("RequestData.Url: {0}", requestData.Url);
            _traceListener.WriteLine("RequestData.Header: {0}", ParseHeader(requestData.Header));
            _traceListener.WriteLine("RequestData.Content: {0}", ParseContent(requestData.Content));
        }

        private void LogResponseData(ResponseData responseData)
        {
            _traceListener.WriteLine("ResponseData.StatusCode: {0}", responseData.StatusCode);
            _traceListener.WriteLine("ResponseData.ReasonPhrase: {0}", responseData.ReasonPhrase);
            _traceListener.WriteLine("ResponseData.Header: {0}", ParseHeader(responseData.Header));
            _traceListener.WriteLine("ResponseData.Content: {0}", ParseContent(responseData.Content));
        }

        private void LogException(Exception ex)
        {
            _traceListener.WriteError(ex);
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
