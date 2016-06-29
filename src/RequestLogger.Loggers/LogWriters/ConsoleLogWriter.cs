using System;

namespace RequestLogger.Loggers.LogWriters
{
    public interface IConsoleLogWriter
    {
        void Write(ConsoleColor color, string format, params object[] args);
        void WriteLine(ConsoleColor color, string format, params object[] args);
        void Error(ConsoleColor color, string format, params object[] args);
    }

    public class ConsoleLogLogWriter : IConsoleLogWriter
    {
        public void Write(ConsoleColor color, string format, params object[] args)
        {
            UseColor(color, () => Console.Write(format, args));
        }

        public void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            UseColor(color, () => Console.WriteLine(format, args));
        }

        public void Error(ConsoleColor color, string format, params object[] args)
        {
            UseColor(color, () => Console.Error.WriteLine(format, args));
        }

        private static void UseColor(ConsoleColor color, Action action)
        {
            var pColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = color;

                action.Invoke();
            }
            finally
            {
                Console.ForegroundColor = pColor;
            }
        }
    }
}
