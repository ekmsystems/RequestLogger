using System;
using System.Diagnostics;

namespace RequestLogger.Wrappers
{
    internal class TraceWrapper : ITraceListener
    {
        public void WriteLine(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        public void WriteError(Exception ex)
        {
            Trace.TraceError("{0}", ex);
        }
    }
}
