#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

using log4net;

using treeDiM.PLMPackLib;
#endregion

namespace TestLegacyDBUploader
{
    class Program
    {
        #region Command line parsing
        class Options
        {
            [Option('i', "inputfile", Required = true,
                HelpText = "Database file.")]
            public string InputFile { get; set; }

            [Option('u', "username", Required = true,
                HelpText = "User name.")]
            public string UserName { get; set; }

            [Option('p', "password", Required = true,
                HelpText = "User password.")]
            public string Password { get; set; }

            [Option('t', "test", Required = false,
                DefaultValue = false, HelpText = "Test : if true, files will not be actually uploaded.")]
            public bool Test { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }
        #endregion


        static int Main(string[] args)
        {
            try
            {
                // set up a simple configuration that logs on the console.
               log4net.Config.XmlConfigurator.Configure();
                // command line parsing
                Options options = new Options();
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Console.WriteLine(options.GetUsage());
                    return -1;
                }
                string dbPath = options.InputFile;
                _log.Info(string.Format("db path = {0}", dbPath));

                LegacyDBUploder uploader = new treeDiM.PLMPackLib.LegacyDBUploder();
                LoggingCallback callback = new LoggingCallback();
                uploader.DatabasePath = dbPath;
                uploader.UserName = options.UserName;
                uploader.Password = options.Password;
                uploader.ActuallyUpload = !options.Test;
                uploader.Upload(callback);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return -1;
            }
            return 0;
        }

        #region Static data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        #endregion
    }
}
