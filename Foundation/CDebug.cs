using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aogood.Foundation
{
    public static class CDebug
    {

        public static void Log(string log)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Log(string log, params object[] arg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(log, arg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void LogError(string logError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logError);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void LogError(string logError, params object[] arg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(logError, arg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
