#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// log4net
using log4net;
// WCF service / data contracts
using PLMPackLibClient.PLMPackSR;
#endregion

namespace PLMPackLibClient
{
    #region FileTag
    public class FileTag
    {
        #region Constructor
        public FileTag(Guid guid, string ext)
        {
            _guid = guid;
            _ext = ext;
        }
        #endregion

        #region Public properties
        public Guid Guid { get { return _guid; } }
        public string Extension { get { return _ext; } }
        public string FileName { get { return _guid.ToString().Replace("-", "_") + _ext; } }
        #endregion

        #region Data members
        private Guid _guid;
        private string _ext;
        #endregion
    }
    #endregion

    #region DocumentTag
    public class DocumentTag
    {
        #region Data members
        private Guid _dbGuid;
        private FileTag _dbDocumentFile;
        #endregion

        #region Constructor
        public DocumentTag(Guid guid, Guid fileGuid, string fileExt)
        {
            _dbGuid = guid;
            _dbDocumentFile = new FileTag(fileGuid, fileExt);
        }
        #endregion

        #region Public properties
        public FileTag File
        {
            get { return _dbDocumentFile; }
        }
        #endregion
    }
    #endregion

    #region ComponentTag
    public class ComponentTag : DocumentTag
    {
        public ComponentTag(Guid guid, Guid fileGuid, string fileExt) : base(guid, fileGuid, fileExt)
        { 
        }
    }
    #endregion

    #region NodeTag
    public class NodeTag
    {
        #region Enums
        public enum NType
        { 
            NTBranch = 0,
            NTDocument = 1,
            NTComponent = 2
        }
        #endregion

        #region Data members
        private Guid _dbNodeId;
        private string _dbNodeName, _dbNodeDescription;
        private bool _dbHasChild;
        private NType _dbType;
        // parent
        private NodeTag _dbParent;
        // childrens
        private List<NodeTag> _dbChildrens;
        // thumbnail
        FileTag _dbThumbFile;
        // document
        DocumentTag _dbDocument;
        #endregion

        #region Private constructor
        private NodeTag(NodeTag parent, Guid dbNodeId
            , string dbNodeName, string dbNodeDescription
            , int dbType, bool hasChild
            , Guid thumGuid, string thumbExt)
        {
            _dbParent = parent;
            _dbNodeId = dbNodeId;
            _dbNodeName = dbNodeName;
            _dbNodeDescription = dbNodeDescription;
            _dbType = (NType)dbType;
            _dbHasChild = hasChild;
            _dbThumbFile = new FileTag(thumGuid, thumbExt);
        }
        #endregion

        #region Public static methods
        public static NodeTag[] GetRoot()
        {
            PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
            DCTreeNode[] wrRoot = client.GetRootNodes();

            List<NodeTag> nodeTags = new List<NodeTag>();
            foreach (DCTreeNode wrNode in wrRoot)
            {
                NodeTag nt = new NodeTag(null, wrNode.ID,
                    wrNode.Name, wrNode.Description,
                    (int)wrNode.NodeType, true, //wrNode.HasChildrens,
                    Guid.Empty, string.Empty);
                nodeTags.Add(nt);
            }

            return nodeTags.ToArray();
        }
        #endregion

        #region Public properties
        public Guid ID
        {
            get { return _dbNodeId; }
        }
        public string Name
        {
            get { return _dbNodeName; }
        }
        public string Description
        {
            get { return _dbNodeDescription; }
        }
        public bool IsBranch
        {
            get { return _dbType == NType.NTBranch; }
        }
        public bool IsDocument
        {
            get { return _dbType == NType.NTDocument; }
        }
        public bool IsComponent
        {
            get { return _dbType == NType.NTComponent; }
        }
        public bool HasChildren
        {
            get { return _dbHasChild; }
        }
        public NodeTag[] Chidrens
        {
            get
            {
                if (null == _dbChildrens)
                {
                    _dbChildrens = new List<NodeTag>();
                    PLMPackServiceClient client = WCFClientSingleton.Instance.Client;
                    DCTreeNode[] wrRoot = client.GetTreeNodeChildrens(ID);
                    foreach (DCTreeNode wrTn in wrRoot)
                    {
                        NodeTag nt = new NodeTag(this, wrTn.ID,
                            wrTn.Name, wrTn.Description,
                            (int)wrTn.NodeType, wrTn.HasChildrens,
                            wrTn.Thumbnail.File.Guid, wrTn.Thumbnail.File.Extension);
                        if (NodeType.NTDocument == wrTn.NodeType)
                            nt.Document = new DocumentTag( Guid.NewGuid(), wrTn.Document.Guid, wrTn.Document.Extension);
                        else if (NodeType.NTComponent == wrTn.NodeType)
                            nt.Document = new ComponentTag( wrTn.Component.CGuid, wrTn.Component.File.Guid, wrTn.Component.File.Extension);
                        _dbChildrens.Add(nt);
                    }
                }
                return _dbChildrens.ToArray();
            }
        }
        public FileTag Thumbnail
        {
            get { return _dbThumbFile; }
        }

        public DocumentTag Document
        {
            get { return _dbDocument; }
            set { _dbDocument = value; }
        }

        public NType Type
        {
            get { return _dbType; }
        }
        #endregion
        #region Object overrides
        public override bool Equals(object obj)
        {
            NodeTag nt = obj as NodeTag;
            return ID == nt.ID;
        }
        public override int GetHashCode()
        {
            return _dbNodeId.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0} ({1})", ID, Name);
        }
        #endregion
    }
    #endregion
}
