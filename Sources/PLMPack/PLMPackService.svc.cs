#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using System.ServiceModel.Channels;
using Microsoft.AspNet.Identity;
using System.Security.Permissions;

using PLMPackModel;
#endregion

namespace PLMPack
{
    public class PLMPackService : IPLMPackService
    {
        #region User
        [PrincipalPermission(SecurityAction.Demand)]
        public DCUser Connect()
        {
            // save connection in database
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser aspNetUser = AspNetUser.GetByUserName(db, UserName);
            aspNetUser.Connect(db);
            return new DCUser()
                {
                    ID = aspNetUser.Id,
                    Name = aspNetUser.UserName,
                    Email = aspNetUser.Email,
                    Phone = aspNetUser.PhoneNumber,
                    GroupID = aspNetUser.GroupId
                };
        }

        [PrincipalPermission(SecurityAction.Demand)]
        public void DisConnect()
        {
            // save connection in database
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser aspNetUser = AspNetUser.GetByUserName(db, UserName);
            aspNetUser.Disconnect(db);
        }

        
        public string UserName
        {
            [PrincipalPermission(SecurityAction.Demand)]
            get
            {
                string userDesc = string.Empty;
                string userName = string.Empty;
                if (OperationContext.Current != null && OperationContext.Current.IncomingMessageProperties != null)
                {
                    var remoteEndpointMessageProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    if (remoteEndpointMessageProperty != null)
                    {
                        userDesc = string.Format("Client IP Address {0}", remoteEndpointMessageProperty.Address);
                    }
                    if (OperationContext.Current.ServiceSecurityContext != null)
                    {
                        if (OperationContext.Current.ServiceSecurityContext.IsAnonymous)
                            throw new Exception(string.Format("Client is Anonymous {0}", "True"));
                        else
                        {
                            userDesc = string.Format("Client Windows Identity {0}", OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name);

                            if (OperationContext.Current.ServiceSecurityContext.PrimaryIdentity != null)
                            {
                                userDesc = string.Format("Client Primary Identity {0}", OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
                                userName = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
                            }
                        }
                    }
                }
                return userName;
            }
        }

        [PrincipalPermission(SecurityAction.Demand)]
        public DCUser GetUser()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName( db, OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name );
            return new DCUser()
            {
                ID = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                GroupID = user.GroupId
            };
        }
        #endregion

