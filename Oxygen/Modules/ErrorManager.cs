using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxygen.Modules
{
    internal static class ErrorManager
    {
        internal enum LogType
        {
            Error,
            Warn,
            Info,
        }

        internal static void Error(string message,string location,string line="")
        {
            try
            {
                Global.ErrorConsole.AddLine(LogType.Error, message, location, line);
            }
            catch { }
            try
            {
                if (line != "") line = ":" + line;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(Console.BufferWidth - Math.Min(Console.BufferWidth / 2, location.Length + line.Length), Console.CursorTop);
                Console.Write((location + line).Substring(0, Math.Min(Console.BufferWidth / 2, location.Length + line.Length)));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                message = "[ERROR] " + message;
                for (int i = 0; i < (float)message.Length / (Console.BufferWidth - Math.Min(Console.BufferWidth / 2, location.Length + line.Length) - 2); i++)
                {
                    Console.WriteLine(message.Substring(i * (Console.BufferWidth - Math.Min(Console.BufferWidth / 2, location.Length + line.Length) - 2), Math.Min(message.Length - i * (Console.BufferWidth - Math.Min(Console.BufferWidth / 2, location.Length + line.Length) - 2), Console.BufferWidth - Math.Min(Console.BufferWidth / 2, location.Length + line.Length) - 2)));
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch { }
        }

    }
}
