using System;
using System.Collections;
using System.Windows.Forms;
using FileExplorer.Shell;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Node sorter that implements the IComparer interface.
    /// </summary>
    internal sealed class ExplorerTreeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode nodeX = x as TreeNode;
            TreeNode nodeY = y as TreeNode;
            ShellItem shX = (ShellItem)nodeX.Tag;
            ShellItem shY = (ShellItem)nodeY.Tag;

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
