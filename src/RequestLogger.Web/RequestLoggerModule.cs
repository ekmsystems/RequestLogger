using System;
using System.Web;

namespace RequestLogger.Web
{
    public class RequestLoggerModule : IHttpModule
    {
        private readonly RequestLogHandler _handler;

        public RequestLoggerModule()
            : this(new NullLogger())
        {
        }

        public RequestLoggerModule(IRequestLogger logger)
        {
            _handler = new RequestLogHandler(logger);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextOnBeginRequest;
            context.EndRequest += ContextOnEndRequest;
            context.Error += ContextOnError;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            _handler.HandleBeginRequest(((HttpApplication) sender).Context);
        }

        private void ContextOnEndRequest(object sender, EventArgs eventArgs)
        {
            _handler.HandleEndRequest(((HttpApplication) sender).Context);
        }

        private void ContextOnError(object sender, EventArgs eventArgs)
        {
            _handler.HandleError(((HttpApplication) sender).Context);
        }
    }
}
