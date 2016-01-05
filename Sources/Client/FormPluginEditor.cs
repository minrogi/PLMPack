#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using PLMPackLibClient.Properties;
#endregion

namespace PLMPackLibClient
{
    /// <summary>
    /// Plugin editor form
    /// </summary>
    public partial class FormPluginEditor : Form
    {
        #region Constructor
        /// <summary>
        /// Form constructor
        /// </summary>
        public FormPluginEditor()
        {
            InitializeComponent();
            // set component search method
            _generatorCtrl.ComponentSearchMethod = new PLMPackLibClient.ComponentSearchMethodDB();
            _generatorCtrl.PluginValidated += new Pic.Plugin.GeneratorCtrl.GeneratorCtrl.GeneratorCtlrHandler(_generatorCtrl_PluginValidated);
            _generatorCtrl.setComponentDirectory(treeDiM.FileTransfer.FileTransferUtility.TempCacheDirectory);
        }
        #endregion

        #region Plugin generator control event handler
        /// <summary>
        /// Plugin validated handler
        /// </summary>
        void _generatorCtrl_PluginValidated(object sender, Pic.Plugin.GeneratorCtrl.GeneratorCtrlEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// input plugin path
        /// </summary>
        public string PluginPath
        {
            set
            {
                _generatorCtrl.setComponentFilePath(value);
                Text = string.Format(Properties.Resources.ID_EDITCOMPONENT, Path.GetFileName(value)); 
            }
        }
        /// <summary>
        /// output plugin path
        /// </summary>
        public string OutputPath
        {
            set { _generatorCtrl.OutputPath = value; }
            get { return _generatorCtrl.OutputPath; }
        }
        #endregion
    }
}
