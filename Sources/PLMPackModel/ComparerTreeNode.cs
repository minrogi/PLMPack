#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace PLMPackModel
{
    class ComparerTreeNode
        :IComparer<TreeNode>
    {
        /// <summary>
        /// Compares two instances of <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="treeNode1">First instance of <see cref="TreeNode"/>.</param>
        /// <param name="treeNode2">Second instance of <see cref="TreeNode"/>.</param>
        /// <returns><see cref="int"/> indicating order of two instances.</returns>
        public int Compare(TreeNode treeNode1, TreeNode treeNode2)
        {
            return string.Compare(treeNode1.Name, treeNode2.Name);
        }
    }
}
