#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace PLMPackModel
{
    #region AspNetUser
    public partial class AspNetUser
    {
        public static AspNetUser GetById(PLMPackEntities db, string userId)
        {
            return db.AspNetUsers.Single(usr => usr.Id == userId);
        }
        public static AspNetUser GetByUserName(PLMPackEntities db, string userName)
        {
            try
            {
                return db.AspNetUsers.Single(usr => usr.UserName == userName);
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                throw ex;
            }
        }

        public List<Group> AllGroups(PLMPackEntities db)
        {
            List<Group> grps = new List<Group>(Groups);
            grps.Add(Group.Everyone(db));
            return grps;
        }

        #region Current group
        public Group CurrentGroup(PLMPackEntities db)
        {
            if (string.IsNullOrEmpty(GroupId))
            {
                string grpName = UserName;

                Group grp = Group.CreateNew(db
                    , string.Format("{0}_grp", grpName), string.Format("Default group of user {0}", grpName)
                    , this);
                grp.AspNetUsers.Add(this);
                GroupId = grp.Id;
                db.SaveChanges();
            }
            return db.Groups.Single(grp => grp.Id == GroupId);
        }
        public void SetCurrentGroup(PLMPackEntities db, Group gp)
        {
            GroupId = gp.Id;
            db.SaveChanges();
        }
        #endregion

        #region Group of interest
        public bool HasGroupOfInterest(Group gp)
        {
            return GroupsOfInterest.Contains(gp);
        }
        public void AddGroupOfInterest(PLMPackEntities db, Group gp)
        {
            if (!HasGroupOfInterest(gp))
            {
                GroupsOfInterest.Add(gp);
                db.SaveChanges();
            }
        }
        public void RemoveGroupOfInterest(PLMPackEntities db, Group gp)
        {
            if (HasGroupOfInterest(gp))
            {
                GroupsOfInterest.Remove(gp);
                db.SaveChanges();
            }
        }
        #endregion

        #region Connect / Disconnect
        public void Connect(PLMPackEntities db)
        {
            UserConnection uConnect = new UserConnection();
            uConnect.UserId = Id;
            uConnect.DateConnectIN = DateTime.Now;
            uConnect.DateConnectOUT = null;
            db.UserConnections.Add(uConnect);
            db.SaveChanges();
        }

        public void Disconnect(PLMPackEntities db)
        {
            UserConnection uConnect = db.UserConnections.Where(uc => uc.UserId == Id)
                                                        .OrderByDescending(uc => uc.DateConnectIN)
                                                        .FirstOrDefault();
            uConnect.DateConnectOUT = DateTime.Now;
            db.SaveChanges();
        }
        #endregion
    }
    #endregion

    #region TreeNode
    public partial class TreeNode
    {
        #region Static methods
        public static TreeNode CreateNew(PLMPackEntities db, string grpId
            , string parentNodeId
            , string name, string description
            , int thumbId)
        {
            if (!string.IsNullOrEmpty(parentNodeId))
            {
                TreeNode tnParent = TreeNode.GetById(db, new Guid(parentNodeId));
                if (null != tnParent)
                {
                    if (tnParent.HasChildWithName(name))
                        throw new ModelException(string.Format("Node {0} ({1}) already has child named {1}", parentNodeId, tnParent.Name, name));
                }
            }

            TreeNode tn = new TreeNode();
            tn.Id = Guid.NewGuid().ToString();
            tn.GroupId = grpId;
            tn.ParentNodeId = parentNodeId;
            tn.Name = name;
            tn.Description = description;
            tn.ThumbnailId = thumbId;
            db.TreeNodes.Add(tn);
            db.SaveChanges();
            return tn;
        }

        public static TreeNode GetById(PLMPackEntities db
            , Guid id)
        {
            if (Guid.Empty == id) throw new ModelException("TreeNode.GetById() cannot handle NULL TreeNode Id");
            return db.TreeNodes.Single(tn => tn.Id == id.ToString());
        }

        public static TreeNode[] GetRootNodes(PLMPackEntities db, AspNetUser user)
        {
            TreeNode[] tNodes = db.TreeNodes.Where(
                tn => string.IsNullOrEmpty(tn.ParentNodeId)
                    ).ToArray();
            List<TreeNode> rootNodes = new List<TreeNode>();
            foreach (TreeNode tn in tNodes)
            {
                if (tn.IsShared(db, user) && (user.HasGroupOfInterest(tn.Group) || (tn.Group.Id == user.GroupId)))
                    rootNodes.Add(tn);
            }
            return rootNodes.ToArray();
        }
        public static TreeNode GetUserRootNode(PLMPackEntities db, AspNetUser user)
        {
            return db.TreeNodes.Single(
                tn => string.IsNullOrEmpty(tn.ParentNodeId) && (tn.GroupId == user.GroupId)
                );
        }
        #endregion

        #region Non static methods
        #region Childrens
        public bool HasChildrens
        {
            get { return Childrens.Count > 0; }
        }
        public bool HasChildWithName(string name)
        {
            foreach (TreeNode tn in Childrens)
            { if (string.Equals(tn.Name, name)) return true; }
            return false;
        }
        #region Sharing
        public void Share(PLMPackEntities db, AspNetUser user)
        {
            Share(db, user, Group.Everyone(db));
        }
        public void Share(PLMPackEntities db, AspNetUser user, Group gp)
        {
            if (Group != user.CurrentGroup(db)) return; // -> user not allowed to share
            if (IsShared(gp)) return;   // already shared with this group
            TreeNodeGroupShare tngs = new PLMPackModel.TreeNodeGroupShare();
            tngs.TreeNodeId = Id;
            tngs.GroupId = gp.Id;
            db.TreeNodeGroupShares.Add(tngs);
            db.SaveChanges();
        }
        public bool IsShared(Group gp)
        {
            if (Group == gp)
                return true;
            if (TreeNodeGroupShare.Count(tngs => tngs.TreeNodeId == Id && tngs.GroupId == gp.Id) > 0)
                return true;
            return false;
        }
        public bool IsShared(PLMPackEntities db, AspNetUser user)
        {
            foreach (Group gp in user.AllGroups(db))
                if (IsShared(gp))
                    return true;
            return false;
        }
        #endregion
        #region Console Writing
        public TreeNode[] ChildrensByUser(PLMPackEntities db, AspNetUser user)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode tn in Childrens)
            {
                if (tn.IsShared(db, user) && user.HasGroupOfInterest(tn.Group))
                    nodes.Add(tn);
            }
            return nodes.ToArray();
        }
        public void Print(PLMPackEntities db, AspNetUser user, string offset)
        {
            // node not shared with user or user has no interest in group that created node
            if (!IsShared(db, user) || !user.HasGroupOfInterest(Group))
                return;
            // actually show node
            Console.WriteLine(string.Format("{0}->{1}", offset, Name));
            // show childrens
            foreach (TreeNode tn in Childrens)
                tn.Print(db, user, offset + "    ");
        }
        #endregion
        #region Type
        public bool IsDocument
        {
            get { return TreeNodeDocuments.Count > 0; }
        }
        public bool IsComponent
        {
            get
            {
                if (0 == TreeNodeDocuments.Count)
                    return false;
                else
                {
                    foreach (TreeNodeDocument d in TreeNodeDocuments)
                    {
                        if (d.Document.IsComponent)
                            return true;
                    }
                    return false;
                }
            }
        }
        #endregion
        #region Removal
        public void Delete(PLMPackEntities db, AspNetUser user)
        {
            if (!user.Groups.Contains(Group))
                throw new ModelException(string.Format("User {0} is not allowed to perform this operation.", user.UserName));
            // remove childrens
            List<TreeNode> childList = new List<TreeNode>(Childrens);
            childList.ForEach(tn => tn.Delete(db, user));
            // remove TreeNodeDocument
            List<TreeNodeDocument> lTreeNodeDocs = new List<TreeNodeDocument>(db.TreeNodeDocuments.Where(tnd => tnd.TreeNodeId == Id).ToArray());
            lTreeNodeDocs.ForEach(tnd => tnd.Delete(db));
            int thumbnailId = this.ThumbnailId;
            // remove
            while (TreeNodeGroupShare.Count() > 0)
            {
                TreeNodeGroupShare tnGpShare = TreeNodeGroupShare.First();
                db.TreeNodeGroupShares.Remove(tnGpShare);
            }
            db.SaveChanges();
            // actually remove node
            db.TreeNodes.Remove(this);
            db.SaveChanges();
            // delete thubnail
            Thumbnail.DeleteIfNotRefered(db, thumbnailId);
        }
        #endregion
        #endregion

        #region Insertion methods
        public TreeNode InsertBranch(PLMPackEntities db, string grpId
            , string name, string description
            , Thumbnail thumb)
        {
            // group validity
            if (grpId != GroupId)
                throw new ModelException(string.Format("User not allowed to insert under node {0}", GetPath(db)));
            // name validity
            if (HasChildWithName(name))
                throw new ModelException(string.Format("Node {0} ({1}) already has child named {1}", Id, Name, name));

            // create treenode
            TreeNode tn = new TreeNode();
            tn.Id = Guid.NewGuid().ToString();
            tn.GroupId = grpId;
            tn.ParentNodeId = Id;
            tn.Name = name;
            tn.Description = description;
            tn.ThumbnailId = thumb.Id;
            db.TreeNodes.Add(tn);
            db.SaveChanges();

            return TreeNode.GetById(db, Guid.Parse(tn.Id));
        }

        public TreeNode InsertDocument(PLMPackEntities db, string grpId
            , string name, string description
            , string docType, Guid docGuid, string docExt
            , Thumbnail thumb)
        {
            // create TreeNode
            TreeNode tn = InsertBranch(db, grpId, name, description, thumb);
            // create document
            Document doc = Document.CreateNew(db, grpId
                , docType
                , name, description
                , docGuid, docExt);
            // create treeNode document
            TreeNodeDocument tnd = new TreeNodeDocument();
            tnd.TreeNodeId = tn.Id;
            tnd.DocumentId = doc.Id;
            db.TreeNodeDocuments.Add(tnd);
            db.SaveChanges();

            return TreeNode.GetById(db, Guid.Parse(tn.Id));
        }

        public TreeNode InsertComponent(PLMPackEntities db, string grpId
            , string name, string description
            , Guid fileGuid, Guid compGuid
            , Thumbnail thumb)
        {
            // create TreeNode
            TreeNode tn = InsertBranch(db, grpId, name, description, thumb);
            // create component
            Component cp = Component.CreateNew(db, grpId, name, description, fileGuid, compGuid);
            // create treeNode document
            db.TreeNodeDocuments.Add(new TreeNodeDocument() { TreeNodeId = tn.Id, DocumentId = cp.DocumentId });
            db.SaveChanges();

            return TreeNode.GetById(db, Guid.Parse(tn.Id));
        }

        public Document FirstDocument(PLMPackEntities db)
        {
            TreeNodeDocument treeNodeDoc = db.TreeNodeDocuments.Single(tnd => tnd.TreeNodeId == this.Id);
            return db.Documents.Single(d => d.Id == treeNodeDoc.DocumentId);
            /*
            get
            {
                if (TreeNodeDocuments.Count > 0)
                    return TreeNodeDocuments.First().Document;
                else
                    return null;
            }
            */ 
        }

        public Component FirstComponent(PLMPackEntities db)
        {
                Document doc = FirstDocument(db);
                if (null != doc)
                    return db.Components.Single(comp => comp.DocumentId == doc.Id);
                else
                    return null;
        }
        #endregion

        #region Path / searching
        public string GetPath(PLMPackEntities db)
        {
            return null == ParentNode ? Name : ParentNode.GetPath(db) + "\\" + Name;
        }
        #endregion
        #endregion
    }
    #endregion

    #region Tree
    public class Tree
    {
        static public void Print(PLMPackEntities db, AspNetUser user)
        {
            Console.WriteLine(string.Format("### Tree for user : {0} ###", user.UserName));

            TreeNode[] roots = TreeNode.GetRootNodes(db, user);
            if (roots.Count() == 0)
            {
                Console.WriteLine("No root treenodes found...");
                return;
            }
            else
            {
                foreach (TreeNode tn in roots)
                    tn.Print(db, user, "");
            }

            Console.WriteLine("###");
        }
    }
    #endregion

    #region Group
    public partial class Group
    {
        public static Group CreateNew(PLMPackEntities db, string groupName, string groupDesc, AspNetUser user)
        {
            // does group name already exist
            if (db.Groups.Count(gp => gp.GroupName.ToLower() == groupName.ToLower()) > 0)
                throw new ModelException(string.Format("A group with name {0} already exist.", groupName));

            string grpId = Guid.NewGuid().ToString();

            // create group
            Group grp = new Group();
            grp.Id = grpId;
            grp.GroupName = groupName;
            grp.GroupDesc = groupDesc;
            grp.UserId = user.Id;
            grp.DateCreated = DateTime.Now;
            db.Groups.Add(grp);
            db.SaveChanges();
            Group grpNew = db.Groups.Single(g => g.Id == grpId);

            grpNew.AspNetUsers.Add(user);
            user.GroupsOfInterest.Add(grpNew);
            db.SaveChanges();

            // create treeNode rootNode
            TreeNode tn = TreeNode.CreateNew(db
                , grpId
                , string.Empty
                , groupName, groupDesc
                , Thumbnail.DefaultFolder(db).Id);

            return db.Groups.Single(g => g.Id == grpId);
        }
        public static Group GetById(PLMPackEntities db, string id)
        {
            return db.Groups.Single(gp => gp.Id == id);
        }
        public static bool Exist(PLMPackEntities db, string name)
        {
            return db.Groups.Count(g => g.GroupName.ToLower() == name.ToLower()) > 0;
        }
        public static Group GetByName(PLMPackEntities db, string name)
        {
            return db.Groups.Single(g => g.GroupName.ToLower() == name.ToLower());
        }
        public static Group Everyone(PLMPackEntities db)
        {
            return Group.GetByName(db, "Everyone");
        }
    }
    #endregion

    #region File
    public partial class File
    {
        #region Static methods
        public static File CreateNew(PLMPackEntities db, Guid g, string extension)
        {
            string sGuid = g.ToString();
            // create file
            File fn = new File();
            fn.Guid = sGuid;
            fn.Extension = extension;
            fn.DateCreated = DateTime.Now;
            db.Files.Add(fn);
            db.SaveChanges();
            // return file
            return db.Files.Single(f => f.Guid == sGuid);
        }
        public static File GetById(PLMPackEntities db, Guid g)
        {
            return db.Files.Single(f => f.Guid == g.ToString());
        }
        #endregion
        #region Delete method
        public void Delete(PLMPackEntities db)
        {
            db.Files.Remove(this);
        }
        #endregion
    }
    #endregion

    #region Thumbnail
    public partial class Thumbnail
    {
        #region Static methods
        public static void Initialize(PLMPackEntities db)
        {
            if (0 == db.Thumbnails.Count())
            {
                foreach (KeyValuePair<string, string> entry in _tbDictionary)
                    Thumbnail.CreateNew(db, new Guid(entry.Value), "png");
            }
        }

        public static Thumbnail CreateNew(PLMPackEntities db, Guid g, string ext)
        {
            // create thumbnail
            Thumbnail tb = new Thumbnail();
            tb.File = File.CreateNew(db, g, ext);
            tb.Width = 150;
            tb.Height = 150;
            tb.MimeType = Ext2MimeType(ext);
            db.Thumbnails.Add(tb);
            db.SaveChanges();

            int tid = tb.Id;
            return db.Thumbnails.Single(t => t.Id == tid);
        }

        public static Thumbnail CreateNew(PLMPackEntities db, Guid fileGuid)
        {
            File f = File.GetById(db, fileGuid);
            Thumbnail tb = new Thumbnail() { Width = 150, Height = 150, FileGuid = fileGuid.ToString(), MimeType = Ext2MimeType(f.Extension) };
            db.Thumbnails.Add(tb);
            db.SaveChanges();

            int tid = tb.Id;
            return db.Thumbnails.Single(t => t.Id == tid);
        }

        public static Thumbnail DefaultFolder(PLMPackEntities db)
        {
            return db.Thumbnails.Single(tb => tb.Id == 0);
        }

        public static Thumbnail GetByID(PLMPackEntities db, int id)
        {
            return db.Thumbnails.Single(tb => tb.Id == id);
        }

        public static Thumbnail GetByGuid(PLMPackEntities db, Guid g)
        {
            return db.Thumbnails.Single(tb => tb.FileGuid == g.ToString());
        }

        public static void DeleteIfNotRefered(PLMPackEntities db, int id)
        {
            if (0 == db.TreeNodes.Count(tn => tn.ThumbnailId == id))
            {
                Thumbnail thumb = db.Thumbnails.Single(tb => tb.Id == id);
                // delete thumbnail
                string fileGuid = thumb.FileGuid;
                db.Thumbnails.Remove(thumb);
                db.SaveChanges();
                // delete file
                File f = File.GetById(db, Guid.Parse(fileGuid));
                f.Delete(db);
            }
        }

        public static Thumbnail GetDefaultThumbnail(PLMPackEntities db, string defName)
        {
            return db.Thumbnails.Single(tb => tb.FileGuid == _tbDictionary[defName]);
        }

        public static string Ext2MimeType(string fileExt)
        {
            switch (fileExt.ToLower().Trim('.'))
            {
                case "bmp": return "image/bmp";
                case "jpg": return "image/jpeg";
                case "jpeg": return "image/jpeg";
                case "png": return "image/png";
                default: throw new ModelException(string.Format("Unknown image extension : {0}", fileExt));
            }
        }
        #endregion
        #region Data members
        private static Dictionary<string, string> _tbDictionary = new Dictionary<string, string>()
        {
            {"AI", "170d61ac-f1fe-47c4-91d2-34fa77809a80"},
            {"ARD", "a35e4d06-4690-4b39-937c-f82c97f83ed7"},
            {"CALC", "affbf3ec-cca4-4ebe-87c7-03960a7134d6"},
            {"CCF2", "12b59ba9-6367-4e4f-98d8-11abaa26ba5d"},
            {"DXF", "527bfef2-db22-4189-93d1-27ce43443fc3"},
            {"EPS", "9742cede-798c-4ad0-9481-7cd8373ad1df"},
            {"EXCEL", "a35e4d06-4690-4b39-937c-f82c97f83ed7"},
            {"FOLDER", "affbf3ec-cca4-4ebe-87c7-03960a7134d6"},
            {"IMAGE", "12b59ba9-6367-4e4f-98d8-11abaa26ba5d"},
            {"PDF", "527bfef2-db22-4189-93d1-27ce43443fc3"},
            {"DES3", "50195740-8652-41b0-a767-c11c08c44acd"},
            {"DES", "bd2dffaf-a48c-40c8-9754-010d6e1bb665"},
            {"PPT", "60dc54c0-c872-4bf9-9e6d-3bf89ac0433f"},
            {"WORD", "f9c38346-8484-4c7b-b92f-ba1b54446348"},
            {"WRITER", "bcc3b88b-efa8-4885-813f-4d9cbedc6831"}
        };
        #endregion
    }
    #endregion

    #region Document
    public partial class Document
    {
        #region Static methods
        public static Document CreateNew(PLMPackEntities db, string groupId
            , string docType, string name, string description
            , Guid fileGuid, string fileExt)
        {
            // create document
            Document doc = new Document()
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                DocumentType = docType,
                FileGuid = fileGuid.ToString(),
                GroupId = groupId
            };
            db.Documents.Add(doc);
            db.SaveChanges();
            return doc;
        }
        public static Document GetByID(PLMPackEntities db, string sId)
        {
            return db.Documents.Single(d => d.Id == sId);
        }
        #endregion

        #region Non static methods
        public bool IsComponent
        {
            get { return Components.Count > 0; }
        }

        public void Delete(PLMPackEntities db)
        {
            // remove tree node documents
            var tnDocs = db.TreeNodeDocuments.Where(tnd => tnd.DocumentId == this.Id);
            foreach (TreeNodeDocument tnd in tnDocs)
            {
                db.TreeNodeDocuments.Remove(tnd);
                db.TreeNodes.Remove(db.TreeNodes.Single(tn => tn.Id == tnd.TreeNodeId));
                db.SaveChanges();
            }
            Guid fGuid = Guid.Parse(FileGuid);
            // actually remove document
            db.Documents.Remove(this);
            db.SaveChanges();
            // remove file
            File f = File.GetById(db, fGuid);
            f.Delete(db);
        }
        #endregion
    }
    #endregion

    #region Component
    public partial class Component
    {
        #region Static methods
        public static Component CreateNew(PLMPackEntities db, string grpId
            , string name, string description
            , Guid fileGuid, Guid compGuid)
        {
            Document doc = Document.CreateNew(db, grpId, "DT_COMPONENT", name, description, fileGuid, "dll");
            Component cp = new Component() { DocumentId = doc.Id, Guid = compGuid.ToString() };
            db.Components.Add(cp);
            db.SaveChanges();

            return GetByGuid(db, compGuid);
        }
        public static Component GetByGuid(PLMPackEntities db, Guid g)
        {
            return db.Components.Single(c => c.Guid == g.ToString());
        }
        #endregion

        #region Non static methods
        #region Majoration sets
        public Dictionary<string, double> GetMajorationSet(PLMPackEntities db, CardboardProfile cf)
        {
            if (db.MajorationSets.Count(
                mjs => (mjs.ComponentGuid == Guid) && (mjs.CardboardProfile.GroupId == cf.GroupId) ) == 0)
            {
                // dict majo
                db.MajorationSets.Add( new MajorationSet()
                    {
                        ComponentGuid = Guid,
                        CardboardProfileId = cf.Id
                    }
                    );
                db.SaveChanges();
                // build list of majo
                var majoSets = db.MajorationSets.Where(mjs => (mjs.ComponentGuid == this.Guid) && (mjs.CardboardProfile.GroupId == this.Document.GroupId));
                MajorationSet mjsNearest = null; double diffMax = double.MaxValue;
                foreach (MajorationSet mjset in majoSets)
                {
                    double thickness = mjset.CardboardProfile.Thickness;
                    if (Math.Abs(thickness - cf.Thickness) < diffMax)
                    {
                        mjsNearest = mjset;
                        diffMax = Math.Abs(thickness - cf.Thickness);
                    }
                    
                }
                MajorationSet mjsCurrent = MajorationSet.Single(mjs => (mjs.ComponentGuid == this.Guid) && (mjs.CardboardProfileId == cf.Id));
                double thicknessNearest = mjsNearest.CardboardProfile.Thickness;
                foreach (Majoration mj in mjsNearest.Majorations)
                {
                    db.Majorations.Add(
                        new Majoration()
                        {
                            MajorationSetId = mjsCurrent.Id,
                            Name = mj.Name,
                            Value = mj.Value * cf.Thickness / thicknessNearest
                        }
                        );
                }
                db.SaveChanges();
            }
            Dictionary<string, double> dictMajo = new Dictionary<string,double>();
            MajorationSet majoSet = MajorationSet.Single();
            return dictMajo;            
        }
        public void UpdateMajorationSet(PLMPackEntities db, CardboardProfile cp, Dictionary<string, double> majorations)
        {
            // create majoration set if it does not exists
            if (db.MajorationSets.Count(mjs => (mjs.ComponentGuid == this.Guid) && (mjs.CardboardProfileId == cp.Id)) == 0)
            {
                db.MajorationSets.Add(new MajorationSet()
                    {
                        Id = System.Guid.NewGuid().ToString(),
                        ComponentGuid = this.Guid,
                        CardboardProfileId = cp.Id
                    });
                db.SaveChanges();
            }
            // retrieve majoration set
            MajorationSet majoSet = db.MajorationSets.Single(mjs => (mjs.ComponentGuid == this.Guid) && (mjs.CardboardProfileId == cp.Id));

            // delete any existing majorations
            foreach (Majoration majo in db.Majorations.Where(m => m.MajorationSetId == majoSet.Id))
                db.Majorations.Remove(majo);
            db.SaveChanges();

            // (re)create majorations
            foreach (KeyValuePair<string, double> entry in majorations)
                majoSet.Majorations.Add(new Majoration() { Name = entry.Key, Value = entry.Value });
            db.SaveChanges();
        }
        #endregion
        #region Param default value
        public Dictionary<string, double> GetParamDefaultValues(PLMPackEntities db, string grpId)
        {
            if (db.ParamDefaultComponents.Count(pdc => (pdc.GroupId == grpId) && (pdc.ComponentGuid == this.Guid)) == 0)
            {
                // get component group
                string compGrpId = this.Document.Group.Id;
                // get param default components
                var paramDefaults1 = db.ParamDefaultComponents.Where(pdc => (pdc.GroupId == grpId) && (pdc.ComponentGuid == this.Guid));
                foreach (ParamDefaultComponent pdc in paramDefaults1)
                {
                    db.ParamDefaultComponents.Add(
                        new ParamDefaultComponent()
                        {
                            ComponentGuid = this.Guid,
                            GroupId = pdc.GroupId,
                            Name=pdc.Name,
                            Value = pdc.Value
                        }
                        );
                }
                db.SaveChanges();
            }
            Dictionary<string, double> defaultParamValues = new Dictionary<string, double>();
            var paramDefaults2 = this.ParamDefaultComponents.Where(
                pdc => (pdc.GroupId == grpId) && (pdc.ComponentGuid == this.Guid)
                );
            foreach (ParamDefaultComponent pdc in paramDefaults2)
                defaultParamValues.Add(pdc.Name, pdc.Value);
            return defaultParamValues;
        }
        public double GetParamDefaultValueDouble(string grpId, string name)
        {
            ParamDefaultComponent paramDefValue = this.ParamDefaultComponents.Single(
                pdc => pdc.GroupId == grpId
                && pdc.Name == name);
            return paramDefValue.Value;
        }
        public void InsertParamDefaultValue(PLMPackEntities db, string grpId, string name, double value)
        {
            if (db.ParamDefaultComponents.Count(
                pdc => (pdc.ComponentGuid == Guid) && (pdc.GroupId == grpId) && (pdc.Name == name)) > 0)
            {
                ParamDefaultComponent paramDefValue = db.ParamDefaultComponents.Single(
                    pdc => (pdc.ComponentGuid == Guid) && (pdc.GroupId == grpId) && (pdc.Name == name));
                paramDefValue.Value = value;
            }
            else
            {
                db.ParamDefaultComponents.Add( new ParamDefaultComponent()
                    {
                        ComponentGuid = Guid,
                        GroupId = grpId,
                        Name = name,
                        Value = value
                    }
                );
            }
            db.SaveChanges();
        }
        #endregion
        #region Print
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(
                string.Format("Component Guid = {0}, Name = {1}"
                , this.Guid, this.Document.Name)
                );
            foreach (MajorationSet majSet in this.MajorationSet)
            {
                sb.AppendLine(string.Format("Profile {0} ({1})", majSet.CardboardProfile.Name, majSet.CardboardProfile.Thickness));
                foreach (Majoration m in majSet.Majorations)
                    sb.AppendLine(string.Format("{0}, {1}", m.Name, m.Value));
            }
            return sb.ToString();
        }
        #endregion
        #region Delete
        public void Delete(PLMPackEntities db, string grpId)
        {
            foreach (MajorationSet majoSet in db.MajorationSets.Where(ms => ms.ComponentGuid == Guid))
            {
                foreach (Majoration majo in db.Majorations.Where(m => m.MajorationSetId == majoSet.Id))
                    db.Majorations.Remove(majo);
                db.MajorationSets.Remove(majoSet);
            }

        }
        #endregion
        #endregion
    }
    #endregion

    #region TreeNodeDocument
    public partial class TreeNodeDocument
    {
        public void Delete(PLMPackEntities db)
        {
            string documentId = this.DocumentId;
            // remove TreeNodeDocuments
            db.TreeNodeDocuments.Remove(this);
            db.SaveChanges();
            // delete document
            Document doc = Document.GetByID(db, documentId);
            if (null != doc) doc.Delete(db);
        }
    }
    #endregion

    #region CardboardFormat
    public partial class CardboardFormat
    {
        #region Static methods
        public static bool Exists(PLMPackEntities db, Group gp, string name)
        {
            return db.CardboardFormats.Count(
                cbf => (cbf.Group.Id == gp.Id) && (cbf.Name.ToLower() == name.ToLower())
                ) > 0;
        }
        public static CardboardFormat CreateNew(
            PLMPackEntities db, Group gp
            , string name, string description
            , double length, double width)
        {
            if (CardboardFormat.Exists(db, gp, name))
                throw new ModelException(string.Format("Cardboad format {0} already exists.", name));
            CardboardFormat cardboardFormat = new CardboardFormat();
            cardboardFormat.Group = gp;
            cardboardFormat.Name = name;
            cardboardFormat.Description = description;
            cardboardFormat.Length = length;
            cardboardFormat.Width = width;
            db.CardboardFormats.Add(cardboardFormat);
            db.SaveChanges();
            return cardboardFormat;
        }
        public static CardboardFormat GetById(PLMPackEntities db, int id)
        {
            return db.CardboardFormats.Single(c => c.Id == id);
        }
        public static CardboardFormat GetByName(PLMPackEntities db, Group gp, string name)
        {
            return db.CardboardFormats.Single(c => (c.Name.ToLower() == name.ToLower()) && (gp.Id == c.GroupId));
        }
        public static CardboardFormat[] GetAll(PLMPackEntities db, Group gp)
        {
            return db.CardboardFormats.Where(dbf => dbf.Group == gp).ToArray();
        }
        public static void PrintAll(PLMPackEntities db, Group gp)
        {
            Console.WriteLine(string.Format("### Cardboard formats of Group {0}", gp.GroupName));
            var cfs = db.CardboardFormats.Where(c => c.GroupId == gp.Id);
            foreach (CardboardFormat cf in cfs)
                cf.Print();
        }
        #endregion

        #region Non static members
        public void Print()
        {
            Console.WriteLine(string.Format("{0} {1} {2} {3}", Name, Description, Length, Width));
        }
        public void Delete(PLMPackEntities db)
        {
            db.CardboardFormats.Remove(this);
            db.SaveChanges();
        }
        #endregion
    }
    #endregion

    #region CardboardProfile
    public partial class CardboardProfile
    {
        #region Static methods
        public static bool Exists(PLMPackEntities db, Group gp, string name)
        {
            return db.CardboardProfiles.Count(
                cp => (cp.GroupId == gp.Id) && (cp.Name.ToLower() == name.ToLower())
                ) > 0;
        }
        public static CardboardProfile CreateNew(
            PLMPackEntities db, Group gp
            , string name, string description
            , string code, double thickness)
        {
            if (CardboardProfile.Exists(db, gp, name))
                throw new ModelException(string.Format("Cardboard profile \'{0}\' already exist.", name));
            CardboardProfile cp = new CardboardProfile();
            cp.Group = gp;
            cp.Name = name;
            cp.Description = description;
            cp.Code = code;
            cp.Thickness = thickness;
            db.CardboardProfiles.Add(cp);
            db.SaveChanges();
            return cp;
        }
        public static CardboardProfile[] GetAll(PLMPackEntities db, Group gp)
        {
            return db.CardboardProfiles.Where(dbp => dbp.Group == gp).ToArray();
        }
        public static CardboardProfile GetByID(PLMPackEntities db, int id)
        {
            return db.CardboardProfiles.Single(dbp => dbp.Id == id);
        }
        public static CardboardProfile GetByName(PLMPackEntities db, Group gp, string name)
        {
            return db.CardboardProfiles.Single(
                cp => (cp.GroupId == gp.Id) && (cp.Name.ToLower() == name.ToLower())
                );
        }
        public static void PrintAll(PLMPackEntities db, Group gp)
        {
            Console.WriteLine("Cardboard profiles");
            var cps = db.CardboardProfiles.Where(cp => cp.GroupId == gp.Id);
            foreach (CardboardProfile cp in cps)
                cp.Print();
        }
        #endregion

        #region Non static methods
        public void Print()
        {
            Console.WriteLine(string.Format("{0} {1} {2}", Name, Code, Thickness));
        }

        public void Delete(PLMPackEntities db)
        {
            // remove all cardboard qualities that use this cardboard
            db.CardboardQualities.RemoveRange(CardboardQualities);
            // remove cardboard profile itself
            db.CardboardProfiles.Remove(this);
            db.SaveChanges();
        }
        #endregion
    }
    #endregion

    #region CardboardQuality
    public partial class CardboardQuality
    {
        #region Static methods
        public static bool Exists(PLMPackEntities db, Group gp, string name)
        {
            return db.CardboardQualities.Count(
                cq => (cq.CardboardProfile.GroupId == gp.Id) && (cq.Name.ToLower() == name.ToLower())
                ) > 0;
        }
        public static CardboardQuality CreateNew(PLMPackEntities db, Group gp
            , string name, string description
            , int profileId
            , double surfacicMass, double rigidityX, double rigidityY
            , double youngModulus, double ect)
        {
            if (CardboardQuality.Exists(db, gp, name))
                throw new ModelException(string.Format("Cardboard quality \'{0}\' already exist.", name));
            CardboardQuality cq = new CardboardQuality();
            cq.Name = name;
            cq.Description = description;
            cq.CardboardProfileId = profileId;
            cq.SurfacicMass = surfacicMass;
            cq.RigidityX = rigidityX;
            cq.RigidityY = rigidityY;
            cq.YoungModulus = youngModulus;
            cq.ECT = ect;
            db.CardboardQualities.Add(cq);
            db.SaveChanges();
            return cq;
        }
        public static CardboardQuality GetByID(PLMPackEntities db, int id)
        {
            return db.CardboardQualities.Single(cq => cq.Id == id);
        }
        public static CardboardQuality[] GetAll(PLMPackEntities db, Group gp)
        {
            return db.CardboardQualities.Where(cq => cq.CardboardProfile.GroupId == gp.Id).ToArray();
        }
        public static CardboardQuality[] GetAllByProfileID(PLMPackEntities db, int profileId)
        {
            return db.CardboardQualities.Where(cq => cq.CardboardProfileId == profileId).ToArray();
        }
        public static void PrintAll(PLMPackEntities db, Group gp)
        {
            Console.WriteLine("Cardboard qualities");
            var cqs = db.CardboardQualities.Where(cp => cp.CardboardProfile.GroupId == gp.Id);
            foreach (CardboardQuality cp in cqs)
                cp.Print();
        }
        #endregion

        #region Non static methods
        public void Print()
        {
            Console.WriteLine(string.Format("{0} {1}", Name, CardboardProfile.Name));
        }
        public void Delete(PLMPackEntities db)
        {
            // remove all cardboard quality
            db.CardboardQualities.Remove(this);
            db.SaveChanges();
        }
        #endregion
    }
    #endregion

    #region User connection
    public partial class UserConnection
    {
    }
    #endregion
}
