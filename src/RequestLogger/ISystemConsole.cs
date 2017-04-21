using System;

namespace RequestLogger
{
    public interface ISystemConsole
    {
        void Write(string format, params object[] args);
        void WriteLine(string format, params object[] args);
        void WriteError(Exception ex);
    }
}