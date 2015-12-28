#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// debug assert
using System.Diagnostics;
// log4net
using log4net;
using log4net.Config;

using System.Reflection;

using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;
#endregion

namespace PLMPackLibClient
{
    static class Program
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        #endregion

        #region Main
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set up a simple logging configuration
            XmlConfigurator.Configure();
            if (!LogManager.GetRepository().Configured)
                Debug.Fail("Logging not configured!\n Press ignore to continue");

            // set up a simple configuration
            try
            {
                // force CultureToUse culture if specified in config file
                string specifiedCulture = PLMPackLibClient.Properties.Settings.Default.CultureToUse;
                if (!string.IsNullOrEmpty(specifiedCulture))
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(specifiedCulture);
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(specifiedCulture);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("Specified culture in config file ({0}) appears to be invalid: {1}", specifiedCulture, ex.Message));
                    }
                }

                _log.Info(string.Format("Starting {0} with culture {1}", Application.ProductName, Thread.CurrentThread.CurrentUICulture));

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                new PLMPackLibClientApp().Run(args);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region PLMPackLibClientApp
        class PLMPackLibClientApp : WindowsFormsApplicationBase
        {
            protected override void OnCreateSplashScreen()
            {
                this.SplashScreen = new SplashScreen();
            }
            protected override void OnCreateMainForm()
            {
                // Then create the main form, the splash screen will close automatically
                this.MainForm = new FormMain();
            }
        }
        #endregion
    }
}
