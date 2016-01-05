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
using System.IO;
// logging
using log4net;
// Dock content
using WeifenLuo.WinFormsUI.Docking;

using Pic.Factory2D;
using Pic.Plugin;
using DesLib4NET;

using PLMPackLibClient.PLMPackSR;
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

            // cardboard format loader
            CardboardFormatLoader formatLoader = new CarboardFormatLoaderImpl();
            pluginViewCtrl.CardboardFormatLoader = formatLoader;
            factoryViewCtrl.CardBoardFormatLoader = formatLoader;

            // profile loader
            pluginViewCtrl.ProfileLoader = new ProfileLoaderImpl();

            // initialize plugin viewer
            pluginViewCtrl.SearchMethod = new ComponentSearchMethodDB();
        }

        void OnSelectionChanged(object sender, EventArgs e, NodeTag tag)
        {
            try
            {
                // change view name (seen in tab)
                this.Text = tag.Name;

                branchView.Visible = tag.IsBranch;
                pluginViewCtrl.Visible = false;
                factoryViewCtrl.Visible = false;
                webBrowser4PDF.Visible = false;

                if (tag.IsDocument || tag.IsComponent)
                {
                    string fileExt = tag.Document.File.Extension.ToLower();
                    string filePath = treeDiM.FileTransfer.FileTransferUtility.DownloadFile(tag.Document.File.Guid, fileExt);
                    bool isPdf = tag.IsDocument && string.Equals(tag.Document.File.Extension, ".pdf");

                    switch (fileExt)
                    {
                        case "dll":
                            LoadComponent(filePath);
                            break;
                        case "des":
                        case "dxf":
                        case "cf2":
                            LoadDrawing(filePath, fileExt);
                            break;
                        case "pdf":
                            LoadPdf(filePath);
                            break;
                        case "png":
                        case "bmp":
                        case "jpg":
                        case "jpeg":
                            LoadImageFile(filePath);
                            break;
                        case "des3":
                        case "doc":
                        case "xls":
                        case "dwg":
                        case "ai":
                        case "sxw":
                        case "stw":
                        case "sxc":
                        case "stc":
                        case "ard":
                        default:
                            LoadUnknownFileFormat(filePath);
                            break;
                    }
                }

                if (sender != branchView) branchView.ShowBranch(tag);
                if (sender != treeView) treeView.SelectTag(tag);
            }
            catch (treeDiM.FileTransfer.FileTransferException ex)
            {
                _log.Error(ex.Message); 
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); 
            }
        }
        #endregion

        #region File loading
        private void LoadDrawing(string filePath, string fileExt)
        {
            factoryViewCtrl.Visible = true;

            PicFactory factory = factoryViewCtrl.Factory;

            if (string.Equals("des", fileExt, StringComparison.CurrentCultureIgnoreCase))
            {
                PicLoaderDes picLoaderDes = new PicLoaderDes(factory);
                using (DES_FileReader fileReader = new DES_FileReader())
                    fileReader.ReadFile(filePath, picLoaderDes);
                // remove existing quotations
                factory.Remove((new PicFilterCode(PicEntity.eCode.PE_COTATIONDISTANCE))
                                    | (new PicFilterCode(PicEntity.eCode.PE_COTATIONHORIZONTAL))
                                    | (new PicFilterCode(PicEntity.eCode.PE_COTATIONVERTICAL)));
                // build autoquotation
                PicAutoQuotation.BuildQuotation(factory);
            }
            else if (string.Equals("dxf", fileExt, StringComparison.CurrentCultureIgnoreCase))
            {
                using (PicLoaderDxf picLoaderDxf = new PicLoaderDxf(factory))
                {
                    picLoaderDxf.Load(filePath);
                    picLoaderDxf.FillFactory();
                }
            }
            // update factoryViewCtrl
            factoryViewCtrl.FitView();        
        }
        private void LoadComponent(string filePath)
        {
            ComponentLoader loader = new ComponentLoader();
            pluginViewCtrl.PluginPath = filePath;
            pluginViewCtrl.Visible = true;
        }
        private void LoadImageFile(string filePath)
        {
            webBrowser4PDF.Url = new Uri(filePath);
            webBrowser4PDF.Size = this.splitContainer.Panel2.Size;
            webBrowser4PDF.Visible = true;
            webBrowser4PDF.Invalidate();
        }
        private void LoadPdf(string filePath)
        {
            webBrowser4PDF.Url = new Uri(filePath);
            webBrowser4PDF.Size = this.splitContainer.Panel2.Size;
            webBrowser4PDF.Visible = true;
            webBrowser4PDF.Invalidate();
        }
        private void LoadUnknownFileFormat(string filePath)
        {
            _log.Info(
                string.Format("Loading file: {0} ({1})"
                , Path.GetFileName(filePath)
                , File.Exists(filePath) ? "Exists" : "Does not exist"));
            /*
            // build new file path
            string filePathCopy = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));
            // copy file
            System.IO.File.Copy(filePath, filePathCopy, true);
            // open using shell execute 'Open'
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.Verb = "Open";
            startInfo.CreateNoWindow = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = filePath;
            using (Process p = new Process())
            {
                p.StartInfo = startInfo;
                p.Start();
            }
            */
        }
        #endregion

        #region ToolStrip event handlers
        public void onToolStripCotations(object sender, EventArgs e)
        {
            if (pluginViewCtrl.Visible)
                pluginViewCtrl.ShowCotations = !pluginViewCtrl.ShowCotations;
            else if (factoryViewCtrl.Visible)
                factoryViewCtrl.ShowCotations = !factoryViewCtrl.ShowCotations;
            UpdateToolCommands();
        }

        public void onToolStripReflectionX(object sender, EventArgs e)
        {
            if (pluginViewCtrl.Visible)
                pluginViewCtrl.ReflectionX = !pluginViewCtrl.ReflectionX;
            else if (factoryViewCtrl.Visible)
                factoryViewCtrl.ReflectionX = !factoryViewCtrl.ReflectionX;
            UpdateToolCommands();
        }

        public void onToolStripReflectionY(object sender, EventArgs e)
        {
            if (pluginViewCtrl.Visible)
                pluginViewCtrl.ReflectionY = !pluginViewCtrl.ReflectionY;
            else if (factoryViewCtrl.Visible)
                factoryViewCtrl.ReflectionY = !factoryViewCtrl.ReflectionY;
            UpdateToolCommands();
        }

        public void onToolStripLayout(object sender, EventArgs e)
        {
            try
            {
                if (pluginViewCtrl.Visible)
                    pluginViewCtrl.BuildLayout();
                else if (factoryViewCtrl.Visible)
                    factoryViewCtrl.BuildLayout();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        public void onToolStripEditComponentCode(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = treeView.SelectedNode.Tag as NodeTag;
                if (null == tag) return; // no valid tag
                if (null == tag.Document) return; // not a document
                string fileExt = tag.Document.File.Extension.ToLower();
                if (!string.Equals(fileExt, "dll")) return;  // not a component

                string filePath = treeDiM.FileTransfer.FileTransferUtility.DownloadFile(tag.Document.File.Guid, fileExt);

                // get client
                PLMPackServiceClient client = WCFClientSingleton.Instance.Client;

                // output Guid / path
                Guid outputGuid = Guid.NewGuid();
                string outputPath = treeDiM.FileTransfer.FileTransferUtility.BuildPath(outputGuid, fileExt);
                // form plugin editor
                FormPluginEditor editorForm = new FormPluginEditor();
                editorForm.PluginPath = filePath;
                editorForm.OutputPath = outputPath;
                if (DialogResult.OK == editorForm.ShowDialog())
                {
                    _log.Info("Component successfully modified!");
                    // clear component cache in plugin viewer
                    ComponentLoader.ClearCache();
                    // update pluginviewer
                    pluginViewCtrl.PluginPath = outputPath;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        public void onToolStripEditParameters(object sender, EventArgs e)
        {
        
        }

        private void UpdateToolCommands()
        { 
        }
        #endregion

        #region Data members
        private static readonly ILog _log = LogManager.GetLogger(typeof(FormTreeView));
        #endregion
    }
}
