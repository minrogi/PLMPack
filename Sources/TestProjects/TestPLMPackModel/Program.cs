#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity.Validation;
#endregion

namespace PLMPackModel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string user1 = "9e94b280-0cb1-455b-9eff-4412f243b71a";
                {
                    PLMPackEntities db = new PLMPackEntities();
                    // thumbnails initialize
                    Thumbnail.Initialize(db);
                    // get user by id
                    AspNetUser user = AspNetUser.GetById(db, user1);
                    user.Connect(db);
                    Console.WriteLine(string.Format("### Connected as user {0}", user.UserName));

                    // create group "treeDiM" 
                    if (!Group.Exist(db, "treeDiM"))
                        Group.CreateNew(db, "treeDiM", "treeDiM", user);
                    // create group "Everyone"
                    if (!Group.Exist(db, "Everyone"))
                        Group.CreateNew(db, "Everyone", "Everyone", user);

                    // get user group
                    Group grp1 = user.CurrentGroup(db);
                    Console.WriteLine(string.Format("Current group is: {0}", grp1.GroupName));

                    // add cardboard format
                    if (!CardboardFormat.Exists(db, grp1, "Cardboard 1"))
                        CardboardFormat.CreateNew(db, grp1, "Cardboard 1", "Cardboard 1 desc.", 2200, 1600);
                    if (!CardboardFormat.Exists(db, grp1, "Cardboard 2"))
                        CardboardFormat.CreateNew(db, grp1, "Cardboard 2", "Cardboard 2 desc.", 2200, 1600);
                    CardboardFormat.PrintAll(db, grp1);

                    // add cardboard profile
                    CardboardProfile prof1 = null;
                    if (!CardboardProfile.Exists(db, grp1, "Profile 1"))
                        prof1 = CardboardProfile.CreateNew(db, grp1, "Profile 1","", "PRO1", 1.0);
                    else
                        prof1 = CardboardProfile.GetByName(db, grp1, "Profile 1");
                    CardboardProfile prof2 = null;
                    if (!CardboardProfile.Exists(db, grp1, "Profile 2"))
                        prof2 = CardboardProfile.CreateNew(db, grp1, "Profile 2","", "PRO2", 2.0);
                    else
                        prof2 = CardboardProfile.GetByName(db, grp1, "Profile 2");
                    CardboardProfile.PrintAll(db, grp1);

                    // add cardboard quality
                    CardboardQuality q1 = null;
                    if (!CardboardQuality.Exists(db, grp1, "Quality 1"))
                        q1 = CardboardQuality.CreateNew(db, grp1, "Quality 1", "Quality 1 based on Profile 1", prof1.Id, 0.1, 1000.0, 1000.0, 1000.0, 10.0);
                    CardboardQuality q2 = null;
                    if (!CardboardQuality.Exists(db, grp1, "Quality 2"))
                        q2 = CardboardQuality.CreateNew(db, grp1, "Quality 2", "Quality 2 based on Profile 2", prof2.Id, 0.2, 2000.0, 2000.0, 2000.0, 20.0);
                    CardboardQuality.PrintAll(db, grp1);

                    // get root tree node
                    TreeNode[] roots = TreeNode.GetRootNodes(db, user);

                    // create thumbnail
                    Thumbnail thumb = Thumbnail.DefaultFolder(db);
                    // insert new treeNode Test
                    string name = "Test Node";
                    string description = "Test tree node";
                    if (roots[0].HasChildWithName(name))
                    {
                        Console.WriteLine(string.Format("Node {0} already has child node {1}", roots[0].Name, name));
                        List<TreeNode> childrens = new List<TreeNode>(roots[0].Childrens);
                        childrens.ForEach(tn => tn.Delete(db, user));
                    }

                    // share root node with group everyone
                    roots[0].Share(db, user);

                    // insert new node with default folder
                    TreeNode tnNew = roots[0].CreateChild(db, grp1.Id, name, description, Thumbnail.DefaultFolder(db));
                    tnNew.Share(db, user);
                    // insert document
                    string docName = "TestDoc";
                    string docDesc = "Test document";
                    string docType = "PICGEOM";
                    Guid docGuid = Guid.NewGuid();
                    string docExt = "des";
                    TreeNode tnDoc = tnNew.InsertDocument(db, grp1.Id, docName, docDesc, docType, docGuid, docExt, Thumbnail.DefaultFolder(db));
                    string componentName = "TestComp";
                    string componentDesc = "Test component";
                    TreeNode tnDocComp = tnNew.InsertComponent(db, grp1.Id
                        , componentName, componentDesc
                        , Guid.NewGuid(), Guid.NewGuid()
                        , Thumbnail.DefaultFolder(db));

                    // disconnect
                    user.Disconnect(db);
                }
                {
                    PLMPackEntities db = new PLMPackEntities();
                    // thumbnails initialize
                    Thumbnail.Initialize(db);
                    // get user by id
                    AspNetUser user = AspNetUser.GetById(db, user1);
                    // show tree
                    Tree.Print(db, user);
                }

                string user2 = "7142ebab-5a02-414d-b6ba-a676fb9f7287";
                {
                    PLMPackEntities db = new PLMPackEntities();
                    // thumbnails initialize
                    Thumbnail.Initialize(db);
                    // get user by id
                    AspNetUser user = AspNetUser.GetById(db, user2);
                    user.Connect(db);

                    Console.WriteLine(string.Format("### Connected as user {0}", user.UserName));
                    // add group of interest to current user
                    user.AddGroupOfInterest(db, Group.GetByName(db, "Anastasia_grp"));
                    // show tree
                    Tree.Print(db, user);
                    user.RemoveGroupOfInterest(db, Group.GetByName(db, "Anastasia_grp"));

                    // diconnect
                    user.Disconnect(db);
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
