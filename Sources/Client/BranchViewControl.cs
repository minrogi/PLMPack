#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// log4net
using log4net;
// File transfer utility
using treeDiM.FileTransfer;
#endregion

namespace PLMPackLibClient
{
    public partial class BranchViewControl : UserControl
    {
        #region Constants
        private const int cxButton = 150, cyButton = 150;   // image button size
        #endregion

        #region Data members
        protected static readonly ILog _log = LogManager.GetLogger(typeof(BranchViewControl));
        private Timer timer = new Timer();
        private ToolTip tooltip = new ToolTip();
        int i, x, y;
        NodeTag _currentNodeTag;
        #endregion

        #region Constructor
        public BranchViewControl()
        {
            InitializeComponent();

            // layout
            AutoScroll = true;
            timer.Interval = 1;
            timer.Tick += new EventHandler(timer_Tick);
        }
        #endregion

        #region Overrides
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AutoScrollPosition = Point.Empty;
            int x = 0, y = 0;
            foreach (Control cntl in Controls)
            {
                cntl.Location = new Point(x, y) + (Size)AutoScrollPosition;
                AdjustXY(ref x, ref y);
            }
        }
        private void AdjustXY(ref int x, ref int y)
        {
            x += cxButton;
            if (x + cxButton > Width - SystemInformation.VerticalScrollBarWidth)
            {
                x = 0;
                y += cyButton;
            }
        }
        #endregion

        #region Display methods
        public void ShowBranch(NodeTag nodeTag)
        {
            if (_currentNodeTag != nodeTag)
            {
                // save current node tag
                _currentNodeTag = nodeTag;
                // clear existing buttons
                Controls.Clear();
                tooltip.RemoveAll();
                i = x = y = 0;
                // start timer
                if ((null != _currentNodeTag) && _currentNodeTag.HasChildren)
                    timer.Start();
            }
        }
        #endregion

        #region Event handlers
        private void timer_Tick(object sender, EventArgs e)
        {
            NodeTag[] listTreeNodes = _currentNodeTag.Chidrens;
            if (i == listTreeNodes.GetLength(0))
            {
                timer.Stop();
                return;
            }

            Image image;
            SizeF sizef;
            try
            {
                image = Image.FromFile(FileTransferUtility.DownloadFile(listTreeNodes[i].Thumbnail.Guid, listTreeNodes[i].Thumbnail.Extension));
                sizef = new SizeF(image.Width / image.HorizontalResolution, image.Height / image.VerticalResolution);
                float fScale = Math.Min(cxButton / sizef.Width, cyButton / sizef.Height);
                sizef.Width *= fScale;
                sizef.Height *= fScale;
            }
            catch (FileTransferException ex)
            {
                _log.Error(ex.Message);
                ++i;
                return;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                ++i;
                return;
            }
            // convert image to small size for button
            Bitmap bitmap = new Bitmap(image, Size.Ceiling(sizef));
            image.Dispose();

            // create button and add to panel
            Button btn = new Button();
            btn.Image = bitmap;
            btn.Location = new Point(x, y) + (Size)AutoScrollPosition;
            btn.Size = new Size(cxButton, cyButton);
            btn.Tag = listTreeNodes[i];
            btn.Click += new EventHandler(btn_Click);
            Controls.Add(btn);

            // give button a tooltip
            tooltip.SetToolTip(btn, String.Format("{0}\n{1}", listTreeNodes[i].Name, listTreeNodes[i].Description));

            // adjust i, x and y for next image
            AdjustXY(ref x, ref y);
            ++i;
        }

        /// <summary>
        /// Handle user click on button and trigger event to other control
        /// </summary>
        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (null == btn) return;
                NodeTag tag = btn.Tag as NodeTag;
                ShowBranch(tag);
                // trigger event
                if (null != SelectionChanged)
                    SelectionChanged(this, e, tag);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Delegates
        public delegate void SelectionChangedHandler(object sender, EventArgs e, NodeTag tag);
        #endregion

        #region Events
        public event SelectionChangedHandler SelectionChanged;
        #endregion
    }
}
