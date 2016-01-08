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
            PLMPackSR.PLMPackServiceClient client = new PLMPackSR.PLMPackServiceClient();
            client.ClientCredentials.UserName.UserName = UserName;
            client.ClientCredentials.UserName.Password = Password;

            PLMPackSR.DCUser user = client.Connect();
            if (user != null)
            {   if (null != callback)   callback.Info(string.Format("Connection successful: {0}", user.Email));  }
            else
            {
                if (null != callback) callback.Info(string.Format("Failed to connect with user credentials ({0} + {1})", UserName, Password));
                return;
            }

            // ### upload default thumbnails
            Dictionary<string, string> defNameDict = new Dictionary<string, string>()
            {
                { "AI", "Ai.png"},
                { "ARD","ARD.png"},
                { "CALC", "Calc.png"},
                { "CCF2", "CFF2.png"},
                { "DXF", "DXF.png"},
                { "EPS", "EPS.png"},
                { "EXCEL", "Excel.png"},
                { "FOLDER", "Folder.png"},
                { "IMAGE", "Image.png"},
                { "PDF", "pdf.png"},
                { "DES3", "Picador3D.png"},
                { "DES", "PicGEOM.png"},
                { "PPT", "Powerpoint.png"},
                { "WORD", "Word.png"},
                { "WRITER", "Writer.png"}
            };
            foreach (KeyValuePair<string, string> entry in defNameDict)
            {
                string filePath = Path.Combine(RepositoryThumbnail, entry.Value);
                Guid fileGuid = client.UploadDefault(entry.Key, Path.GetExtension(filePath).Trim('.'));
                Upload(filePath, fileGuid, callback);
            }
            client.Close();
            // ### upload default thumbnails
            PPDataContext db = new PPDataContext(_dbPath, RepositoryPath);
            CopyCardboardFormat(db, callback);
            CopyCardboardProfiles(db, callback);
            CopyTreeNodeRecursively(db, callback);

            // end
            if (null != callback) callback.End();
        }
        private void CopyCardboardFormat(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Cardboard formats...");

            PLMPackSR.PLMPackServiceClient client = new PLMPackSR.PLMPackServiceClient();
            client.ClientCredentials.UserName.UserName = UserName;
            client.ClientCredentials.UserName.Password = Password;

            foreach (CardboardFormat cf in db.CardboardFormats)
            {
                if (null != callback)
                    callback.Info(string.Format("Cardboard format {0} ( {1} * {2} )", cf.Name, cf.Length, cf.Width));
                if (!client.CardboardFormatExists(cf.Name))
                    client.CreateNewCardboardFormat(cf.Name, cf.Description, cf.Length, cf.Width);
            }
            client.Close();
        }
        private void CopyCardboardProfiles(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Cardboard profiles...");

            PLMPackSR.PLMPackServiceClient client = new PLMPackSR.PLMPackServiceClient();
            client.ClientCredentials.UserName.UserName = UserName;
            client.ClientCredentials.UserName.Password = Password;

            foreach (CardboardProfile cp in db.CardboardProfiles)
            {
                if (null != callback)
                    callback.Info(string.Format("Cardboard profile {0} ({1})", cp.Name, cp.Thickness));
                if (!client.CardboardProfileExists(cp.Name))
                    client.CreateNewCardboardProfile(cp.Name, "", cp.Code, cp.Thickness);
            }
            client.Close();
        }
        private void CopyTreeNodeRecursively(PPDataContext db, IProcessingCallback callback)
        {
            if (null != callback) callback.Info("Tree nodes...");

            PLMPackSR.PLMPackServiceClient client = new PLMPackSR.PLMPackServiceClient();
            client.ClientCredentials.UserName.UserName = UserName;
            client.ClientCredentials.UserName.Password = Password;

            List<TreeNode> rootNodes = TreeNode.GetRootNodes(db);
            string offset = string.Empty;
            PLMPackSR.DCTreeNode userRootNode = client.GetUserRootNode();

            client.Close();

            foreach (TreeNode root in rootNodes)
            {
                List<TreeNode> rootChildrens = root.Childrens(db);
                foreach (TreeNode tn in rootChildrens)
                    RecursiveInsert(db, tn, userRootNode, string.Empty, callback);
            }
        }
        private void RecursiveInsert(PPDataContext db, TreeNode tn, PLMPackSR.DCTreeNode wsNode, string offset, IProcessingCallback callback)
        {
            PLMPackSR.PLMPackServiceClient client = new PLMPackSR.PLMPackServiceClient();
            client.ClientCredentials.UserName.UserName = UserName;
            client.ClientCredentials.UserName.Password = Password;

            PLMPackSR.DCTreeNode wsNodeChild = null;
            string docType = string.Empty;

            try
            {
                // create node thumbnail
                string thumbPath = tn.Thumbnail.File.PathWRepo(RepositoryPath);
                DCFile thFile = Upload(thumbPath, callback, client);
                PLMPackSR.DCThumbnail wsThumbnail = client.CreateNewThumbnailFromFile(thFile);

                if (tn.IsDocument)
                {
                    // get document
                    Document doc = tn.Documents(db)[0];
                    string docPath = doc.File.PathWRepo(RepositoryPath);
                    // upload document
                    PLMPackSR.DCFile wsDocFile = Upload(docPath, callback, client);

                    if (tn.IsComponent)
                    {
                        docType = "COMPONENT";
                        Component comp = doc.Components[0];

                        // get majorations
                        List<PLMPackSR.DCMajorationSet> majorationSets = new List<PLMPackSR.DCMajorationSet>();
                        foreach (MajorationSet majoSet in comp.MajorationSets)
                        {
                            DCCardboardProfile cbProfile = client.GetCardboardProfileByName(majoSet.CardboardProfile.Name);
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

                        PLMPackSR.DCTreeNode wsNodeComp = client.CreateNewNodeComponent(
                            wsNode, tn.Name, tn.Description
                            , wsThumbnail, wsDocFile, doc.Components[0].Guid
                            , majorationSets.ToArray(), paramDefaultValues.ToArray());
                        client.ShareEveryone(wsNodeComp);
                    }
                    else
                    {
                        docType = "DOCUMENT";
                        PLMPackSR.DCTreeNode wsNodeDocument = client.CreateNewNodeDocument(wsNode, tn.Name, tn.Description
                            , wsThumbnail, wsDocFile);
                        client.ShareEveryone(wsNodeDocument);
                    }
                }
                else
                {
                    docType = "BRANCH";
                    wsNodeChild = client.CreateNewNodeBranch(wsNode, tn.Name, tn.Description, wsThumbnail);
                    client.ShareEveryone(wsNodeChild);
                }

                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                if (null != callback)
                    callback.Error(ex.ToString());
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
        private PLMPackSR.DCFile Upload(string filePath, IProcessingCallback callback, PLMPackSR.PLMPackServiceClient client)
        {
            if (null != callback)
                callback.Info(string.Format(_actuallyUpload ? "Uploading {0}..." : "Not actually uploading {0}...", Path.GetFileName(filePath)));
            return client.CreateNewFile(
                _actuallyUpload ? FileTransferUtility.UploadFile(filePath) : Guid.NewGuid()
                , Path.GetExtension(filePath)
                );
        }
        private void Upload(string filePath, Guid g, IProcessingCallback callback)
        { 
            if (null != callback)
                callback.Info(string.Format(_actuallyUpload ? "Uploading {0}..." : "Not actually uploading {0}...", Path.GetFileName(filePath)));
            FileTransferUtility.UploadFile(filePath, g);        
        }
        private string RepositoryPath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Path.GetDirectoryName(_dbPath)).FullName, "Documents");
            }
        }
        private string RepositoryThumbnail
        {
            get
            {
                return Path.Combine(Directory.GetParent(Path.GetDirectoryName(_dbPath)).FullName, "Thumbnails");
            }
        }
        #endregion

        #region Data members
        private string _dbPath;
        private string _userName, _password;
        private bool _actuallyUpload = true;
        #endregion
    }
}
