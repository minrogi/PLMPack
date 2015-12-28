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

using System.Diagnostics;

using log4net;
#endregion

namespace PLMPackLibClient
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        public string UserName
        {
            set { tbUserName.Text = value; }
            get { return tbUserName.Text; }
        }

        public string Password
        {
            set { tbPassword.Text = value; }
            get { return tbPassword.Text; }
        }

        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormLogin));
        #endregion

        private void bnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.RegisterURL);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
    }
}
