using System;

namespace RequestLogger
{
    public interface ITraceListener
    {
        void WriteLine(string format, params object[] args);
        void WriteError(Exception ex);
    }
}
