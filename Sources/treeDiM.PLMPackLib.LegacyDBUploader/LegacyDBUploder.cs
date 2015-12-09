#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net;

using Pic.DAL.SQLite;
#endregion

namespace treeDiM.PLMPackLib
{
    public class LegacyDBUploder
    {
        #region Public methods
        public void Upload(IProcessingCallback callback)
        {
            if (null != callback) callback.Begin();

            // connect
            _client = new PLMPackSR.PLMPackServiceClient();
            _client.ClientCredentials.UserName.UserName = UserName;
            _client.ClientCredentials.UserName.Password = Password;

            PLMPackSR.DCUser user = _client.Connect();
            if (null != callback)
                callback.Info(string.Format("Connection successful: {0}", user.Email));

            string repositoryPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(_dbPath)).FullName, "Documents");
            PPDataContext db = new PPDataContext(_dbPath, repositoryPath);
            CopyCardboardFormat(db, callback);
            CopyCardboardProfiles(db, callback);
            CopyTreeNodeRecursively(db, callback);

            _client.Close();

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
            foreach (TreeNode tn in rootNodes)
                RecursiveInsert(db, tn, offset, callback);

        }

        private void RecursiveInsert(PPDataContext db, TreeNode tn, string offset, IProcessingCallback callback)
        {
            string docType = string.Empty;
            if (tn.IsDocument)
            {
                // get document
                Document doc = tn.Documents(db)[0];

                if (tn.IsComponent)
                {
                    docType = "COMPONENT";
                    Component comp = doc.Components[0];
                    foreach (MajorationSet majoSet in comp.MajorationSets)
                    {
                        string sMajo = string.Empty;
                        foreach (Majoration majo in majoSet.Majorations)
                        {
                            sMajo += string.Format("({0}={1})", majo.Name, majo.Value);
                        }

                        if (null != callback)
                            callback.Info(string.Format("{0} - {1}", majoSet.CardboardProfile.Name, sMajo));
                    }
                }
                else
                {
                    docType = "DOCUMENT";
                }                    
            }
            else
            {
                docType = "BRANCH";
            }
            if (null != callback)
                callback.Info(string.Format("{0}-> {1} ({2})", offset, tn.Name, docType));

            offset += "   ";
            foreach (TreeNode tnChild in tn.Childrens(db))
                RecursiveInsert(db, tnChild, offset, callback);
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
        #endregion

        #region Data members
        private string _dbPath;
        private string _userName, _password;
        private PLMPackSR.PLMPackServiceClient _client;
        #endregion
    }
}
