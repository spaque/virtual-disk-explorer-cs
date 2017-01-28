using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using FileExplorer.Shell;

namespace FileExplorer.Controls
{
    /// <summary>
    /// ListView sorter that implements the IComparer interface.
    /// </summary>
    internal sealed class ExplorerListSorter : IComparer<ListViewItem>
    {
        public int Compare(ListViewItem x, ListViewItem y)
        {
            ShellItem shX = (ShellItem)x.Tag;
            ShellItem shY = (ShellItem)y.Tag;

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
