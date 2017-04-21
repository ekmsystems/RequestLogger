using System.Web;

namespace RequestLogger.Web
{
    public abstract class BaseHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (o, e) => OnBeginRequest(new HttpContextWrapper(((HttpApplication) o).Context));
            context.EndRequest += (o, e) => OnEndRequest(new HttpContextWrapper(((HttpApplication) o).Context));
            context.Error += (o, e) => OnError(new HttpContextWrapper(((HttpApplication) o).Context));
        }

        public void Dispose()
        {
        }

        public virtual void OnBeginRequest(HttpContextBase context)
        {
        }

        public virtual void OnEndRequest(HttpContextBase context)
        {
        }

        public virtual void OnError(HttpContextBase context)
        {
        }
    }
}