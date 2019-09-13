using System;
using System.Diagnostics;

namespace Robot.Common
{
    public enum LogValue { High, Normal, Low, Error }


    public class LogRoundEventArgs
    {
        public int Number { get; set; }
    }

    public class LogEventArgs
    {
        public string OwnerName { get; set; }
        public string Message { get; set; }
        public LogValue Priority { get; set; }

        public LogEventArgs()
        {
        }

        public LogEventArgs(string owner, string message)
        {
            OwnerName = owner;
            Message = message;
            Priority = LogValue.Normal;
        }

        public LogEventArgs(string owner, string message, LogValue priority)
        {
            OwnerName  = owner;
            Message = message;
            Priority = priority;
        }
    }

    public delegate void LogEventHandler(object sender, LogEventArgs e);
    public delegate void LogRoundEventHandler(object sender, LogRoundEventArgs e);

    public static class Logger
    {
        public static event LogRoundEventHandler OnLogRound;

        public static void LogRound(int roundNumber)
        {
            Debug.WriteLine($"ROUND NOMBER: {roundNumber}");
            OnLogRound?.Invoke(null, new LogRoundEventArgs() { Number = roundNumber });
        }

        public static event LogEventHandler OnLogMessage;

        private static void LogMessage(LogEventArgs e)
        {
            Debug.WriteLine(e.Message);
            OnLogMessage?.Invoke(null, e);
        }

        
        public static void LogMessage(string owner, string message, LogValue priority = LogValue.Normal)
        {
            LogMessage(new LogEventArgs(owner, message, priority));
        }

    }
}
