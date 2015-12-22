#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net;

using Pic.DAL.SQLite;

using treeDiM.PLMPackLib.PLMPackSR;
using treeDiM.FileTransfer;
#endregion

namespace treeDiM.PLMPackLib
{
    public class LegacyDBUploder
    {
        #region Public methods
        public void Upload(IProcessingCallback callback)
        {
            // check that database actually exist
            if (!System.IO.File.Exists(_dbPath))
            {
                if (null != callback)
                    callback.Error(string.Format("Input database path ({0}) could not be found.", _dbPath));
                return;
            }
            // begin
            if (null != callback) callback.Begin();
            // connect
            _client = new PLMPackSR.PLMPackServiceClient();
            _client.ClientCredentials.UserName.UserName = UserName;
            _client.ClientCredentials.UserName.Password = Password;

            PLMPackSR.DCUser user = _client.Connect();
            if (user != null)
            {   if (null != callback)   callback.Info(string.Format("Connection successful: {0}", user.Email));  }
            else
            {
                if (null != callback) callback.Info(string.Format("Failed to connect with user credentials ({0} + {1})", UserName, Password));
                return;
            }

            PPDataContext db = new PPDataContext(_dbPath, RepositoryPath);
            CopyCardboardFormat(db, callback);
            CopyCardboardProfiles(db, callback);
            CopyTreeNodeRecursively(db, callback);

            _client.Close();
            // end
            if (null != callback) callback.End();
        }

        private void CopyCardboardFormat(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Cardboard formats...");
            foreach (CardboardFormat cf in db.CardboardFormats)
            {
                if (null != callback)
                    callback.Info(string.Format("Cardboard format {0} ( {1} * {2} )", cf.Name, cf.Length, cf.Width));
                if (!_client.CardboardFormatExists(cf.Name))
                    _client.CreateNewCardboardFormat(cf.Name, cf.Description, cf.Length, cf.Width);
            }
        }
        private void CopyCardboardProfiles(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Cardboard profiles...");
            foreach (CardboardProfile cp in db.CardboardProfiles)
            {
                if (null != callback)
                    callback.Info(string.Format("Cardboard profile {0} ({1})", cp.Name, cp.Thickness));
                if (!_client.CardboardProfileExists(cp.Name))
                    _client.CreateNewCardboardProfile(cp.Name, "", cp.Code, cp.Thickness);
            }

        }
        private void CopyTreeNodeRecursively(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Tree nodes...");
            List<TreeNode> rootNodes = TreeNode.GetRootNodes(db);
            string offset = string.Empty;
            PLMPackSR.DCTreeNode userRootNode = _client.GetUserRootNode();
            foreach (TreeNode tn in rootNodes)
                RecursiveInsert(db, tn, userRootNode, string.Empty, callback);

        }

        private void RecursiveInsert(PPDataContext db, TreeNode tn, PLMPackSR.DCTreeNode wsNode, string offset, IProcessingCallback callback)
        {
            // create node thumbnail
            string thumbPath = tn.Thumbnail.File.PathWRepo( RepositoryPath );
            DCFile thFile = Upload(thumbPath, callback);
            PLMPackSR.DCThumbnail wsThumbnail = _client.CreateNewThumbnailFromFile(thFile);

            string docType = string.Empty;
            PLMPackSR.DCTreeNode wsNodeChild = null;
            if (tn.IsDocument)
            {
                // get document
                Document doc = tn.Documents(db)[0];
                string docPath = doc.File.PathWRepo(RepositoryPath);
                // upload document
                PLMPackSR.DCFile wsDocFile = Upload(docPath, callback);

                if (tn.IsComponent)
                {
                    docType = "COMPONENT";
                    Component comp = doc.Components[0];

                    // get majorations
                    List<PLMPackSR.DCMajorationSet> majorationSets = new List<PLMPackSR.DCMajorationSet>();
                    foreach (MajorationSet majoSet in comp.MajorationSets)
                    {
                        DCCardboardProfile cbProfile = _client.GetCardboardProfileByName(majoSet.CardboardProfile.Name);
                        string sMajo = string.Empty;
                        List<DCMajoration> dcMajorationList = new List<DCMajoration>();
                        foreach (Majoration majo in majoSet.Majorations)
                        {
                            sMajo += string.Format("({0}={1})", majo.Name, majo.Value);
                            dcMajorationList.Add(new DCMajoration() { Name = majo.Name, Value = majo.Value });
                        }
                        majorationSets.Add(
                            new DCMajorationSet()
                            {
                                Profile = cbProfile,
                                Majorations = dcMajorationList.ToArray()
                            }
                            );

                        if (null != callback)
                            callback.Info(string.Format("{0} - {1}", majoSet.CardboardProfile.Name, sMajo));
                    }
                    // get default parameter values
                    List<PLMPackSR.DCParamDefaultValue> paramDefaultValues = new List<PLMPackSR.DCParamDefaultValue>();
                    foreach (ParamDefaultValue pdv in comp.ParamDefaultValues)
                        paramDefaultValues.Add(new DCParamDefaultValue() { Name = pdv.Name, Value = pdv.Value });

                    PLMPackSR.DCTreeNode wsNodeComp = _client.CreateNewNodeComponent(
                        wsNode, tn.Name, tn.Description
                        , wsThumbnail, wsDocFile, doc.Components[0].Guid
                        , majorationSets.ToArray(), paramDefaultValues.ToArray());
                }
                else
                {
                    docType = "DOCUMENT";
                    PLMPackSR.DCTreeNode wsNodeDocument = _client.CreateNewNodeDocument(wsNode, tn.Name, tn.Description
                        , wsThumbnail, wsDocFile, docType);
                }                    
            }
            else
            {
                docType = "BRANCH";
                wsNodeChild = _client.CreateNewNodeBranch(wsNode, tn.Name, tn.Description, wsThumbnail);
            }
            if (null == wsNodeChild)
                return;

            if (null != callback)
                callback.Info(string.Format("{0}-> {1} ({2})", offset, tn.Name, docType));
            offset += "   ";
            foreach (TreeNode tnChild in tn.Childrens(db))
                RecursiveInsert(db, tnChild, wsNodeChild, offset, callback);
        }
        #endregion

        #region Public properties
        public string DatabasePath
        {
            get { return _dbPath; }
            set { _dbPath = value; }
        }
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public bool ActuallyUpload
        {
            get { return _actuallyUpload; }
            set { _actuallyUpload = value; }
        }
        #endregion

        #region Helpers
        private PLMPackSR.DCFile Upload(string filePath, IProcessingCallback callback)
        {
            if (null != callback)
                callback.Info(string.Format(_actuallyUpload ? "Uploading {0}..." : "Not actually uploading {0}...", Path.GetFileName(filePath)));
            return _client.CreateNewFile(
                _actuallyUpload ? FileTransferUtility.UploadFile(filePath) : Guid.NewGuid()
                , Path.GetExtension(filePath)
                );
        }
        private string RepositoryPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Path.GetDirectoryName(_dbPath)).FullName, "Documents");
            }
        }
        #endregion

        #region Data members
        private string _dbPath;
        private string _userName, _password;
        private bool _actuallyUpload = true;
        private PLMPackSR.PLMPackServiceClient _client;
        #endregion
    }
}
