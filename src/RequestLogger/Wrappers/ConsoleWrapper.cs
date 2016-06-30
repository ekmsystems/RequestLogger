using System;

namespace RequestLogger.Wrappers
{
    public interface ISystemConsole
    {
        void Write(string format, params object[] args);
        void WriteLine(string format, params object[] args);
        void WriteError(Exception ex);
    }

    internal class ConsoleWrapper : ISystemConsole
    {
        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void WriteError(Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}
