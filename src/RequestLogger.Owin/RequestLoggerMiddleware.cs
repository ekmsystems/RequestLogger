using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace RequestLogger.Owin
{
    public class RequestLoggerMiddleware : OwinMiddleware
    {
        private readonly IRequestLogger _logger;

        public RequestLoggerMiddleware(OwinMiddleware next, IRequestLogger logger)
            : base(next)
        {
            _logger = logger;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var requestData = new RequestData();
            var responseData = new ResponseData();
            var requestStream = await CreateSeekableStream(context.Request.Body);
            var responseStream = context.Response.Body;

            context.Request.Body = requestStream;

            using (var copyStream = new MemoryStream())
            {
                try
                {
                    context.Response.Body = copyStream;

                    requestData = GetRequestData(context.Request);

                    await Next.Invoke(context);

                    responseData = GetResponseData(context.Response);

                    _logger.Log(requestData, responseData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(requestData, responseData, ex);
                }

                requestStream.Seek(0, SeekOrigin.Begin);
                copyStream.Seek(0, SeekOrigin.Begin);

                await copyStream.CopyToAsync(responseStream);
            }
        }

        private static async Task<Stream> CreateSeekableStream(Stream stream)
        {
            var ms = new MemoryStream();
            
            if (stream != null)
            {
                await stream.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
            }

            return ms;
        }

        private static RequestData GetRequestData(IOwinRequest request)
        {
            return new RequestData
            {
                HttpMethod = request.Method,
                Url = request.Uri,
                Header = ExtractHeaders(request.Headers),
                Content = ExtractContent(request.Body)
            };
        }

        private static ResponseData GetResponseData(IOwinResponse response)
        {
            return new ResponseData
            {
                StatusCode = response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                Header = ExtractHeaders(response.Headers),
                Content = ExtractContent(response.Body)
            };
        }

        private static IDictionary<string, string[]> ExtractHeaders(IHeaderDictionary headers)
        {
            return headers.ToDictionary(x => x.Key, y => y.Value);
        }

        private static byte[] ExtractContent(Stream stream)
        {
            return (stream as MemoryStream ?? new MemoryStream()).ToArray();
        }
    }
}
