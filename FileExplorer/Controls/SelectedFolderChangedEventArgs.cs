using System;
using System.Windows.Forms;
using System.Text;
using FileExplorer.Shell;

namespace FileExplorer.Controls
{
    public class SelectedFolderChangedEventArgs : EventArgs
    {
        private TreeNode newNode;
        private TreeNode oldNode;
        private bool refresh;

        public SelectedFolderChangedEventArgs(TreeNode newNode, TreeNode oldNode, bool refresh)
        {
            this.newNode = newNode;
            this.oldNode = oldNode;
            this.refresh = refresh;
        }

        public TreeNode OldNode { get { return oldNode; } }

        public TreeNode NewNode { get { return newNode; } }

        public bool Refresh { get { return refresh; } }
    }
}
