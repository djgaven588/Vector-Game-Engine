using System;
using System.Diagnostics;

namespace VectorEngine.Engine
{
    public static class Debug
    {
        public static void Log(object toLog)
        {
            ChangeConsoleColor(ConsoleColor.Gray);
            string callInfo = GetCallerInfo(GetStackInfo());
            Console.WriteLine($"Log -> {callInfo} - {toLog.ToString()}");
        }

        private static void ChangeConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private static StackFrame GetStackInfo()
        {
            StackFrame stackFrame = new StackFrame(1, true);

            return stackFrame;
        }

        private static string GetCallerInfo(StackFrame frame)
        {
            string[] splitFilePath = frame.GetFileName().Split('\\');
            string fileName = splitFilePath[splitFilePath.Length - 1];
            return fileName + ", " + frame.GetMethod().ToString() + " : " + frame.GetFileLineNumber();
        }
    }
}
