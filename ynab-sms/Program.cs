using System;
using System.Collections.Generic;

using CommandLine;

namespace Ynab_Sms
{
    /// <summary>
    /// Arguments to expect when the application is executed from the command line
    /// </summary>
    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Path to config file")]
        public string ConfigFilePath { get; set; }
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
            Runner.Run(options.ConfigFilePath);
        }

        /// <summary>
        /// Called when CommandLineParse failed to parse the command line arguments
        /// </summary>
        static void HandleParseError(IEnumerable<Error> errors)
        {
            Console.WriteLine("Error parsing command line arguments. Errors:\n");
            foreach (Error e in errors)
                Console.WriteLine(e.ToString());
        }
    }
}
