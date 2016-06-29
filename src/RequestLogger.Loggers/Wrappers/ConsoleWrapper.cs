using System;
using System.IO;

namespace RequestLogger.Loggers.Wrappers
{
    public interface ISystemConsole
    {
        ConsoleColor ForegroundColor { get; set; }
        TextWriter Error { get; }

        void Write(string format, params object[] args);
        void WriteLine(string format, params object[] args);
    }

    internal class ConsoleWrapper : ISystemConsole
    {
        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        public TextWriter Error
        {
            get { return Console.Error; }
        }

        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
