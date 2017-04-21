using System;

namespace RequestLogger
{
    public interface ISystemConsole
    {
        void WriteLine(string format, params object[] args);
        void WriteError(Exception ex);
    }
}