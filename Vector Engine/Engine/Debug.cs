using System;
using System.Diagnostics;

namespace VectorEngine.Engine
{
    public static class Debug
    {
        public static void Log(object toLog)
        {
            ChangeConsoleColor(ConsoleColor.Gray);
            StackFrame frame = new StackFrame(1, true);

            // Weird behaviour can happen if an attempt to log is done when the application
            // is closing, this prevents that issue. A better solution should be implemented
            // though.
            if (frame == null || frame.GetFileName() == null)
                return;

            string[] splitFilePath = frame.GetFileName().Split('\\');
            string fileName = splitFilePath[splitFilePath.Length - 1];
            string callInfo = fileName + ", " + frame.GetMethod().ToString() + " : " + frame.GetFileLineNumber();

            Console.WriteLine($"Log -> {callInfo} - {toLog.ToString()}");
        }

        private static void ChangeConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
}
