using Owin;

namespace RequestLogger.Owin
{
    public static class RequestLoggerMiddlewareExtensions
    {
        public static IAppBuilder UseRequestLoggerMiddleware(this IAppBuilder app, IRequestLogger requestLogger)
        {
            return app.Use(typeof(RequestLoggerMiddleware), requestLogger);
        }
    }
}
