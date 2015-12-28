#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
// log4net
using log4net;
// docking
using WeifenLuo.WinFormsUI.Docking;
// file transfer utility
using treeDiM.FileTransfer;
// Properties
using PLMPackLibClient.Properties;

//using Microsoft.IdentityModel.Clients.ActiveDirectory;
#endregion

namespace PLMPackLibClient
{
    public partial class FormMain : Form
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormMain));
        private FormLogConsole _logConsole = new FormLogConsole();
        private FormStartPage _formStartPage = new FormStartPage();
        #endregion

        #region Constructor
        public FormMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Docking
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Set DockPanel properties
            dockPanel.DocumentStyle = DocumentStyle.DockingMdi;
            dockPanel.ActiveAutoHideContent = null;
            dockPanel.Parent = this;
            dockPanel.SuspendLayout(true);

            ShowLogConsole();
            dockPanel.ResumeLayout(true, true);

            if (IsWebSiteReachable)
            {
                ShowStartPage();
                timerLogin.Start();
            }
            else
                MessageBox.Show(Properties.Resources.ID_WEBCONNECTIONFAILED);
        }

        private void CreateBasicLayout()
        { 
            FormTreeView formTV = new FormTreeView();
            formTV.CloseButtonVisible = false;
            formTV.Show(dockPanel, DockState.Document);
            dockPanel.ResumeLayout(true, true);
        }

        public void ShowLogConsole()
        {
            // show or hide log console ?
            if (AssemblyConf == "debug" || Settings.Default.DebugMode)
                _logConsole.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            else
                _logConsole.Hide();
        }
        public void ShowStartPage()
        {
            if (!IsWebSiteReachable || null == _formStartPage)
                return;
            _formStartPage.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            _formStartPage.Url = new System.Uri(Settings.Default.StartPageUrl);
        }
        private void CloseStartPage()
        {
            if (null != _formStartPage)
                _formStartPage.Close();
            _formStartPage = null;
        }
        private void timerLogin_Tick(object sender, EventArgs e)
        {
            timerLogin.Stop();
            
            // show login form
            if (WCFClientSingleton.Connected)
            {
                // update main frame title
                Text += " - (" + WCFClientSingleton.Instance.User.Name + ")";
                // create basic layout
                CreateBasicLayout();
            }
        }
        #endregion

        #region Menu handlers
        private void clearFileCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try {   FileTransferUtility.ClearFileCache();  }
            catch (Exception ex)  { _log.Error(ex.Message); }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAboutBox form = new FormAboutBox();
            form.ShowDialog();
        }
        #endregion

        #region Helpers
        public bool IsWebSiteReachable
        {
            get
            {
                try
                {
                    System.Uri uri = new System.Uri(Settings.Default.StartPageUrl);
                    System.Net.IPHostEntry objIPHE = System.Net.Dns.GetHostEntry(uri.DnsSafeHost);
                    return true;
                }
                catch (System.Net.Sockets.SocketException /*ex*/)
                {
                    _log.Info(
                        string.Format(
                        "Url '{0}' could not be accessed : is the computer connected to the web?"
                        , Settings.Default.StartPageUrl
                        ));
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    return false;
                }
            }
        }
        public string AssemblyConf
        {
            get
            {
                object[] attributes = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyConfigurationAttribute confAttribute = (AssemblyConfigurationAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!string.IsNullOrEmpty(confAttribute.Configuration))
                        return confAttribute.Configuration.ToLower();
                }
                return "release";
            }
        }
        #endregion
    }
}
