using System;

namespace Ynab_Sms.Logging
{
    /// <summary>
    /// Specifies the verbosity used to log to console
    /// </summary>
    public enum LoggingLevel
    {
        Basic = 0,
        Verbose = 1
    }

    /// <summary>
    /// Class to be used if anything in the app wants to log to console
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Logging level used for the app.
        /// Should be set at the beginning of the application's execution
        /// </summary>
        public static LoggingLevel Level = LoggingLevel.Basic;

        /// <summary>
        /// Log the message to the console only of logging is enabled at the specified level
        /// </summary>
        public static void Log(string message, LoggingLevel level = LoggingLevel.Basic)
        {
            if ((int)level > (int)Level)
                return;

            Console.WriteLine(message);
        }
    }
}