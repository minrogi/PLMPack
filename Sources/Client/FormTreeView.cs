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
// logging
using log4net;
// Dock content
using WeifenLuo.WinFormsUI.Docking;

using Pic.Plugin;
#endregion

namespace PLMPackLibClient
{
    public partial class FormTreeView : DockContent
    {
        #region Constructor
        public FormTreeView()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void FormTreeView_Load(object sender, EventArgs e)
        {
            treeView.InitializeTree();
            treeView.SelectionChanged += OnSelectionChanged;
            branchView.SelectionChanged += OnSelectionChanged;

            // initialize plugin viewer
            pluginViewCtrl.SearchMethod = new ComponentSearchMethodDB();
        }

        void OnSelectionChanged(object sender, EventArgs e, NodeTag tag)
        {
            try
            {
                this.Text = tag.Name;
                bool isPdf = tag.IsDocument && string.Equals(tag.Document.File.Extension, ".pdf");

                branchView.Visible = tag.IsBranch;
                factoryViewCtrl.Visible = false;
                pluginViewCtrl.Visible = tag.IsComponent;
                webBrowser4PDF.Visible = isPdf;

                if (tag.IsDocument)
                {
                    string filePath = treeDiM.FileTransfer.FileTransferUtility.DownloadFile(tag.Document.File.Guid, tag.Document.File.Extension);
                    if (isPdf) webBrowser4PDF.Url = new Uri(filePath);
                }
                else if (tag.IsComponent)
                {
                    string filePath = treeDiM.FileTransfer.FileTransferUtility.DownloadFile(tag.Document.File.Guid, tag.Document.File.Extension);
                    ComponentLoader loader = new ComponentLoader();
                    pluginViewCtrl.PluginPath = filePath;
                }

                if (sender != branchView) branchView.ShowBranch(tag);
                if (sender != treeView) treeView.SelectTag(tag);
            }
            catch (treeDiM.FileTransfer.FileTransferException ex)
            {   _log.Error(ex.Message); }
            catch (Exception ex)
            {   _log.Error(ex.ToString()); }
        }
        #endregion

        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormTreeView));
        #endregion
    }
}
