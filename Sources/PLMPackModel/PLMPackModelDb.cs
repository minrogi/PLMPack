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
    }
    #endregion

    #region TreeNode
    public partial class TreeNode
    {
        public static TreeNode CreateNew(PLMPackEntities db, string grpId
            , string parentNodeId
            , string name, string description
            , int thumbId)
        {
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
            return db.TreeNodes.Single(tn => tn.Id == id.ToString());
        }

        public static TreeNode[] GetRootNodes(PLMPackEntities db, AspNetUser user)
        {
            TreeNode[] rootNodes = db.TreeNodes.Where(tn => string.IsNullOrEmpty(tn.ParentNodeId)).ToArray();
            if (0 == rootNodes.Count())
            { 
            
            }
            return rootNodes;
        }
    }
    #endregion

    #region Group
    public partial class Group
    {
        public static Group CreateNew(PLMPackEntities db, string groupName, string groupDesc, AspNetUser user)
        {
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
    }
    #endregion

    #region File
    public partial class File
    {
        public static File CreateNew(PLMPackEntities db, string extension)
        {
            // guid
            string sGuid = System.Guid.NewGuid().ToString();
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
    }
    #endregion

    #region Thumbnail
    public partial class Thumbnail
    {
        public static void Initialize(PLMPackEntities db)
        {
            if (0 == db.Thumbnails.Count())
            { 
                // create thumbnail
                Thumbnail tb = Thumbnail.CreateNew(db, "png");
            }
        }

        public static Thumbnail CreateNew(PLMPackEntities db, string ext)
        {
            // create thumbnail
            Thumbnail tb = new Thumbnail();
            tb.File = File.CreateNew(db, ext);
            tb.Width = 150;
            tb.Height = 150;
            tb.MimeType = ext;
            db.Thumbnails.Add(tb);
            db.SaveChanges();

            int tid = tb.Id;
            return db.Thumbnails.Single(t => t.Id == tid);
        }

        public static Thumbnail DefaultFolder(PLMPackEntities db)
        {
            return db.Thumbnails.Single(tb => tb.Id == 0);
        }
    }
    #endregion
}
