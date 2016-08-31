using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using RequestLogger.Loggers;

namespace RequestLogger.Owin
{
    public class RequestLoggerMiddleware : OwinMiddleware
    {
        private readonly IRequestLogger _logger;

        public RequestLoggerMiddleware(OwinMiddleware next)
            : this(new NullLogger(), next)
        {
        }

        public RequestLoggerMiddleware(IRequestLogger logger, OwinMiddleware next)
            : base(next)
        {
            _logger = logger;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var stream = context.Request.Body;
            var requestData = new RequestData();
            var responseData = new ResponseData();

            using (var ms = new MemoryStream())
            {
                try
                {
                    context.Response.Body = ms;

                    requestData = GetRequestData(context.Request);

                    await Next.Invoke(context);

                    responseData = GetResponseData(context.Response);

                    _logger.Log(requestData, responseData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(requestData, responseData, ex);
                }

                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(stream);
            }
        }

        private static RequestData GetRequestData(IOwinRequest request)
        {
            return new RequestData();
        }

        private static ResponseData GetResponseData(IOwinResponse response)
        {
            return new ResponseData();
        }
    }
}
