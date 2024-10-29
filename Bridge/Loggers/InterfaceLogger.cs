using ProxyParser.Bridge.Loggers.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyParser.Bridge.Loggers
{
    public class InterfaceLogger : ILogger
    {
        private static string LogHistory = string.Empty;
        private int _logMemory = 50;


        //сделать автоудаление после 50 
        public string GetHistory()
        {
            if(LogHistory == string.Empty)
                return string.Empty;

            string export = string.Empty;

            string[] lines = LogHistory.Split('\n');

            int j = 0;

            for (int i = lines.Length - 1; i > 0; i--)
            {
                export = $"{lines[i]}\n{export}";
                j++;

                if (j >= _logMemory)
                    break;
            }

            return export;
        }

        public void Error(string message)
        {
            Log(LogLevel.ERROR, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.INFO, message);
        }

        public void Success(string message)
        {
            Log(LogLevel.SUCCESS, message);
        }

        public void Warning(string message)
        {
            Log(LogLevel.WARNING, message);
        }

        private void Log(LogLevel level, string message)
        {
            string article = $"[{level}] {message}";

            LogHistory += article + "\n";
        }

        [System.Flags]
        private enum LogLevel
        {
            TRACE,
            INFO,
            DEBUG,
            WARNING,
            ERROR,
            SUCCESS
        }


    }
}
