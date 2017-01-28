using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

using FileExplorer.Shell;
using FileExplorer.Controls;

namespace FileExplorer
{
    public delegate void SelectedFolderChangedEventHandler(object sender, SelectedFolderChangedEventArgs e);

    public sealed partial class Explorer : UserControl
    {
        #region Fields

        private ShellImageList imageList;
        private TreeNode currentNode;
        private const int maxBackFordward = 10;

        #endregion

        public event SelectedFolderChangedEventHandler SelectedFolderChanged;

        #region Constructor

        public Explorer()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            imageList = ShellImageList.Instance;

            viewDetailsMenuItem.Checked = true;

            explorerTreeView1.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
            explorerTreeView1.BeforeCollapse += new TreeViewCancelEventHandler(treeView_BeforeCollapse);
            explorerTreeView1.DragOver += new DragEventHandler(explorerTreeView1_DragOver);
            explorerTreeView1.DragDrop += new DragEventHandler(explorerTreeView1_DragDrop);

            explorerListView1.MouseDoubleClick += new MouseEventHandler(listView_MouseDoubleClick);
            explorerListView1.KeyDown += new KeyEventHandler(listView_KeyDown);
            explorerListView1.ListViewLoaded += new ListViewLoadedEventHandler(listView_ListViewLoaded);
            explorerListView1.BeforeLabelEdit += new LabelEditEventHandler(explorerListView1_BeforeLabelEdit);
            explorerListView1.AfterLabelEdit += new LabelEditEventHandler(explorerListView1_AfterLabelEdit);
            explorerListView1.DragOver += new DragEventHandler(explorerListView1_DragOver);
            explorerListView1.ItemDrag += new ItemDragEventHandler(explorerListView1_ItemDrag);
            explorerListView1.DragDrop += new DragEventHandler(explorerListView1_DragDrop);

            navUpButton.Click += new EventHandler(navUpButton_Click);

            navBackButton.DropDownItemClicked += new ToolStripItemClickedEventHandler(navBackButton_DropDownItemClicked);
            navBackButton.ButtonClick += new EventHandler(navBackButton_ButtonClick);

            navFordwardButton.DropDownItemClicked += new ToolStripItemClickedEventHandler(navFordwardButton_DropDownItemClicked);
            navFordwardButton.ButtonClick += new EventHandler(navFordwardButton_ButtonClick);

            refreshButton.Click += new EventHandler(refreshButton_Click);

            foldersButton.Click += new EventHandler(foldersButton_Click);
        }

        #endregion

        #region Private Methods

        private void AddNavBackFordwardItem(ToolStripSplitButton button, TreeNode item)
        {
            Bitmap image = imageList.GetIcon(item.ImageIndex, true).ToBitmap();
            ToolStripItem navItem = new ToolStripMenuItem(item.Text, image);
            navItem.Name = item.Text;
            navItem.Tag = item;
            navItem.ImageScaling = ToolStripItemImageScaling.None;

            if (button.DropDownItems.Count == maxBackFordward)
                button.DropDownItems.RemoveAt(maxBackFordward - 1);
            if ((button.DropDownItems.Count == 0) ||
                (button.DropDownItems.Count > 0 &&
                 button.DropDownItems[0] != navItem))
            {
                button.DropDownItems.Insert(0, navItem);
                button.Enabled = true;
            }
        }

        private void ResetFordwardButton()
        {
            if (navFordwardButton.DropDownItems.Count > 0)
            {
                navFordwardButton.DropDownItems.Clear();
                navFordwardButton.Enabled = false;
            }
        }

        private void NavigateDown(String text)
        {
            ResetFordwardButton();
            AddNavBackFordwardItem(navBackButton, currentNode);
            explorerTreeView1.ExpandSubNode(text);
        }

