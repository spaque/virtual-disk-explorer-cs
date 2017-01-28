using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using FileExplorer.Shell;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Custom TreeView control used in the explorer control
    /// </summary>
    internal sealed class ExplorerTreeView : TreeView
    {
        #region Fields

        private TreeNode root;
        private ShellImageList imageList;
        private ExplorerTreeSorter sorter;

        #endregion

        #region Constructor

        public ExplorerTreeView()
            : base()
        {
            sorter = new ExplorerTreeSorter();
            imageList = ShellImageList.Instance;
            TreeViewNodeSorter = sorter;
            LoadRoot();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            HotTracking = true;
            HideSelection = false;
            ShowLines = false;
            ShowNodeToolTips = true;
            HandleCreated += new EventHandler(ExplorerTreeView_HandleCreated);
            HandleDestroyed += new EventHandler(ExplorerTreeView_HandleDestroyed);
            VisibleChanged += new EventHandler(ExplorerTreeView_VisibleChanged);
            BeforeExpand += new TreeViewCancelEventHandler(ExplorerTreeView_BeforeExpand);
            BeforeCollapse += new TreeViewCancelEventHandler(ExplorerTreeView_BeforeCollapse);
        }

        #endregion

        #region Events

        void ExplorerTreeView_HandleCreated(object sender, EventArgs e)
        {
            imageList.SetSmallImageList(this);
        }

        void ExplorerTreeView_HandleDestroyed(object sender, EventArgs e)
        {
            ((ShellItem)root.Tag).Dispose();
        }

        void ExplorerTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Cursor oldCursor = this.Cursor;
            ShellItem item = (ShellItem)e.Node.Tag;
            ShellItem shRoot = (ShellItem)root.Tag;
            Cursor = Cursors.WaitCursor;
            Enabled = false;
            if (!item.Equals(shRoot))
            {
                if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text.Equals(" : "))
                {
                    e.Node.Nodes.Clear();
                    List<ShellItem> subDirs = item.GetFolders(true);
                    TreeNode[] nodes = new TreeNode[subDirs.Count];
                    for ( int i = 0; i < subDirs.Count; i++)
                        nodes[i] = MakeNode(subDirs[i]);
                    e.Node.Nodes.AddRange(nodes);
                }
                else
                    RefreshNode(e.Node);
            }
            Enabled = true;
            Cursor = oldCursor;
        }

        void ExplorerTreeView_VisibleChanged(object sender, EventArgs e)
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

        void ExplorerTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Equals(root))
                e.Cancel = true;
        }

        #endregion

        #region Public Methods

        public void ExpandSubNode(String text)
        {
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
            ShellItem nodeItem = (ShellItem)node.Tag;
            if (!(node.Nodes.Count == 1 && node.Nodes[0].Text.Equals(" : ")))
            {
                if (nodeItem.RefreshFolders())
                {
                    List<ShellItem> curDirs = new List<ShellItem>();
                    curDirs.AddRange(nodeItem.GetFolders(false));
                    List<TreeNode> invalidNodes = new List<TreeNode>();
                    bool match;
                    foreach (TreeNode tn in node.Nodes)
                    {
                        match = false;
                        for (int i = 0; i < curDirs.Count; i++)
                        {
                            if (curDirs[i].Equals(tn.Tag))
                            {
                                curDirs.RemoveAt(i);
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                            invalidNodes.Add(tn);
                    }
                    if (invalidNodes.Count + curDirs.Count > 0)
                    {
                        try
                        {
                            BeginUpdate();
                            TreeViewNodeSorter = null;
                            TreeNode[] nodes = new TreeNode[curDirs.Count];
                            int i = 0;
                            foreach (TreeNode tn in invalidNodes)
                                node.Nodes.Remove(tn);
                            foreach (ShellItem sh in curDirs)
                            {
                                nodes[i] = MakeNode(sh);
                                i++;
                            }
                            node.Nodes.AddRange(nodes);
                            TreeViewNodeSorter = sorter;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                        finally
                        {
                            EndUpdate();
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void LoadRoot()
        {
            ShellItem desktop = new ShellItem();
            List<ShellItem> subDirs;
            root = new TreeNode(desktop.Text);
            root.ImageIndex = imageList.GetIconIndex(desktop, false);
            root.SelectedImageIndex = imageList.GetIconIndex(desktop, true);
            root.Tag = desktop;
            subDirs = desktop.GetFolders(true);
            foreach (ShellItem sh in subDirs) 
                root.Nodes.Add(MakeNode(sh));
            Nodes.Clear();
            Nodes.Add(root);
            root.Expand();
        }

        private TreeNode MakeNode(ShellItem item)
        {
            TreeNode tn = new TreeNode(item.Text);
            tn.Tag = item;
            tn.ImageIndex = imageList.GetIconIndex(item, false);
            tn.SelectedImageIndex = imageList.GetIconIndex(item, true);
            tn.ToolTipText = item.ToolTip;
            if (item.IsFolder && item.HasSubFolder)
                tn.Nodes.Add(" : ");
            return tn;
        }

        #endregion
    }
}
