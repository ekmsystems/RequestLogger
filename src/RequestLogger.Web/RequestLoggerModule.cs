using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using RequestLogger.Loggers;

namespace RequestLogger.Web
{
    public class RequestLoggerModule : BaseHttpModule
    {
        private readonly IRequestLogger _logger;

        public RequestLoggerModule()
            : this(new NullLogger())
        {
        }

        public RequestLoggerModule(IRequestLogger logger)
        {
            _logger = logger;
        }

        public override void OnBeginRequest(HttpContextBase context)
        {
            if (ShouldNotLog(context.Request))
                return;

            context.Response.Filter = new ResponseFilterStream(context.Response.Filter);
        }

        public override void OnEndRequest(HttpContextBase context)
        {
            if (ShouldNotLog(context.Request))
                return;

            var requestData = ExtractRequestData(context.Request);
            var responseData = ExtractResponseData(context.Response);

            _logger.Log(requestData, responseData);
        }

        public override void OnError(HttpContextBase context)
        {
            var requestData = ExtractRequestData(context.Request);
            var responseData = ExtractResponseData(context.Response);
            var ex = context.Server.GetLastError();

            _logger.LogError(requestData, responseData, ex);
        }

        private static bool ShouldNotLog(HttpRequestBase request)
        {
            var extension = (Path.GetExtension(request.CurrentExecutionFilePath) ?? "");

            return extension.ToLowerInvariant() == ".axd";
        }

        private static RequestData ExtractRequestData(HttpRequestBase request)
        {
            return new RequestData
            {
                HttpMethod = request.HttpMethod,
                Url = request.Url,
                Headers = ParseHeaders(request.Headers),
                Content = GetRequestContent(request)
            };
        }

        private static ResponseData ExtractResponseData(HttpResponseBase response)
        {
            return new ResponseData
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = response.StatusDescription,
                Headers = ParseHeaders(response.Headers),
                Content = GetResponseContent(response)
            };
        }

        private static IDictionary<string, string[]> ParseHeaders(NameValueCollection headers)
        {
            return headers.AllKeys.ToDictionary(x => x, headers.GetValues);
        }

        private static byte[] GetRequestContent(HttpRequestBase request)
        {
            if (!request.InputStream.CanSeek)
                return new byte[] {};

            try
            {
                using (var ms = new MemoryStream())
                {
                    request.InputStream.Seek(0, SeekOrigin.Begin);
                    request.InputStream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    return ms.ToArray();
                }
            }
            finally
            {
                request.InputStream.Seek(0, SeekOrigin.Begin);
            }
        }

        private static byte[] GetResponseContent(HttpResponseBase response)
        {
            return ((ResponseFilterStream) response.Filter).ReadStream();
        }
    }
}
