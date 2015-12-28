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
#endregion

namespace PLMPackLibClient
{
    public partial class SplashScreen : Form
    {
        #region Data members
        private bool _transparent = false;
        #endregion

        #region Constructor
        public SplashScreen()
        {
            InitializeComponent();

            if (null != this.BackgroundImage)
            {
                string rebrandedVersion = PLMPackLibClient.Properties.Settings.Default.UseRebrandedVersion;
                Bitmap b = null;
                if (string.Equals(rebrandedVersion, "CANON", StringComparison.CurrentCultureIgnoreCase))
                    b = PLMPackLibClient.Properties.Resources.splash_canon;
                else if (string.Equals(rebrandedVersion, "ARISTO", StringComparison.CurrentCultureIgnoreCase))
                    b = PLMPackLibClient.Properties.Resources.splash_aristo;
                else if (string.Equals(rebrandedVersion, "ZUND", StringComparison.CurrentCultureIgnoreCase))
                    b = PLMPackLibClient.Properties.Resources.splash_zund;
                else
                    b = PLMPackLibClient.Properties.Resources.splashpic;
                // make lower right pixel color transparent
                if (Transparent)
                    b.MakeTransparent(b.GetPixel(1, 1));
                this.BackgroundImage = b;
            }

            // version
            lblVersion.Text = String.Format("Version {0}", AssemblyVersion);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// retrieves assembly version
        /// </summary>
        public string AssemblyVersion
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        /// <summary>
        /// set / get transparency
        /// </summary>
        public bool Transparent
        {
            get { return _transparent; }
            set { _transparent = value; }
        }
        #endregion
    }
}