        #region Group
        [PrincipalPermission(SecurityAction.Demand)]
        public DCGroup GetCurrentGroup()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Group grp = Group.GetById(db, user.GroupId);
            List<DCUser> users = new List<DCUser>();
            foreach (AspNetUser u in grp.AspNetUsers)
            {
                users.Add(
                    new DCUser()
                    {
                        ID = u.Id,
                        Name = u.UserName,
                        Phone = u.PhoneNumber,
                        Email = u.Email,
                        GroupID = grp.Id
                    }
                    );
            }
            return new DCGroup()
                {
                    ID = grp.Id,
                    Name = grp.GroupName,
                    Description = grp.GroupDesc,
                    Members = users.ToArray()
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void SetCurrentGroup(string grpId)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            user.SetCurrentGroup(db, Group.GetById(db, grpId));
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void AddInterest(string grpId)
        { 
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Group grp = Group.GetById(db, grpId);
            user.AddGroupOfInterest(db, grp);           
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void RemoveInterest(string grpId)
        { 
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Group grp = Group.GetById(db, grpId);
            user.RemoveGroupOfInterest(db, grp);
        }
        #endregion

        #region Cardboard format
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboadFormat[] GetAllCardboardFormats()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardFormat[] cardboardFormats = CardboardFormat.GetAll(db, user.CurrentGroup(db));
            List<DCCardboadFormat> listCardboardFormat = new List<DCCardboadFormat>();
            foreach (CardboardFormat cf in cardboardFormats)
            {
                listCardboardFormat.Add(new DCCardboadFormat()
                    {
                        ID = cf.Id,
                        Name = cf.Name,
                        Description = cf.Description,
                        Length = cf.Length,
                        Width = cf.Width
                    }
                    );
            }
            return listCardboardFormat.ToArray();
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboadFormat GetCardboardFormatByID(int id)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardFormat cf = CardboardFormat.GetById(db, id);
            return new DCCardboadFormat()
            {
                ID = cf.Id,
                Name = cf.Name,
                Description = cf.Description,
                Length = cf.Length,
                Width = cf.Width
            };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public bool CardboardFormatExists(string name)
        { 
             PLMPackEntities db = new PLMPackEntities();
             AspNetUser user = AspNetUser.GetByUserName(db, UserName);
             return CardboardFormat.Exists(db, user.CurrentGroup(db), name);
           
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboadFormat GetCardboardFormatByName(string name)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardFormat cf = CardboardFormat.GetByName(db, user.CurrentGroup(db), name);
            return new DCCardboadFormat()
            {
                ID = cf.Id,
                Name = cf.Name,
                Description = cf.Description,
                Length = cf.Length,
                Width = cf.Width
            };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboadFormat CreateNewCardboardFormat(string name, string description, double length, double width)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Group gp = Group.GetById(db, user.GroupId);
            CardboardFormat cf = CardboardFormat.CreateNew( db, gp, name, description, length, width);
            return new DCCardboadFormat() { ID = cf.Id, Name = cf.Name, Description = cf.Description, Length = cf.Length, Width = cf.Width };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void RemoveCardboardFormat(int id)
        { 
            PLMPackEntities db = new PLMPackEntities();
            CardboardFormat cf = CardboardFormat.GetById(db, id);
            cf.Delete(db);            
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboadFormat UpdateCardboardFormat(DCCardboadFormat cbFormat)
        {
            PLMPackEntities db = new PLMPackEntities();
            CardboardFormat cf = CardboardFormat.GetById(db, cbFormat.ID);
            cf.Name = cbFormat.Name;
            cf.Description = cbFormat.Description;
            cf.Length = cbFormat.Length;
            cf.Width = cbFormat.Width;
            db.SaveChanges();
            return cbFormat;
        }
        #endregion

        #region Cardboard profile
        [PrincipalPermission(SecurityAction.Demand)]
        public bool CardboardProfileExists(string name)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            return CardboardProfile.Exists(db, user.CurrentGroup(db), name);
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboardProfile[] GetAllCardboardProfile()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardProfile[] cardboardProfiles = CardboardProfile.GetAll(db, user.CurrentGroup(db));
            List<DCCardboardProfile> listCardboardProfiles = new List<DCCardboardProfile>();
            foreach (CardboardProfile cp in cardboardProfiles)
            {
                listCardboardProfiles.Add(
                    new DCCardboardProfile()
                        {
                            ID = cp.Id,
                            Name = cp.Name,
                            Description = cp.Description,
                            Code = cp.Code,
                            Thickness = cp.Thickness
                        }
                    );
            }
            return listCardboardProfiles.ToArray();
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboardProfile GetCardboardProfileByID(int id)
        {
            PLMPackEntities db = new PLMPackEntities();
            CardboardProfile cp = CardboardProfile.GetByID(db, id);
            return new DCCardboardProfile()
                {
                    ID = cp.Id,
                    Name = cp.Name,
                    Description = cp.Description,
                    Code = cp.Code,
                    Thickness = cp.Thickness
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboardProfile GetCardboardProfileByName(string name)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardProfile cp = CardboardProfile.GetByName(db, user.CurrentGroup(db), name);
            return new DCCardboardProfile()
                {
                    ID = cp.Id,
                    Name = cp.Name,
                    Description = cp.Description,
                    Code = cp.Code,
                    Thickness = cp.Thickness
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboardProfile CreateNewCardboardProfile(string name, string description, string code, double thickness)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardProfile cf = CardboardProfile.CreateNew(db, user.CurrentGroup(db), name, description, code, thickness);
            return new DCCardboardProfile()
                {
                    ID = cf.Id,
                    Name = cf.Name,
                    Description = cf.Description,
                    Code = cf.Code,
                    Thickness = thickness
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void RemoveCardboardProfile(int id)
        {
            PLMPackEntities db = new PLMPackEntities();
            CardboardProfile cp = CardboardProfile.GetByID(db, id);
            cp.Delete(db);
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCCardboardProfile UpdateCardboardProfile(DCCardboardProfile cbProfile)
        {
            PLMPackEntities db = new PLMPackEntities();
            CardboardProfile cp = CardboardProfile.GetByID(db, cbProfile.ID);
            cp.Name = cbProfile.Name;
            cp.Description = cbProfile.Description;
            cp.Code = cbProfile.Code;
            cp.Thickness = cbProfile.Thickness;
            db.SaveChanges();
            return cbProfile;
        }
        #endregion

        #region Carboard quality
        #endregion

        #region File
        [PrincipalPermission(SecurityAction.Demand)]
        public DCFile CreateNewFile(Guid g, string ext)
        {
            PLMPackEntities db = new PLMPackEntities();
            File f = File.CreateNew(db, g, ext);
            return new DCFile()
            {
                Guid = new Guid(f.Guid),
                Extension = f.Extension,
                DateCreated = f.DateCreated
            };
        }
        #endregion

        #region Thumbnail
        [PrincipalPermission(SecurityAction.Demand)]
        public DCThumbnail CreateNewThumbnail(Guid g, string ext)
        {
            PLMPackEntities db = new PLMPackEntities();
            Thumbnail tb = Thumbnail.CreateNew(db, g, ext);
            return new DCThumbnail()
                {  
                    ID = tb.Id,
                    MimeType = tb.MimeType,
                    Width = tb.Width,
                    Height = tb.Height,
                    File = new DCFile() { Guid = new Guid(tb.File.Guid), Extension = tb.File.Extension, DateCreated = tb.File.DateCreated }
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCThumbnail GetDefaultThumbnail(string defName)
        {
            PLMPackEntities db = new PLMPackEntities();
            Thumbnail tb = Thumbnail.GetDefaultThumbnail(db, defName);
            return new DCThumbnail()
                {
                    ID = tb.Id,
                    MimeType = tb.MimeType,
                    Width = tb.Width,
                    Height = tb.Height,
                    File = new DCFile() { Guid = new Guid(tb.File.Guid), Extension = tb.File.Extension, DateCreated = tb.File.DateCreated }
                };
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCThumbnail GetThumbnailById(int thumbnailId)
        { 
            PLMPackEntities db = new PLMPackEntities();
            Thumbnail tb = Thumbnail.GetByID(db, thumbnailId);
            return new DCThumbnail()
                {
                    ID = tb.Id,
                    MimeType = tb.MimeType,
                    Width = tb.Width,
                    Height = tb.Height,
                    File = new DCFile() { Guid = new Guid(tb.File.Guid), Extension = tb.File.Extension, DateCreated = tb.File.DateCreated }
                };
        }
        #endregion

        #region Tree nodes
        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode[] GetRootNodes()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode[] rootNodes = TreeNode.GetRootNodes(db, user);
            List<DCTreeNode> listTreeNode = new List<DCTreeNode>();
            foreach (TreeNode tn in rootNodes)
            {
                listTreeNode.Add(new DCTreeNode()
                    {
                        ID = new Guid(tn.Id),
                        ParentNodeID = Guid.Empty,
                        Name = tn.Name,
                        Description = tn.Description,
                        NodeType = DCNodeTypeEnum.NTBranch,
                        HasChildrens = tn.HasChildrens
                    }
                    );            
            }
            return listTreeNode.ToArray();
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode GetUserRootNode()
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode tnRoot = TreeNode.GetUserRootNode(db, user);
            return new DCTreeNode()
                {
                    ID = new Guid(tnRoot.Id),
                    ParentNodeID = Guid.Empty,
                    Name = tnRoot.Name,
                    Description = tnRoot.Description,
                    NodeType = DCNodeTypeEnum.NTBranch,
                    HasChildrens = tnRoot.HasChildrens
                };
        }

        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode[] GetTreeNodeChildrens(Guid nodeId)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode node = TreeNode.GetById(db, nodeId);
            List<DCTreeNode> listTreeNode = new List<DCTreeNode>();
            foreach (TreeNode tn in node.ChildrensByUser(db, user))
                listTreeNode.Add(Db_2_DCTreeNode(db, user, tn));
            return listTreeNode.ToArray();
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode CreateNewNodeBranch(Guid parentNodeId, string name, string description, int thumbnailId)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode tnParent = TreeNode.GetById(db, parentNodeId);
            TreeNode tnBranch = tnParent.InsertBranch(db, user.GroupId, name, description, Thumbnail.GetByID(db, thumbnailId));
            return Db_2_DCTreeNode(db, user, tnBranch);
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode CreateNewNodeDocument(Guid parentNodeId, string name, string description, int thumbnailId, string docType, DCFile dFile)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode tnParent = TreeNode.GetById(db, parentNodeId);
            Thumbnail thumb = Thumbnail.GetByID(db, thumbnailId);
            TreeNode tnDocument = tnParent.InsertDocument(db, user.GroupId, name, description, docType, dFile.Guid, dFile.Extension, thumb);
            return Db_2_DCTreeNode(db, user, tnDocument);
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCTreeNode CreateNewNodeComponent(Guid parentNodeId, string name, string description, int thumbnailId, DCFile compFile, Guid compGuid,
            DCMajorationSet[] majorationSets, DCParamDefaultValue[] defaultValues)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            TreeNode tnParent = TreeNode.GetById(db, parentNodeId);
            Thumbnail thumb = Thumbnail.GetByID(db, thumbnailId);

            TreeNode tnComponent = tnParent.InsertComponent(
                db, user.GroupId,
                name, description,
                compFile.Guid, compGuid,
                thumb);
            File f = thumb.File;
            return Db_2_DCTreeNode(db, user, tnComponent);
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void ShareTreeNode(string treeNodeId, string grpId)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Group grp = Group.GetById(db, grpId);

            TreeNode node = TreeNode.GetById(db, new Guid(treeNodeId));
            node.Share(db, user, grp);
        }
        #endregion

        #region Components
        [PrincipalPermission(SecurityAction.Demand)]
        public DCComponent GetComponentByGuid(Guid g)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Component c = Component.GetByGuid(db, g);
            return Db_2_Component(db, user, c);
            
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCMajorationSet UpdateMajorationSet(Guid g, int profileId, DCMajoration[] majorations)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            CardboardProfile cp = CardboardProfile.GetByID(db, profileId);
            Component comp = Component.GetByGuid(db, g);

            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (DCMajoration maj in majorations)
                dict.Add(maj.Name, maj.Value);
            comp.UpdateMajorationSet(db, cp, dict);

            dict = comp.GetMajorationSet(db, cp);
            DCMajorationSet majoSet = new DCMajorationSet();
            majoSet.Profile = new DCCardboardProfile()
            {
                ID = cp.Id,
                Name = cp.Name,
                Description = cp.Description,
                Code = cp.Code,
                Thickness = cp.Thickness
            };
            majoSet.Majorations = new DCMajoration[dict.Count];
            int iCount = 0;
            foreach (KeyValuePair<string, double> v in dict)
            { majoSet.Majorations[iCount] = new DCMajoration() { Name = v.Key, Value = v.Value }; ++iCount; }
            return majoSet;
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCMajorationSet GetMajorationSet(Guid g, int profileId)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Component comp = Component.GetByGuid(db, g);
            CardboardProfile cp = CardboardProfile.GetByID(db, profileId);
            Dictionary<string, double> dict = comp.GetMajorationSet(db, cp);
            DCMajorationSet majoSet = new DCMajorationSet();
            majoSet.Profile = new DCCardboardProfile()
                {
                    ID = cp.Id,
                    Name = cp.Name,
                    Description = cp.Description,
                    Code = cp.Code,
                    Thickness = cp.Thickness
                };
            majoSet.Majorations = new DCMajoration[dict.Count];
            int iCount = 0;
            foreach (KeyValuePair<string, double> v in dict)
            { majoSet.Majorations[iCount] = new DCMajoration() { Name = v.Key, Value = v.Value }; ++iCount; }
            return majoSet;
        }
        [PrincipalPermission(SecurityAction.Demand)]
        public DCParamDefaultValue[] GetParamDefaultValue(Guid g)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);

            Component c = Component.GetByGuid(db, g);
            Dictionary<string, double> dictDefaultValues = c.GetParamDefaultValues(db, user.GroupId);
            DCParamDefaultValue[] paramDefaultValue = new DCParamDefaultValue[dictDefaultValues.Count];
            int iCount = 0;
            foreach (KeyValuePair<string, double> entry in dictDefaultValues)
                paramDefaultValue[iCount++] = new DCParamDefaultValue() { Name = entry.Key, Value = entry.Value };
            return paramDefaultValue;

        }
        [PrincipalPermission(SecurityAction.Demand)]
        public void UpdateParamDefaultComponent(Guid g, DCParamDefaultValue[] paramDefaultValue)
        {
            PLMPackEntities db = new PLMPackEntities();
            AspNetUser user = AspNetUser.GetByUserName(db, UserName);
            Component c = Component.GetByGuid(db, g);
            for (int i = 0; i < paramDefaultValue.Length; ++i)
            {
                DCParamDefaultValue param = paramDefaultValue[i];
                c.InsertParamDefaultValue(db, user.GroupId, param.Name, param.Value);
            }
        }
        #endregion

        #region Helpers
        private DCFile Db_2_DCFile(File f)
        {
            if (null == f) return null;
            return new DCFile() { Guid = new Guid(f.Guid), Extension = f.Extension, DateCreated = f.DateCreated };
        }
        private DCThumbnail Db_2_DCThumbnail(Thumbnail th)
        {
            return new DCThumbnail() { ID = th.Id, Width = th.Width, Height = th.Height, MimeType = th.MimeType, File = Db_2_DCFile(th.File) };
        }
        private DCComponent Db_2_Component(PLMPackEntities db, AspNetUser user, Component c)
        {
            // null component
            if (null == c)  return null;
            // ### component default parameter values 
            List<DCParamDefaultValue> paramDefaults = new List<DCParamDefaultValue>();
            Dictionary<string, double> dictParamDefaults = c.GetParamDefaultValues(db, user.GroupId);
            foreach (KeyValuePair<string, double> entry in dictParamDefaults)
            {
                paramDefaults.Add(
                    new DCParamDefaultValue() { Name = entry.Key, Value = entry.Value }
                    );
            }
            // ### component default majorations
            List<DCMajorationSet> majoSets = new List<DCMajorationSet>();
            foreach (MajorationSet mjs in c.MajorationSet)
            {
                List<DCMajoration> listMajo = new List<DCMajoration>();
                CardboardProfile cp = mjs.CardboardProfile;
                majoSets.Add(new DCMajorationSet()
                   {
                       Profile = new DCCardboardProfile()
                           {
                               ID = cp.Id,
                               Name = cp.Name,
                               Description = cp.Description,
                               Code = cp.Code,
                               Thickness = cp.Thickness
                           },
                       Majorations = listMajo.ToArray()
                   }
                   );
            }
            return new DCComponent()
             {
                 CGuid = new Guid(c.Guid),
                 File = Db_2_DCFile(c.Document.File),
                 ParamDefaults = paramDefaults.ToArray(),
                 MajoSets = majoSets.ToArray()
             };
        }
        private DCTreeNode Db_2_DCTreeNode(PLMPackEntities db, AspNetUser user, TreeNode tn)
        {
            DCNodeTypeEnum nodeType = DCNodeTypeEnum.NTBranch;
            if (tn.IsDocument) nodeType = DCNodeTypeEnum.NTDocument;
            if (tn.IsComponent) nodeType = DCNodeTypeEnum.NTComponent;

            Document d = tn.IsDocument ? tn.FirstDocument : null;
            Component c = tn.FirstComponent;

            return new DCTreeNode()
            {
                ID = Guid.Parse(tn.Id),
                ParentNodeID = Guid.Parse(tn.ParentNodeId),
                Name = tn.Name,
                Description = tn.Description,
                NodeType = nodeType,
                HasChildrens = tn.HasChildrens,
                Thumbnail = Db_2_DCThumbnail(tn.Thumbnail),
                Document = Db_2_DCFile(d != null ? d.File : null),
                Component = Db_2_Component(db, user, c)
            };
        }
        #endregion
    }
}
