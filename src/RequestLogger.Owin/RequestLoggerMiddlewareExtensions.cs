using Owin;

namespace RequestLogger.Owin
{
    public static class RequestLoggerMiddlewareExtensions
    {
        public static IAppBuilder UseRequestLoggerMiddleware(this IAppBuilder app, IRequestLogger logger)
        {
            return app.Use(typeof(RequestLoggerMiddleware), logger);
        }
    }
}
