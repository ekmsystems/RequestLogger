using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace RequestLogger.Web
{
    internal class RequestLogHandler
    {
        private readonly IRequestLogger _logger;

        public RequestLogHandler(IRequestLogger logger)
        {
            _logger = logger;
        }

        public void HandleBeginRequest(HttpContext context)
        {
            if (ShouldNotLog(context.Request))
                return;

            context.Response.Filter = new ResponseFilterStream(context.Response.Filter);
        }

        public void HandleEndRequest(HttpContext context)
        {
            if (ShouldNotLog(context.Request))
                return;

            var requestData = ExtractRequestData(context.Request);
            var responseData = ExtractResponseData(context.Response);

            _logger.Log(requestData, responseData);
        }

        public void HandleError(HttpContext context)
        {
            var requestData = ExtractRequestData(context.Request);
            var responseData = ExtractResponseData(context.Response);
            var ex = context.Server.GetLastError();

            _logger.LogError(requestData, responseData, ex);
        }

        private static bool ShouldNotLog(HttpRequest request)
        {
            var extension = (Path.GetExtension(request.CurrentExecutionFilePath) ?? "");

            return extension.ToLowerInvariant() == ".axd";
        }

        private static RequestData ExtractRequestData(HttpRequest request)
        {
            return new RequestData
            {
                HttpMethod = request.HttpMethod,
                Uri = request.Url.ToString(),
                Headers = ParseHeaders(request.Headers),
                Content = GetRequestContent(request)
            };
        }

        private static ResponseData ExtractResponseData(HttpResponse response)
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

        private static byte[] GetRequestContent(HttpRequest request)
        {
            if (!request.InputStream.CanSeek)
                return new byte[] { };

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

        private static byte[] GetResponseContent(HttpResponse response)
        {
            return ((ResponseFilterStream)response.Filter).ReadStream();
        }
    }
}
