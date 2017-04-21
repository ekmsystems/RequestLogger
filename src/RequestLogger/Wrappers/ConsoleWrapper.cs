using System;

namespace RequestLogger.Wrappers
{
    internal class ConsoleWrapper : ISystemConsole
    {
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
