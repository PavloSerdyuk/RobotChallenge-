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
        public Owner Owner { get; set; }
        public string Message { get; set; }
        public LogValue Priority { get; set; }

        public LogEventArgs()
        {
        }

        public LogEventArgs(Owner owner, string message)
        {
            Owner = owner;
            Message = message;
            Priority = LogValue.Normal;
        }

        public LogEventArgs(Owner owner, string message, LogValue priority)
        {
            Owner = owner;
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
            Debug.WriteLine(string.Format( "ROUND NOMBER: {0}", roundNumber));
            LogRoundEventHandler handler = OnLogRound;
            if (handler != null) handler(null, new LogRoundEventArgs(){Number = roundNumber});
        }

        public static event LogEventHandler OnLogMessage;

        private static void LogMessage(LogEventArgs e)
        {
            Debug.WriteLine(e.Message);

            LogEventHandler handler = OnLogMessage;
            if (handler != null) handler(null, e);
        }

        public static void LogMessage(Owner owner, string message)
        {
            
            LogMessage(new LogEventArgs(owner, message));
        }

        public static void LogMessage(Owner owner, string message, LogValue priority)
        {
            LogMessage(new LogEventArgs(owner, message, priority));
        }
    }
}
