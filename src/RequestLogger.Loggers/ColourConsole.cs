using System;

namespace RequestLogger
{
    internal static class ColourConsole
    {
        public static void Write(ConsoleColor color, string format, params object[] args)
        {
            UseColor(color, () => Console.Write(format, args));
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            UseColor(color, () => Console.WriteLine(format, args));
        }

        public static void Error(ConsoleColor color, string format, params object[] args)
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
