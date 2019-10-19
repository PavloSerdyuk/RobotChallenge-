using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Robot.Tournament")]
namespace Robot.Common
{
    public enum LogValue { High, Normal, Low, Error }


    public class LogRoundEventArgs
    {
        //made setter private so student won't be able to change this value for students that recieve event after him
        public int Number { get; private set; }
        public LogRoundEventArgs(int number)
        {
            this.Number = number;
        }
    }

    public class LogEventArgs
    {
        //made setter for these private so student won't be able to change this value for students that recieve event after him
        public string OwnerName { get; private set; }
        public string Message { get; private set; }
        public LogValue Priority { get; private set; }

        internal LogEventArgs(string owner, string message, LogValue priority = LogValue.Normal)
        {
            OwnerName = owner;
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
            OnLogRound?.Invoke(null, new LogRoundEventArgs(roundNumber));
        }

        public static event LogEventHandler OnLogMessage;

        private static void LogMessage(LogEventArgs e)
        {
            Debug.WriteLine(e.Message);
            OnLogMessage?.Invoke(null, e);
        }


        internal static void LogMessage(string owner, string message, LogValue priority = LogValue.Normal)
        {
            LogMessage(new LogEventArgs(owner, message, priority));
        }

    }
}
