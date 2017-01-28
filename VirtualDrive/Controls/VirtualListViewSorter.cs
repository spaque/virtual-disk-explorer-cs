using System;
using System.Windows.Forms;
using System.Collections.Generic;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    /// <summary>
    /// ListView sorter that implements the IComparer interface.
    /// </summary>
    internal sealed class VirtualListViewSorter : IComparer<ListViewItem>
    {
        public int Compare(ListViewItem x, ListViewItem y)
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
