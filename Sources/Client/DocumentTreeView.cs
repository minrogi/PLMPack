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
// threading
using System.Threading;
// log4net
using log4net;

#endregion

namespace PLMPackLibClient
{
    public partial class DocumentTreeView : TreeView
    {
        #region Constructor
        public DocumentTreeView()
        {
            InitializeComponent();

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentTreeView));
            // get image for tree
            ImageList = new ImageList();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(GetType());
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("CLSDFOLD"))); // 0
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("OPENFOLD"))); // 1
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("COMPONENT"))); // 2
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("DOC"))); // 3
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("DXF"))); // 4
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("PDF"))); // 5
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("AI"))); // 6
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("IMAGE"))); // 7
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("MSWORD"))); // 8
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("MSEXCEL"))); // 9
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("MSPPT"))); // 10
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("WRITER"))); // 11
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("CALC"))); // 12
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("ARD"))); // 13
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("PICADOR3D"))); // 14
            ImageList.Images.Add((System.Drawing.Image)(resources.GetObject("ROOT"))); // 15

            // events
            AfterExpand += new TreeViewEventHandler(DocumentTreeView_AfterExpand);
            AfterSelect += new TreeViewEventHandler(DocumentTreeView_AfterSelect);

            MouseDown += new MouseEventHandler(DocumentTreeView_MouseDown);

            this.AllowDrop = true;
            ItemDrag += new ItemDragEventHandler(DocumentTreeView_ItemDrag);
            DragEnter += new DragEventHandler(DocumentTreeView_DragEnter);
            DragOver += new DragEventHandler(DocumentTreeView_DragOver);
            DragDrop += new DragEventHandler(DocumentTreeView_DragDrop);
            NodeDropped += new NodeDroppedHandler(DocumentTreeView_NodeDropped);

            // show tool tips
            ShowNodeToolTips = true;
            // allow drag and drop
            AllowDrop = true;
        }
        #endregion

        #region Tree filling
        /// <summary>
        /// Method called by FormTreeView.Load method to avoid issues while in design mode
        /// </summary>
        public void InitializeTree()
        {
            if (this.DesignMode)
                return;
            // disable any redrawing of the tree view.
            BeginUpdate();

            _log.Info("Loading root node");
            // load
            NodeTag[] tags = NodeTag.GetRoot();

            foreach (NodeTag tag in tags)
            {
                TreeNode tnRoot = new TreeNode(tag.Name);
                tnRoot.ImageIndex = 15;
                tnRoot.SelectedImageIndex = 15;

                tnRoot.Tag = tag;
                tnRoot.Nodes.Add("_DUMMY_");
                this.Nodes.Add(tnRoot);
                tnRoot.Collapse();
            }

            // enable any redrawing of the tree view.
            EndUpdate();
        }
        /// <summary>
        /// PopulateChildren : fills tree using 
        /// </summary>
        /// <param name="parent">Tree node to be populated</param>
        private void PopulateChildren(TreeNode parent)
        {
            // remove _DUMMY_ tree node
            parent.Nodes.Clear();
            // populate with actual nodes 
            NodeTag tag = parent.Tag as NodeTag;
            NodeTag[] childTags = tag.Chidrens;
            foreach (NodeTag t in childTags)
            {
                TreeNode tn = new TreeNode(t.Name);
                tn.ImageIndex = TagToIndex(t);
                tn.SelectedImageIndex = TagToIndex(t);
                tn.Tag = t;
                parent.Nodes.Add(tn);
                if (t.HasChildren)
                {
                    tn.Nodes.Add(new TreeNode("_DUMMY_"));
                    tn.Collapse();
                }
                else
                    tn.Expand();
            }
        }

        private int TagToIndex(NodeTag nt)
        {
            switch (nt.Type)
            { 
                case NodeTag.NType.NTBranch:
                    return 0;
                case NodeTag.NType.NTDocument:
                    {
                        string ext = nt.Document.File.Extension.ToLower();
                        if (string.Equals(ext, "des")) return 3;
                        else if (string.Equals(ext, "dxf")) return 4;
                        else if (string.Equals(ext, "pdf")) return 5;
                        else if (string.Equals(ext, "ai")) return 6;
                        else if (string.Equals(ext, "jpg")
                            || string.Equals(ext, "bmp")
                            || string.Equals(ext, "jpg")
                            || string.Equals(ext, "jpeg")
                            || string.Equals(ext, "png")) return 7;
                        else if (string.Equals(ext, "doc")) return 8;
                        else if (string.Equals(ext, "xls")) return 9;
                        else if (string.Equals(ext, "ppt")) return 10;
                        else if (string.Equals(ext, "odt")) return 11;
                        else if (string.Equals(ext, "ods")) return 12;
                        else if (string.Equals(ext, "ard")) return 13;
                        else if (string.Equals(ext, "des3")) return 14;
                        else
                            return 0;
                    }
                case NodeTag.NType.NTComponent:
                    return 2;
            }
            return 0;
        }

        public TreeNode FindNode(TreeNode node, NodeTag tag)
        {
            // check with node itself
            if (null != node)
            {
                NodeTag tagNode = node.Tag as NodeTag;
                if (null != tagNode && tagNode.Equals(tag))
                    return node;
            }
            // check with child nodes
            TreeNodeCollection tnCollection = null == node ? Nodes : node.Nodes;
            foreach (TreeNode tn in tnCollection)
            {
                TreeNode tnResult = FindNode(tn, tag);
                if (null != tnResult)
                    return tnResult;
            }
            return null;
        }
        #endregion

        #region Selection
        public void SelectTag(NodeTag tag)
        { 
            // find node
            TreeNode tn = FindNode(null, tag);
            if (null != tn)
            {
                tn.Expand();
                // select node
                SelectedNode = tn;
            }
            else
                _log.Error(string.Format("Tag {0} has no treenode", tag.ToString()));
        }
        #endregion

        #region TreeView event handlers
        private void DocumentTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Nodes.Count == 1 && 0 == string.Compare(e.Node.Nodes[0].Text, "_DUMMY_"))
                {   // this node has not yet been populated
                    bool singlethreaded = true;
                    if (singlethreaded)
                        PopulateChildren(e.Node);
                    else // multithreaded
                    {
                        // launch a thread to get the data
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            // load the data into the tree view (on the UI thread)
                            BeginInvoke((Action)delegate
                            {
                                PopulateChildren(e.Node);
                            });
                        });
                    }
                }
                // select node
                SelectedNode = e.Node;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void DocumentTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                NodeTag currentTag = e.Node.Tag as NodeTag;
                if (null == currentTag)
                {
                }
                else if (null != SelectionChanged)
                    SelectionChanged(this, e, currentTag);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void DocumentTreeView_MouseDown(object sender, MouseEventArgs e)
        {
        }
        #endregion

        #region Drag&Drop handlers
        void DocumentTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                _draggedNode = e.Item as TreeNode;
                if (null != _draggedNode)
                    DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }
        void DocumentTreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // retrieve the client coordinates of the drop location.
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            // retrieve the node at the drop location.
            TreeNode dropTargetNode = GetNodeAt(targetPoint);
            if (null != dropTargetNode)
            {
            }
        }

        void DocumentTreeView_DragOver(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // retrieve the client coordinates of the mouse position.
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = GetNodeAt(targetPoint);
            if (null != targetNode)
            {
            }
        }

        void DocumentTreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (null == _draggedNode) return;
            // Set the target drop effect to the effect 
            // specified in the ItemDrag event handler.
            e.Effect = e.AllowedEffect;
        }

        void DocumentTreeView_NodeDropped(object sender, NodeDroppedArgs args)
        {
            try
            {
                if (null != NodeDropped)
                    NodeDropped(this, args);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Delegates
        public delegate void SelectionChangedHandler(object sender, EventArgs e, NodeTag tag);
        public delegate void NodeDroppedHandler(object sender, NodeDroppedArgs e);
        #endregion

        #region Events
        public event SelectionChangedHandler SelectionChanged;
        public event NodeDroppedHandler NodeDropped;
        #endregion

        #region Data members
        // logging
        static readonly ILog _log = LogManager.GetLogger(typeof(DocumentTreeView));
        // Drag & drop
        private TreeNode _draggedNode;
        #endregion
    }

    #region Event argument classes : NodeDroppedArgs
    public class NodeDroppedArgs : EventArgs
    {
        public NodeDroppedArgs(TreeNode nodeFrom, TreeNode nodeTo)
        { _treeNodeFrom = nodeFrom; _treeNodeTo = nodeTo; }
        public TreeNode _treeNodeFrom, _treeNodeTo;
    }
    #endregion


}
