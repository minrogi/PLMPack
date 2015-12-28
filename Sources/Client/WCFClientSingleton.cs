#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// logging
using log4net;
// web service
using PLMPackLibClient.PLMPackSR;
#endregion

namespace PLMPackLibClient
{
    public class WCFClientSingleton
    {
        #region Private constructor (Singleton)
        private WCFClientSingleton()
        {
        }
        #endregion

        #region Accessing songleton instance & properties
        public static WCFClientSingleton Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new WCFClientSingleton();
                    _instance._client = new PLMPackServiceClient();
                    while (!_instance.Connect()) { }
                    if (null == _instance._user)
                        Application.Exit();
                }
                return _instance;
            }
        }
        public PLMPackServiceClient Client
        { get { return _client; } }
        public DCUser User
        { get { return _user; } }
        #endregion

        #region Connection
        private bool Connect()
        {
            FormLogin form = new FormLogin();
            form.UserName = Properties.Settings.Default.CredentialUserName;
            form.Password = Properties.Settings.Default.CredentialPassword;
            if (DialogResult.OK == form.ShowDialog())
            {
                try
                {
                    // try to connect using user credentials
                    _client.ClientCredentials.UserName.UserName = form.UserName;
                    _client.ClientCredentials.UserName.Password = form.Password;
                    _user = _client.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed!");
                    _log.Error(ex.ToString());
                    return false;
                }
                Properties.Settings.Default.CredentialUserName = form.UserName;
                Properties.Settings.Default.CredentialPassword = form.Password;
                Properties.Settings.Default.Save();

                _log.Info(string.Format("User connected as {0}", _user.Name));
                return true;
            }
            else
            {
                _log.Info("User cancelled connection!");
                return true;
            }
        }
        public void Disconnect()
        {
            _client.DisConnect();
        }
        public static bool Connected
        {
            get { return null != Instance.User; }
        }
        #endregion

        #region Data members
        // non static data members
        private PLMPackSR.PLMPackServiceClient _client;
        private DCUser _user;
        // static data members
        protected static WCFClientSingleton _instance;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(WCFClientSingleton));
        #endregion
    }
}
