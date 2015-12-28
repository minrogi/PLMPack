namespace PLMPackLibClient
{
    partial class FormTreeView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new PLMPackLibClient.DocumentTreeView();
            this.webBrowser4PDF = new System.Windows.Forms.WebBrowser();
            this.branchView = new PLMPackLibClient.BranchViewControl();
            this.factoryViewCtrl = new Pic.Factory2D.Control.FactoryViewerBase();
            this.pluginViewCtrl = new Pic.Plugin.ViewCtrl.PluginViewCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pluginViewCtrl)).BeginInit();
            this.pluginViewCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.webBrowser4PDF);
            this.splitContainer.Panel2.Controls.Add(this.branchView);
            this.splitContainer.Panel2.Controls.Add(this.factoryViewCtrl);
            this.splitContainer.Panel2.Controls.Add(this.pluginViewCtrl);
            this.splitContainer.Size = new System.Drawing.Size(784, 562);
            this.splitContainer.SplitterDistance = 172;
            this.splitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.ShowNodeToolTips = true;
            this.treeView.Size = new System.Drawing.Size(172, 562);
            this.treeView.TabIndex = 0;
            // 
            // webBrowser4PDF
            // 
            this.webBrowser4PDF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser4PDF.Location = new System.Drawing.Point(0, 0);
            this.webBrowser4PDF.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser4PDF.Name = "webBrowser4PDF";
            this.webBrowser4PDF.Size = new System.Drawing.Size(608, 562);
            this.webBrowser4PDF.TabIndex = 1;
            // 
            // branchView
            // 
            this.branchView.AutoScroll = true;
            this.branchView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.branchView.Location = new System.Drawing.Point(0, 0);
            this.branchView.Name = "branchView";
            this.branchView.Size = new System.Drawing.Size(608, 562);
            this.branchView.TabIndex = 0;
            // 
            // factoryViewCtrl
            // 
            this.factoryViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.factoryViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.factoryViewCtrl.Name = "factoryViewCtrl";
            this.factoryViewCtrl.ReflectionX = false;
            this.factoryViewCtrl.ReflectionY = false;
            this.factoryViewCtrl.ShowAboutMenu = false;
            this.factoryViewCtrl.ShowCotations = false;
            this.factoryViewCtrl.ShowNestingMenu = false;
            this.factoryViewCtrl.Size = new System.Drawing.Size(608, 562);
            this.factoryViewCtrl.TabIndex = 2;
            // 
            // pluginViewCtrl
            // 
            this.pluginViewCtrl.CloseButtonVisible = false;
            this.pluginViewCtrl.Component = null;
            this.pluginViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pluginViewCtrl.HasDependancies = false;
            this.pluginViewCtrl.Localizer = null;
            this.pluginViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.pluginViewCtrl.Name = "pluginViewCtrl";
            // 
            // pluginViewCtrl.Panel2
            // 
            this.pluginViewCtrl.Panel2.AutoScroll = true;
            this.pluginViewCtrl.ParamValues = null;
            this.pluginViewCtrl.ReflectionX = false;
            this.pluginViewCtrl.ReflectionY = false;
            this.pluginViewCtrl.ShowCotations = true;
            this.pluginViewCtrl.ShowSummary = false;
            this.pluginViewCtrl.Size = new System.Drawing.Size(608, 562);
            this.pluginViewCtrl.SplitterDistance = 358;
            this.pluginViewCtrl.SplitterWidth = 1;
            this.pluginViewCtrl.TabIndex = 0;
            this.pluginViewCtrl.ValidateButtonVisible = false;
            // 
            // FormTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormTreeView";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.FormTreeView_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pluginViewCtrl)).EndInit();
            this.pluginViewCtrl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private DocumentTreeView treeView;
        private BranchViewControl branchView;
        private Pic.Factory2D.Control.FactoryViewerBase factoryViewCtrl;
        private Pic.Plugin.ViewCtrl.PluginViewCtrl pluginViewCtrl;
        private System.Windows.Forms.WebBrowser webBrowser4PDF;
    }
}