        private void CopyToClipboard(bool cut)
        {
            String[] files = GetSelectedForClipboard(cut);
            if (files != null)
            {
                IDataObject data = new DataObject(DataFormats.FileDrop, files);
                MemoryStream memo = new MemoryStream(4);
                byte[] bytes = new byte[] { (byte)(cut ? 2 : 5), 0, 0, 0 };
                memo.Write(bytes, 0, bytes.Length);
                data.SetData("Preferred DropEffect", memo);
                Clipboard.SetDataObject(data);
            }
        }

        private void PasteFromClipboard()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(DataFormats.FileDrop))
                return;

            String[] files = (String[])data.GetData(DataFormats.FileDrop);
            MemoryStream stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
            int flag = stream.ReadByte();
            if (flag != 2 && flag != 5)
                return;
            bool cut = (flag == 2);
            ShellItem item = (ShellItem)currentNode.Tag;
            String dest = item.Path;
            if (item.IsDesktop)
                dest = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            for (int i = 0; i < files.Length; i++)
                FileOperation(files[i], dest, cut);
        }

        private void FileOperation(String src, String dest, bool cut)
        {
            if (String.Compare(src, dest, true) != 0)
            {
                ShellAPI.SHFILEOPSTRUCT fileOp = new ShellAPI.SHFILEOPSTRUCT();
                fileOp.hwnd = ParentForm.Handle;
                if (cut)
                    fileOp.wFunc = ShellAPI.FO_Func.MOVE;
                else
                    fileOp.wFunc = ShellAPI.FO_Func.COPY;
                fileOp.pFrom = Marshal.StringToHGlobalUni(src + '\0');
                fileOp.pTo = Marshal.StringToHGlobalUni(dest + '\0');
                ShellAPI.SHFileOperation(ref fileOp);
            }
        }

        private String[] GetSelectedForClipboard(bool cut)
        {
            List<String> selected = new List<String>();
            String path;
            foreach (ListViewItem lvi in explorerListView1.SelectedItems)
            {
                ShellItem item = (ShellItem)lvi.Tag;
                path = item.Path;
                if (item.IsDesktop)
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                else if (String.Compare(path, item.MyDocumentsName, true) == 0)
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                if (cut && item.CanMove)
                    selected.Add(path);
                else if (!cut && item.CanCopy)
                    selected.Add(path);
            }
            return selected.ToArray();
        }

        private String[] GetSelected()
        {
            String[] selected = new String[explorerListView1.SelectedItems.Count];
            int i = 0;
            foreach (ListViewItem lvi in explorerListView1.SelectedItems)
            {
                ShellItem item = (ShellItem)lvi.Tag;
                selected[i++] = item.Path;
            }
            return selected;
        }

        private void RefreshExplorer()
        {
            TreeNode node = explorerTreeView1.SelectedNode;
            explorerListView1.Wait();
            explorerTreeView1.RefreshNode(node);
            //explorerTreeView1.SelectedNode = node;
            OnSelectedFolderChanged(new SelectedFolderChangedEventArgs(node, node, true));
        }

        private String[] GetSelectedForDelete()
        {
            List<String> selected = new List<String>();
            foreach (ListViewItem lvi in explorerListView1.SelectedItems)
            {
                ShellItem item = (ShellItem)lvi.Tag;
                if (item.CanDelete)
                    selected.Add(item.Path);
            }
            return selected.ToArray();
        }

        private void OpenCmd(ShellItem item)
        {
            ShellAPI.SHELLEXECUTEINFO shEx = new ShellAPI.SHELLEXECUTEINFO();
            ShellAPI.SEE_MASK mask = ShellAPI.SEE_MASK.IDLIST;
            shEx.cbSize = Marshal.SizeOf(typeof(ShellAPI.SHELLEXECUTEINFO));
            shEx.fMask = mask;
            shEx.lpVerb = "open";
            shEx.lpFile = item.Path;
            shEx.lpDirectory = item.Parent.Path;
            shEx.lpIDList = item.FullPidl;
            shEx.nShow = ShellAPI.SW.NORMAL;
            ShellAPI.ShellExecuteEx(ref shEx);
        }

        private void DeleteSelected()
        {
            if (explorerListView1.SelectedItems.Count > 0)
            {
                String[] files = GetSelectedForDelete();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < files.Length; i++)
                {
                    sb.Append(files[i] + '\0');
                }
                sb.Append('\0');
                ShellAPI.SHFILEOPSTRUCT fileOp = new ShellAPI.SHFILEOPSTRUCT();
                fileOp.hwnd = ParentForm.Handle;
                fileOp.wFunc = ShellAPI.FO_Func.DELETE;
                fileOp.pFrom = Marshal.StringToHGlobalUni(sb.ToString());
                ShellAPI.SHFileOperation(ref fileOp);
                RefreshExplorer();
            }
        }

        #endregion

        #region Events

        #region Drag and Drop

        void explorerTreeView1_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            Point pt = explorerTreeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode node = explorerTreeView1.GetNodeAt(pt.X, pt.Y);
            if (node == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            else
            {
                ShellItem item = (ShellItem)node.Tag;
                if (!Regex.IsMatch(item.Path, @"^[a-z]:\\\w+", RegexOptions.IgnoreCase))
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }
            }
            if ((e.KeyState & ShellAPI.SHIFT) == ShellAPI.SHIFT &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & ShellAPI.CTRL) == ShellAPI.CTRL &&
                (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void explorerTreeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            Point pt = explorerTreeView1.PointToClient(new Point(e.X, e.Y));
            TreeNode node = explorerTreeView1.GetNodeAt(pt);
            if (node == null) return;

            bool cut = false;
            if (e.Effect == DragDropEffects.Move) cut = true;
            else if (e.Effect != DragDropEffects.Copy) return;
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            ShellItem item = (ShellItem)node.Tag;
            String dest = item.Path;
            for (int i = 0; i < files.Length; i++)
            {
                FileOperation(files[i], dest, cut);
            }
            RefreshExplorer();
        }

        void explorerListView1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            bool cut = false;
            if (e.Effect == DragDropEffects.Move) cut = true;
            else if (e.Effect != DragDropEffects.Copy) return;
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            ShellItem item = (ShellItem)currentNode.Tag;
            String dest = item.Path;
            if (item.IsDesktop)
                dest = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            for (int i = 0; i < files.Length; i++)
            {
                FileOperation(files[i], dest, cut);
            }
            RefreshExplorer();
        }

        void explorerListView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            String[] selected = GetSelectedForClipboard(true);
            if (selected.Length > 0)
            {
                DoDragDrop(new DataObject(DataFormats.FileDrop, selected),
                                DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        void explorerListView1_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            if ((e.KeyState & ShellAPI.SHIFT) == ShellAPI.SHIFT &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & ShellAPI.CTRL) == ShellAPI.CTRL &&
                (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        #endregion

        #region Buttons

        private void viewLargeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewDropDownButton.DropDownItems)
                    item.Checked = false;
                explorerListView1.View = View.LargeIcon;
                view.Checked = true;
            }
        }

        private void viewListMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewDropDownButton.DropDownItems)
                    item.Checked = false;
                explorerListView1.View = View.List;
                view.Checked = true;
            }
        }

        private void viewDetailsMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewDropDownButton.DropDownItems)
                    item.Checked = false;
                explorerListView1.View = View.Details;
                view.Checked = true;
            }
        }

        void foldersButton_Click(object sender, EventArgs e)
        {
            if (foldersButton.Checked)
            {
                splitContainer1.Panel1Collapsed = true;
                foldersButton.Checked = false;
            }
            else
            {
                splitContainer1.Panel1Collapsed = false;
                foldersButton.Checked = true;
            }
        }

        void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshExplorer();
        }

        void navFordwardButton_ButtonClick(object sender, EventArgs e)
        {
            ToolStripItem navItem = navFordwardButton.DropDownItems[0];
            navFordwardButton.DropDownItems.RemoveAt(0);
            if (navFordwardButton.DropDownItems.Count == 0)
                navFordwardButton.Enabled = false;
            AddNavBackFordwardItem(navBackButton, explorerTreeView1.SelectedNode);
            explorerTreeView1.SelectedNode = (TreeNode)navItem.Tag;
        }

        void navFordwardButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = navFordwardButton.DropDownItems.IndexOf(e.ClickedItem);
            ToolStripItem navItem;
            AddNavBackFordwardItem(navBackButton, explorerTreeView1.SelectedNode);
            while (index > -1)
            {
                navItem = navFordwardButton.DropDownItems[0];
                navFordwardButton.DropDownItems.RemoveAt(0);
                index--;
                if (index > -1)
                {
                    navBackButton.DropDownItems.Insert(0, navItem);
                    if (navBackButton.DropDownItems.Count == maxBackFordward)
                        navBackButton.DropDownItems.RemoveAt(maxBackFordward - 1);
                }
            }
            if (navFordwardButton.DropDownItems.Count == 0)
                navFordwardButton.Enabled = false;
            explorerTreeView1.SelectedNode = (TreeNode)e.ClickedItem.Tag;
        }

        void navBackButton_ButtonClick(object sender, EventArgs e)
        {
            ToolStripItem navItem = navBackButton.DropDownItems[0];
            navBackButton.DropDownItems.RemoveAt(0);
            if (navBackButton.DropDownItems.Count == 0)
                navBackButton.Enabled = false;
            AddNavBackFordwardItem(navFordwardButton, explorerTreeView1.SelectedNode);
            explorerTreeView1.SelectedNode = (TreeNode)navItem.Tag;
        }

        void navBackButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = navBackButton.DropDownItems.IndexOf(e.ClickedItem);
            ToolStripItem navItem;
            AddNavBackFordwardItem(navFordwardButton, explorerTreeView1.SelectedNode);
            while (index > -1)
            {
                navItem = navBackButton.DropDownItems[0];
                navBackButton.DropDownItems.RemoveAt(0);
                index--;
                if (index > -1)
                {
                    navFordwardButton.DropDownItems.Insert(0, navItem);
                    if (navFordwardButton.DropDownItems.Count == maxBackFordward)
                        navFordwardButton.DropDownItems.RemoveAt(maxBackFordward - 1);
                }
            }
            if (navBackButton.DropDownItems.Count == 0)
                navBackButton.Enabled = false;
            explorerTreeView1.SelectedNode = (TreeNode)e.ClickedItem.Tag;
        }

        void navUpButton_Click(object sender, EventArgs e)
        {
            ResetFordwardButton();
            AddNavBackFordwardItem(navBackButton, currentNode);
            explorerTreeView1.navigateUp();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            CopyToClipboard(false);
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            CopyToClipboard(true);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
            RefreshExplorer();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ShellItem item = (ShellItem)currentNode.Tag;
            ShellAPI.SHFindFiles(item.FullPidl, IntPtr.Zero);
        }

        #endregion

        #region ListView

        void explorerListView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ShellItem item = (ShellItem)explorerListView1.SelectedItems[0].Tag;
            String from = item.Path;
            if (Regex.IsMatch(from, @"^[a-z]:\\\w+", RegexOptions.IgnoreCase))
            {
                String to = item.Parent.Path + "\\" + e.Label;
                ShellAPI.SHFILEOPSTRUCT fileOp = new ShellAPI.SHFILEOPSTRUCT();
                fileOp.hwnd = ParentForm.Handle;
                fileOp.wFunc = ShellAPI.FO_Func.RENAME;
                fileOp.pFrom = Marshal.StringToHGlobalUni(from + '\0');
                fileOp.pTo = Marshal.StringToHGlobalUni(to + '\0');
                ShellAPI.SHFileOperation(ref fileOp);
            }
        }

        void explorerListView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (explorerListView1.SelectedItems.Count == 1)
            {
                ShellItem item = (ShellItem)explorerListView1.SelectedItems[0].Tag;
                if (!item.CanRename)
                    e.CancelEdit = true;
            }
            else
                e.CancelEdit = true;
        }

        void listView_ListViewLoaded(object sender, EventArgs e)
        {
            explorerTreeView1.Enabled = true;
            toolStrip1.Enabled = true;
        }

        void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && explorerListView1.SelectedItems.Count == 1)
            {
                explorerListView1.Wait();
                ListViewItem lvi = explorerListView1.SelectedItems[0];
                ShellItem item = (ShellItem)lvi.Tag;
                if (item.IsFolder)
                    NavigateDown(item.Text);
                else
                    OpenCmd(item);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                explorerListView1.Wait();
                ResetFordwardButton();
                if (explorerTreeView1.SelectedNode.Parent != null)
                    AddNavBackFordwardItem(navBackButton, currentNode);
                explorerTreeView1.navigateUp();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelected();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                if (explorerListView1.SelectedItems.Count == 1)
                {
                    explorerListView1.SelectedItems[0].BeginEdit();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshExplorer();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                CopyToClipboard(false);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.X && e.Control)
            {
                CopyToClipboard(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                PasteFromClipboard();
                RefreshExplorer();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                int count = explorerListView1.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    explorerListView1.Items[i].Selected = true;
                }
                e.Handled = true;
            }
        }

        void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button != MouseButtons.Left) ||
                (explorerListView1.SelectedItems == null) ||
                (explorerListView1.SelectedItems.Count < 1))
                return;
            ListViewItem lvi = explorerListView1.GetItemAt(e.X, e.Y);
            if (lvi == null)
                return;
            ShellItem item = (ShellItem)lvi.Tag;
            if (item.IsFolder)
            {
                explorerListView1.Wait();
                NavigateDown(item.Text);
            }
            else
            {
                //if (item.Path.Contains(":\\"))
                    OpenCmd(item);
                //else
                    //MessageBox.Show(String.Format("No se puede abrir {0}", item.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region TreeView

        void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // if the selected node was situated below the one to 
            // collapse, select the latter
            ShellItem selected = explorerTreeView1.SelectedNode.Tag as ShellItem;
            ShellItem collapsed = e.Node.Tag as ShellItem;
            if (collapsed.IsAncestorOf(selected,false) && (
                selected != collapsed))
            {
                ResetFordwardButton();
                AddNavBackFordwardItem(navBackButton, explorerTreeView1.SelectedNode);
                explorerTreeView1.SelectedNode = e.Node;
            }
        }

        void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e != null)
            {
                OnSelectedFolderChanged(new SelectedFolderChangedEventArgs(e.Node, currentNode, false));
                if (e.Action != TreeViewAction.Unknown)
                {
                    // if there's any history in the fordward button delete it
                    ResetFordwardButton();
                    // if it's not the first node expansion
                    // add it to the navigation history
                    if (currentNode != null)
                        AddNavBackFordwardItem(navBackButton, currentNode);
                    // expand the node only when it's activated by the mouse
                    if (e.Action == TreeViewAction.ByMouse)
                        explorerTreeView1.SelectedNode.Expand();
                }
                currentNode = e.Node;
            }
        }

        #endregion

        #endregion

        #region Generated Events

        private void OnSelectedFolderChanged(SelectedFolderChangedEventArgs e)
        {
            if (SelectedFolderChanged != null)
            {
                explorerTreeView1.Enabled = false;
                toolStrip1.Enabled = false;
                explorerListView1.RefreshListView((ShellItem)e.NewNode.Tag);

                if (!e.Refresh)
                {
                    // raise the SelectedFolderChanged event
                    SelectedFolderChanged(this, e);

                    ShellItem item = (ShellItem)e.NewNode.Tag;
                    // check for root folder to deactivate the up button
                    if (!item.IsDesktop)
                        navUpButton.Enabled = true;
                    else
                        navUpButton.Enabled = false;

                    Icon icon = imageList.GetIcon(item.ImageIndex, true);
                    imageLabel.Image = icon.ToBitmap();
                    // set the new address to the addressTextBox
                    addressTextBox.Text = item.Path;
                }
            }
        }

        #endregion
    }
}
