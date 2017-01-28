using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    internal sealed class VirtualTreeViewSorter : IComparer<TreeNode>
    {
        public int Compare(TreeNode x, TreeNode y)
        {
            VirtualItem shX = (VirtualItem)x.Tag;
            VirtualItem shY = (VirtualItem)y.Tag;

            if (shX != null && shY != null)
                return shY.CompareTo(shX);
            else if (shX != null)
                return 1;
            else if (shY != null)
                return -1;
            else
                return 0;
        }
    }
}
