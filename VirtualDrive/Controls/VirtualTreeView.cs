using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using FileExplorer.Shell;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    internal class VirtualTreeView : TreeView
    {
        #region Fields

        private TreeNode root;
        private ShellImageList imageList;
        private VirtualTreeViewSorter sorter;

        #endregion

        #region Constructor

        public VirtualTreeView()
            : base()
        {
            imageList = ShellImageList.Instance;
            sorter = new VirtualTreeViewSorter();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            HotTracking = true;
            HideSelection = false;
            ShowLines = false;
            HandleCreated += new EventHandler(VirtualTreeView_HandleCreated);
            VisibleChanged += new EventHandler(VirtualTreeView_VisibleChanged);
            BeforeCollapse += new TreeViewCancelEventHandler(VirtualTreeView_BeforeCollapse);
            BeforeExpand += new TreeViewCancelEventHandler(VirtualTreeView_BeforeExpand);
        }

        #endregion

        #region Properties

        public TreeNode Root { get { return root; } }

        #endregion

        #region Events

        void VirtualTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            Cursor = Cursors.WaitCursor;
            VirtualItem item = (VirtualItem)node.Tag;
            Enabled = false;
            if (node != root)
            {
                BeginUpdate();
                node.Nodes.Clear();
                List<VirtualItem> subFolders = item.GetFolders(true);
                TreeNode[] nodes = new TreeNode[subFolders.Count];
                for (int i = 0; i < subFolders.Count; i++)
                {
                    nodes[i] = MakeNode(subFolders[i]);
                }
                Array.Sort(nodes, sorter);
                node.Nodes.AddRange(nodes);
                EndUpdate();
            }
            Enabled = true;
            Cursor = Cursors.Default;
        }

        void VirtualTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Equals(root))
                e.Cancel = true;
        }

        void VirtualTreeView_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                imageList.SetSmallImageList(this);
                if (root != null)
                {
                    root.Expand();
                    if (SelectedNode != null)
                        SelectedNode.Expand();
                    else
                        SelectedNode = root;
                }
            }
        }

        void VirtualTreeView_HandleCreated(object sender, EventArgs e)
        {
            imageList.SetSmallImageList(this);
        }

        #endregion

        #region Public Methods

        public void LoadRoot(VirtualItem rootItem)
        {
            List<VirtualItem> subDirs = rootItem.GetFolders(true);
            TreeNode[] nodes = new TreeNode[subDirs.Count];
            root = new TreeNode(rootItem.Text);
            root.ImageIndex = rootItem.ImageIndex;
            root.SelectedImageIndex = rootItem.ImageIndex;
            root.Tag = rootItem;
            for (int i = 0; i < subDirs.Count; i++)
                nodes[i] = MakeNode(subDirs[i]);
            root.Nodes.AddRange(nodes);
            Nodes.Clear();
            Nodes.Add(root);
            root.Expand();
            SelectedNode = root;
        }

        public void Unload()
        {
            Nodes.Clear();
            root = null;
        }

        public void ExpandSubNode(String text)
        {
            if (SelectedNode == null)
                return;
            TreeNodeCollection nodes = SelectedNode.Nodes;
            if ((nodes.Count == 1) && (nodes[0].Text.Equals(" : ")))
                SelectedNode.Expand();
            foreach (TreeNode tn in nodes)
            {
                if (tn.Text == text)
                {
                    SelectedNode = tn;
                    break;
                }
            }
        }

        public void navigateUp()
        {
            TreeNode parent = SelectedNode.Parent;
            if (parent != null)
            {
                SelectedNode = parent;
            }
        }

        public void RefreshNode(TreeNode node)
        {
            if (node == null)
                return;
            if (!(node.Nodes.Count == 1 && node.Nodes[0].Text.Equals(" : ")))
            {
                Cursor = Cursors.WaitCursor;
                VirtualItem item = (VirtualItem)node.Tag;
                Enabled = false;
                BeginUpdate();
                node.Nodes.Clear();
                List<VirtualItem> subFolders = item.GetFolders(true);
                TreeNode[] nodes = new TreeNode[subFolders.Count];
                for (int i = 0; i < subFolders.Count; i++)
                {
                    nodes[i] = MakeNode(subFolders[i]);
                }
                Array.Sort(nodes, sorter);
                node.Nodes.AddRange(nodes);
                EndUpdate();
                Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Private Methods

        private TreeNode MakeNode(VirtualItem item)
        {
            TreeNode node = new TreeNode(item.Text);
            node.ImageIndex = item.ImageIndex;
            node.SelectedImageIndex = item.ImageIndex;
            node.Tag = item;
            node.Nodes.Add(" : ");
            return node;
        }

        #endregion
    }
}
