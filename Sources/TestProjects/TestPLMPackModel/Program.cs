#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace PLMPackModel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string userId = "9e94b280-0cb1-455b-9eff-4412f243b71a";
                PLMPackEntities db = new PLMPackEntities();
                // thumbnails initialize
                Thumbnail.Initialize(db);
                // get user by id
                AspNetUser user = AspNetUser.GetById(db, userId);
                Console.WriteLine(string.Format("Found user {0}", user.UserName));
                // get user group
                Group grp = user.CurrentGroup(db);
                Console.WriteLine(string.Format("Current group is: {0}", grp.GroupName));
                // get root tree node
                TreeNode[] roots = TreeNode.GetRootNodes(db, user);
                if (roots.Count() == 0)
                    Console.WriteLine("No root treenodes found...");
                else
                {
                    Console.WriteLine("Root treeNodes: ");
                    foreach (TreeNode tn in roots)
                        Console.WriteLine(tn.Name);
                }
                // insert new treeNode
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
