using System;
using System.Collections.Generic;

using CommandLine;
using Ynab_Sms.Logging;

namespace Ynab_Sms
{
    /// <summary>
    /// Arguments to expect when the application is executed from the command line
    /// </summary>
    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Path to config file")]
        public string ConfigFilePath { get; set; }

        [Option('v', "verbose", Required = false, Default = false, HelpText = "Run with verbose console logging")]
        public bool Verbose { get; set; }

        [Option('s', "send-sms", Required = false, Default = false, HelpText = "Send SMS messages")]
        public bool SendSms { get; set; }
    }

    class Program
    {
        /// <summary>
        /// Is what it is. Does what it does.
        /// </summary>
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        /// <summary>
        /// Called when CommandLineParse was able to successfully parse the command line arguments
        /// </summary>
        static void RunOptions(CommandLineOptions options)
        {
            Logger.Log("Running YNAB-SMS...");
            Runner.Run(options);
            Logger.Log("Success!");
        }

        /// <summary>
        /// Called when CommandLineParse failed to parse the command line arguments
        /// </summary>
        static void HandleParseError(IEnumerable<Error> errors)
        {
            Logger.Log("Error parsing command line arguments. Errors:\n");
            foreach (Error e in errors)
                Logger.Log(e.ToString());
        }
    }
}
