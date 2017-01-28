using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

using FileExplorer.Shell;
using VirtualDrive.Controls;
using VirtualDrive.Shell;
using VirtualDrive.FileSystem.FAT32;

namespace VirtualDrive
{
    public delegate void SelectedFolderChangedEventHandler(object sender, SelectedFolderChangedEventArgs e);
    public delegate void OpProgressChangeEventHandler(object sender, OpProgressChangedEventArgs e);

    public sealed partial class VirtualExplorer : UserControl
    {
        #region Fields

        private Disk disk;

        private ShellImageList imageList;
        private TreeNode currentNode;
        private const int maxBackFordward = 10;

        private CommandLine cmd;
        private DiskPerformance perfChart;

        private String tempPath;
        private DragDropEffects effect;
        private bool lvDragCanceled;
        private bool lvDragItem;
        private bool lvDragLeave;

        private bool openCmd;

        private BackgroundWorker copyWorker;
        private BackgroundWorker moveWorker;
        private BackgroundWorker extractWorker;
        private BackgroundWorker formatWorker;
        private ManualResetEvent extractMre;

        private DataObject dataObject;

        #endregion

        public event SelectedFolderChangedEventHandler SelectedFolderChanged;
        public event OpProgressChangeEventHandler ProgressChange;

        #region Constructor

        public VirtualExplorer()
        {
            InitializeComponent();
            SetTempPath();

            imageList = ShellImageList.Instance;
            viewDetailsMenuItem.Checked = true;
            buttonsToolStrip.Enabled = false;

            copyWorker = new BackgroundWorker();
            copyWorker.DoWork += new DoWorkEventHandler(copyWorker_DoWork);
            copyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            moveWorker = new BackgroundWorker();
            moveWorker.DoWork += new DoWorkEventHandler(moveWorker_DoWork);
            moveWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            extractWorker = new BackgroundWorker();
            extractWorker.WorkerSupportsCancellation = true;
            extractWorker.DoWork += new DoWorkEventHandler(extractWorker_DoWork);
            extractWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(extractWorker_RunWorkerCompleted);

            formatWorker = new BackgroundWorker();
            formatWorker.DoWork += new DoWorkEventHandler(formatWorker_DoWork);
            formatWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(formatWorker_RunWorkerCompleted);

            extractMre = new ManualResetEvent(true);

            HandleDestroyed += new EventHandler(VirtualExplorer_HandleDestroyed);

            virtualTreeView1.AfterSelect += new TreeViewEventHandler(virtualTreeView_AfterSelect);
            virtualTreeView1.BeforeCollapse += new TreeViewCancelEventHandler(virtualTreeView_BeforeCollapse);
            virtualTreeView1.DragDrop += new DragEventHandler(virtualTreeView1_DragDrop);
            virtualTreeView1.DragOver += new DragEventHandler(virtualTreeView1_DragOver);
            virtualTreeView1.MouseDown += new MouseEventHandler(virtualTreeView1_MouseDown);

            virtualListView1.MouseDoubleClick += new MouseEventHandler(virtualListView_MouseDoubleClick);
            virtualListView1.KeyDown += new KeyEventHandler(virtualListView_KeyDown);
            virtualListView1.ListViewLoaded += new ListViewLoadedEventHandler(virtualListView_ListViewLoaded);
            virtualListView1.ItemDrag += new ItemDragEventHandler(virtualListView1_ItemDrag);
            virtualListView1.DragDrop += new DragEventHandler(virtualListView1_DragDrop);
            virtualListView1.DragLeave += new EventHandler(virtualListView1_DragLeave);
            virtualListView1.QueryContinueDrag += new QueryContinueDragEventHandler(virtualListView1_QueryContinueDrag);
            virtualListView1.DragOver += new DragEventHandler(virtualListView1_DragOver);
            virtualListView1.MouseDown += new MouseEventHandler(virtualListView1_MouseDown);
            virtualListView1.AfterLabelEdit += new LabelEditEventHandler(virtualListView1_AfterLabelEdit);

            navUpButton.Click += new EventHandler(navUpButton_Click);
            navUpButton.Enabled = false;

            navBackButton.DropDownItemClicked += new ToolStripItemClickedEventHandler(navBackButton_DropDownItemClicked);
            navBackButton.ButtonClick += new EventHandler(navBackButton_ButtonClick);
            navBackButton.Enabled = false;

            navFordwardButton.DropDownItemClicked += new ToolStripItemClickedEventHandler(navFordwardButton_DropDownItemClicked);
            navFordwardButton.ButtonClick += new EventHandler(navFordwardButton_ButtonClick);
            navFordwardButton.Enabled = false;

            refreshButton.Click += new EventHandler(refreshButton_Click);

            foldersButton.Click += new EventHandler(foldersButton_Click);

            viewDetailsMenuItem.Click += new EventHandler(viewDetailsMenuItem_Click);
            viewListMenuItem.Click += new EventHandler(viewListMenuItem_Click);
            viewLargeMenuItem.Click += new EventHandler(viewLargeMenuItem_Click);
        }

        #endregion

        #region Properties

        public bool DiskMounted { get { return disk != null && disk.Mounted; } }

        public bool DiskFormatted { get { return disk != null && disk.Formatted; } }

        public float DiskTransferRate
        {
            get
            {
                if (disk != null && disk.Formatted)
                    return disk.GetRate();
                else
                    return 0;
            }
        }

        public bool DiskBusy { get { return disk != null && disk.Busy; } }

        #endregion

        #region Events

        void VirtualExplorer_HandleDestroyed(object sender, EventArgs e)
        {
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Context Menu

        void virtualListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy && e.Button == MouseButtons.Right)
            {
                ListViewItem lvi = virtualListView1.GetItemAt(e.X, e.Y);
                if (lvi != null)
                    itemContextMenu.Show(virtualListView1, e.X, e.Y);
                else
                    lvContextMenuStrip.Show(virtualListView1, e.X, e.Y);
            }
        }

        void virtualTreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy && e.Button == MouseButtons.Right)
            {
                TreeNode node = virtualTreeView1.GetNodeAt(e.X, e.Y);
                if (node != null)
                {
                    VirtualItem item = (VirtualItem)node.Tag;
                    if (item.IsRoot)
                        diskContextMenu.Show(virtualTreeView1, e.X, e.Y);
                }
            }
        }

        private void formatearToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FormatDisk();
        }

        private void propiedadesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            new DiskProperties(disk).ShowDialog();
        }

        private void actualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshExplorer();
        }

        private void iconosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.LargeIcon;
                view.Checked = true;
                viewLargeMenuItem.Checked = true;
            }
        }

        private void listaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.List;
                view.Checked = true;
                viewListMenuItem.Checked = true;
            }
        }

        private void detallesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.Details;
                view.Checked = true;
                viewDetailsMenuItem.Checked = true;
            }
        }

        private void nuevaCarpetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<String> itemList = GetLVItemsNames();
            String name = "Nueva Carpeta";
            int i = 1;
            while (itemList.Contains(name))
            {
                name = "Nueva Carpeta (" + i.ToString() + ")";
                i++;
            }
            VirtualItem item = (VirtualItem)currentNode.Tag;
            String path = item.Path;
            if (!path.EndsWith("\\")) path += "\\";
            try
            {
                disk.CreateDir(path + name);
                RefreshExplorer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteSelected();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cambiarNombreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi;
            if (virtualListView1.SelectedItems.Count > 0)
            {
                lvi = virtualListView1.SelectedItems[0];
                lvi.BeginEdit();
            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi;
                if (virtualListView1.SelectedItems.Count > 0)
                {
                    lvi = virtualListView1.SelectedItems[0];
                    VirtualItem item = (VirtualItem)lvi.Tag;
                    if (item.IsFolder)
                        NavigateDown(item.Text);
                    else
                    {
                        OpenSelectedItem();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void propiedadesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem lvi;
            if (virtualListView1.SelectedItems.Count > 0)
            {
                lvi = virtualListView1.SelectedItems[0];
                VirtualItem item = (VirtualItem)lvi.Tag;
                new EntryProperties(item, disk).ShowDialog();
            }
        }

        private void propiedadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VirtualItem item = (VirtualItem)currentNode.Tag;
            if (item.IsRoot)
                new DiskProperties(disk).ShowDialog();
            else
                new EntryProperties(item, disk).ShowDialog();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard(false);
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard(true);
        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        #endregion

        #region BackgroundWorker

        void formatWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableControl();
            if (cmd == null || cmd.IsDisposed)
            {
                cmd = new CommandLine(disk);
                cmd.NavCommandEntered += new NavCommandEnteredEventHandler(cmd_NavCommandEntered);
            }
            if (perfChart == null || perfChart.IsDisposed)
            {
                perfChart = new DiskPerformance(disk);
            }
            virtualTreeView1.LoadRoot(new VirtualItem(disk));
            ResetSplitButton(navFordwardButton);
            ResetSplitButton(navBackButton);
            perfChart.UpdateValues();
        }

        void formatWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DisableControl();
            disk.Format();
        }

        void extractWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (openCmd)
            {
                String path = (String)e.Result;
                ShellAPI.SHELLEXECUTEINFO shEx = new ShellAPI.SHELLEXECUTEINFO();
                shEx.cbSize = Marshal.SizeOf(typeof(ShellAPI.SHELLEXECUTEINFO));
                shEx.lpVerb = "open";
                shEx.lpFile = path;
                shEx.lpDirectory = Path.GetDirectoryName(path);
                shEx.nShow = ShellAPI.SW.NORMAL;
                ShellAPI.ShellExecuteEx(ref shEx);
            }
            openCmd = false;
        }

        void extractWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            String[] files = (String[])e.Argument;
            String[] selected = GetSelectedItems();
            extractMre.Reset();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (File.Exists(files[i]))
                        ExtractTempFile(files[i], selected[i]);
                    else if (Directory.Exists(files[i]))
                    {
                        ExtractTempDirectory(files[i]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            e.Result = files[0];
            extractMre.Set();
        }

        void moveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            List<String> args = (List<String>)e.Argument;
            String dest = args[args.Count - 1];
            List<String> folderList = FolderList();
            List<String> archiveList = ArchiveList();
            String name;
            bool check = false;

            if (String.Compare(Path.GetDirectoryName(args[0]), dest, true) == 0)
            {
                e.Cancel = true;
                return;
            }
            VirtualItem item = (VirtualItem)currentNode.Tag;
            String path = item.Path;
            if (String.Compare(path, dest, true) == 0) check = true;
            if (!path.EndsWith("\\")) path += "\\";
            for (int i = 0; i < args.Count - 1; i++)
            {
                try
                {
                    name = Path.GetFileName(args[i]);
                    if (check && folderList.Contains(name))
                    {
                        if (MessageBox.Show(
                             String.Format("Esta carpeta ya contiene una carpeta con el nombre \"{0}\"", name),
                             "Confirmar reemplazo de carpeta",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.No)
                            continue;
                        else
                            disk.DeleteDir(path + name, true);
                    }
                    else if (check && archiveList.Contains(name))
                    {
                        if (MessageBox.Show(
                             String.Format("Esta carpeta ya contiene un archivo con el nombre \"{0}\"", name),
                             "Confirmar reemplazo de archivo",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.No)
                            continue;
                        else
                            disk.Delete(path + name);
                    }
                    MoveItem(args[i], dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            disk.Busy = false;
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                RefreshExplorer();
            }
            disk.Busy = false;
        }

        void copyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            List<String> args = (List<String>)e.Argument;
            String dest = args[args.Count - 1];
            List<String> folderList = FolderList();
            List<String> archiveList = ArchiveList();
            String name;
            bool check = false;
            if (String.Compare(Path.GetDirectoryName(args[0]), dest, true) == 0)
            {
                e.Cancel = true;
                return;
            }
            VirtualItem item = (VirtualItem)currentNode.Tag;
            String path = item.Path;
            if (String.Compare(path, dest, true) == 0) check = true;
            if (!path.EndsWith("\\")) path += "\\";
            for (int i = 0; i < args.Count - 1; i++)
            {
                try
                {
                    name = Path.GetFileName(args[i]);
                    if (check && folderList.Contains(name))
                    {
                        if (MessageBox.Show(
                             String.Format("Esta carpeta ya contiene una carpeta con el nombre \"{0}\"", name),
                             "Confirmar reemplazo de carpeta",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.No)
                            continue;
                        else
                            disk.DeleteDir(path + name, true);
                    }
                    else if (check && archiveList.Contains(name))
                    {
                        if (MessageBox.Show(
                             String.Format("Esta carpeta ya contiene un archivo con el nombre \"{0}\"", name),
                             "Confirmar reemplazo de archivo",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.No)
                            continue;
                        else
                            disk.Delete(path + name);
                    }
                    Copy(args[i], dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }
            disk.Busy = false;
        }

        #endregion

        #region Command Line

        void cmd_NavCommandEntered(object sender, CommandEventArgs e)
        {
            if (e.CmdType == CMDTYPE.NAV_DOWN)
                NavigateDown(e.Text);
            else if (e.CmdType == CMDTYPE.NAV_UP)
                virtualTreeView1.navigateUp();
            else if (e.CmdType == CMDTYPE.NAV_MOD)
                RefreshExplorer();
        }

        #endregion

        #region Drag and Drop

        void virtualTreeView1_DragOver(object sender, DragEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy)
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.None;
                    effect = DragDropEffects.None;
                    return;
                }
                Point pt = virtualTreeView1.PointToClient(new Point(e.X, e.Y));
                TreeNode node = virtualTreeView1.GetNodeAt(pt.X, pt.Y);
                if (node == null)
                {
                    e.Effect = DragDropEffects.None;
                    effect = DragDropEffects.None;
                    return;
                }
                if ((e.KeyState & ShellAPI.SHIFT) == ShellAPI.SHIFT &&
                    (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    e.Effect = DragDropEffects.Move;
                    effect = DragDropEffects.Move;
                }
                else if ((e.KeyState & ShellAPI.CTRL) == ShellAPI.CTRL &&
                    (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    e.Effect = DragDropEffects.Copy;
                    effect = DragDropEffects.Copy;
                }
                else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    e.Effect = DragDropEffects.Copy;
                    effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
                effect = DragDropEffects.None;
            }
        }

        void virtualTreeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy)
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    return;
                }
                try
                {
                    if (extractWorker.IsBusy)
                        extractWorker.CancelAsync();
                    Point pt = virtualTreeView1.PointToClient(new Point(e.X, e.Y));
                    TreeNode node = virtualTreeView1.GetNodeAt(pt.X, pt.Y);

                    if (node == null)
                    {
                        lvDragItem = false;
                        lvDragLeave = false;
                        return;
                    }
                    String dest = ((VirtualItem)node.Tag).Path;

                    String[] files;
                    if (lvDragItem)
                        files = GetSelectedItems();
                    else
                        files = (String[])e.Data.GetData(DataFormats.FileDrop);

                    String src = Path.GetDirectoryName(files[0]);
                    if (String.Compare(src, dest, true) == 0)
                    {
                        lvDragItem = false;
                        lvDragLeave = false;
                        return;
                    }

                    disk.Busy = true;
                    List<String> args = new List<String>(files);
                    args.Add(dest);
                    if (e.Effect == DragDropEffects.Copy)
                    {
                        copyWorker.RunWorkerAsync(args);
                    }
                    else if (e.Effect == DragDropEffects.Move)
                    {
                        moveWorker.RunWorkerAsync(args);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    disk.Busy = false;
                }
                finally
                {
                    lvDragItem = false;
                    lvDragLeave = false;
                }
            }
        }

        void virtualListView1_DragDrop(object sender, DragEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy)
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    return;
                }
                try
                {
                    if (extractWorker.IsBusy)
                        extractWorker.CancelAsync();
                    Point pt = virtualListView1.PointToClient(new Point(e.X, e.Y));
                    ListViewItem lvi = virtualListView1.GetItemAt(pt.X, pt.Y);
                    String dest = ((VirtualItem)currentNode.Tag).Path;

                    if (lvi != null)
                    {
                        VirtualItem item = (VirtualItem)lvi.Tag;
                        if (item.IsFolder)
                            dest = item.Path;
                    }

                    String[] files;
                    if (lvDragItem)
                        files = GetSelectedItems();
                    else
                        files = (String[])e.Data.GetData(DataFormats.FileDrop);

                    disk.Busy = true;
                    List<String> args = new List<String>(files);
                    args.Add(dest);
                    if (e.Effect == DragDropEffects.Copy)
                    {
                        copyWorker.RunWorkerAsync(args);
                    }
                    else if (e.Effect == DragDropEffects.Move)
                    {
                        moveWorker.RunWorkerAsync(args);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    disk.Busy = false;
                }
                finally
                {
                    lvDragItem = false;
                    lvDragLeave = false;
                }
            }
        }

        void virtualListView1_DragLeave(object sender, EventArgs e)
        {
            if (!lvDragCanceled)
            {
                lvDragLeave = true;
            }
        }

        void virtualListView1_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                lvDragItem = false;
                lvDragLeave = false;
                lvDragCanceled = true;
                if (extractWorker.IsBusy)
                    extractWorker.CancelAsync();
                return;
            }
            if (lvDragLeave)
            {
                if ((e.KeyState & ShellAPI.SHIFT) == ShellAPI.SHIFT)
                {
                    effect = DragDropEffects.Move;
                }
                else if ((e.KeyState & ShellAPI.CTRL) == ShellAPI.CTRL)
                {
                    effect = DragDropEffects.Copy;
                }
                else if (e.KeyState == 0)
                {
                    dataObject.SetData(ShellAPI.CFSTR_INDRAGLOOP, 0);
                }
            }
        }

        void virtualListView1_DragOver(object sender, DragEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy)
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.None;
                    effect = DragDropEffects.None;
                    return;
                }
                if ((e.KeyState & ShellAPI.SHIFT) == ShellAPI.SHIFT &&
                    (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    e.Effect = DragDropEffects.Move;
                    effect = DragDropEffects.Move;
                }
                else if ((e.KeyState & ShellAPI.CTRL) == ShellAPI.CTRL &&
                    (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    e.Effect = DragDropEffects.Copy;
                    effect = DragDropEffects.Copy;
                }
                else if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    e.Effect = DragDropEffects.Copy;
                    effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
                effect = DragDropEffects.None;
            }
        }

        void virtualListView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (disk != null && disk.Formatted && !disk.Busy)
            {
                String[] selection = GetSelection();
                if (selection.Length > 0)
                {
                    lvDragCanceled = false;
                    lvDragLeave = false;
                    lvDragItem = true;
                    dataObject = new ShellDataObject(extractMre, false);
                    dataObject.SetData(DataFormats.FileDrop, selection);
                    dataObject.SetData(ShellAPI.CFSTR_INDRAGLOOP, 1);
                    extractWorker.RunWorkerAsync(selection);
                    virtualListView1.DoDragDrop(
                                dataObject,
                                DragDropEffects.Copy | DragDropEffects.Move);
                    dataObject = null;
                    if (lvDragLeave && effect == DragDropEffects.Move)
                    {
                        DeleteSelected();
                        lvDragLeave = false;
                    }
                }
            }
        }

        #endregion

        #region Buttons

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
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteSelected();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            ListViewItem lvi;
            if (virtualListView1.SelectedItems.Count > 0)
            {
                lvi = virtualListView1.SelectedItems[0];
                VirtualItem item = (VirtualItem)lvi.Tag;
                new EntryProperties(item, disk).ShowDialog();
            }
        }

        void viewLargeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.LargeIcon;
                view.Checked = true;
                iconosToolStripMenuItem.Checked = true;
            }
        }

        void viewListMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.List;
                view.Checked = true;
                listaToolStripMenuItem.Checked = true;
            }
        }

        void viewDetailsMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem view = sender as ToolStripMenuItem;
            if (!view.Checked)
            {
                foreach (ToolStripMenuItem item in viewsButton.DropDownItems)
                    item.Checked = false;
                foreach (ToolStripMenuItem item in verToolStripMenuItem.DropDownItems)
                    item.Checked = false;
                virtualListView1.View = View.Details;
                view.Checked = true;
                detallesToolStripMenuItem.Checked = true;
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
            AddNavBackFordwardItem(navBackButton, virtualTreeView1.SelectedNode);
            virtualTreeView1.SelectedNode = (TreeNode)navItem.Tag;
        }

        void navFordwardButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = navFordwardButton.DropDownItems.IndexOf(e.ClickedItem);
            ToolStripItem navItem;
            AddNavBackFordwardItem(navBackButton, virtualTreeView1.SelectedNode);
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
            virtualTreeView1.SelectedNode = (TreeNode)e.ClickedItem.Tag;
        }

        void navBackButton_ButtonClick(object sender, EventArgs e)
        {
            ToolStripItem navItem = navBackButton.DropDownItems[0];
            navBackButton.DropDownItems.RemoveAt(0);
            if (navBackButton.DropDownItems.Count == 0)
                navBackButton.Enabled = false;
            AddNavBackFordwardItem(navFordwardButton, virtualTreeView1.SelectedNode);
            virtualTreeView1.SelectedNode = (TreeNode)navItem.Tag;
        }

        void navBackButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = navBackButton.DropDownItems.IndexOf(e.ClickedItem);
            ToolStripItem navItem;
            AddNavBackFordwardItem(navFordwardButton, virtualTreeView1.SelectedNode);
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
            virtualTreeView1.SelectedNode = (TreeNode)e.ClickedItem.Tag;
        }

        void navUpButton_Click(object sender, EventArgs e)
        {
            ResetSplitButton(navFordwardButton);
            AddNavBackFordwardItem(navBackButton, currentNode);
            virtualTreeView1.navigateUp();
        }

        #endregion

        #region ListView

        void virtualListView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListViewItem lvi = virtualListView1.Items[e.Item];
            VirtualItem parent = (VirtualItem)currentNode.Tag;
            VirtualItem item = (VirtualItem)lvi.Tag;
            String path = parent.Path;
            if (!path.EndsWith("\\")) path += "\\";
            if (!disk.Rename(path + lvi.Text, e.Label))
                e.CancelEdit = true;
            else if (item.IsFolder)
                virtualTreeView1.RefreshNode(currentNode);
            if (!e.CancelEdit)
            {
                item.Text = e.Label;
                item.Path = path + e.Label;
                lvi.Tag = item;
            }
        }

        void virtualListView_ListViewLoaded(object sender, EventArgs e)
        {
            virtualTreeView1.Enabled = true;
            buttonsToolStrip.Enabled = true;
        }

        void virtualListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && virtualListView1.SelectedItems.Count == 1)
            {
                virtualListView1.Wait();
                ListViewItem lvi = virtualListView1.SelectedItems[0];
                VirtualItem item = (VirtualItem)lvi.Tag;
                if (item.IsFolder)
                    NavigateDown(item.Text);
                else
                    OpenSelectedItem();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                virtualListView1.Wait();
                ResetSplitButton(navFordwardButton);
                if (virtualTreeView1.SelectedNode.Parent != null)
                    AddNavBackFordwardItem(navBackButton, currentNode);
                virtualTreeView1.navigateUp();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelected();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2 && virtualListView1.SelectedItems.Count == 1)
            {
                ListViewItem lvi = virtualListView1.SelectedItems[0];
                lvi.BeginEdit();
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
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                int count = virtualListView1.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    virtualListView1.Items[i].Selected = true;
                }
                e.Handled = true;
            }
        }

        void virtualListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button != MouseButtons.Left) ||
                (virtualListView1.SelectedItems == null) ||
                (virtualListView1.SelectedItems.Count < 1))
                return;
            ListViewItem lvi = virtualListView1.GetItemAt(e.X, e.Y);
            if (lvi == null)
                return;
            VirtualItem item = (VirtualItem)lvi.Tag;
            if (item.IsFolder)
            {
                virtualListView1.Wait();
                NavigateDown(item.Text);
            }
            else
                OpenSelectedItem();
        }

        #endregion

        #region TreeView

        void virtualTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // if the selected node was situated below the one to 
            // collapse, select the latter
            VirtualItem selected = (VirtualItem)virtualTreeView1.SelectedNode.Tag;
            VirtualItem collapsed = (VirtualItem)e.Node.Tag;
            if (collapsed.IsAncestorOf(selected, false) && (
                selected != collapsed))
            {
                ResetSplitButton(navFordwardButton);
                AddNavBackFordwardItem(navBackButton, virtualTreeView1.SelectedNode);
                virtualTreeView1.SelectedNode = e.Node;
            }
        }

        void virtualTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e != null)
            {
                OnSelectedFolderChanged(new SelectedFolderChangedEventArgs(e.Node, currentNode, false));
                if (e.Action != TreeViewAction.Unknown)
                {
                    // if there's any history in the fordward button delete it
                    ResetSplitButton(navFordwardButton);
                    // if it's not the first node expansion
                    // add it to the navigation history
                    if (currentNode != null)
                        AddNavBackFordwardItem(navBackButton, currentNode);
                    // expand the node only when it's activated by the mouse
                    if (e.Action == TreeViewAction.ByMouse)
                        virtualTreeView1.SelectedNode.Expand();
                }
                currentNode = e.Node;
            }
        }

        #endregion

        #endregion

        #region Generated Events

        private void OnProgressChange(OpProgressChangedEventArgs e)
        {
            OpProgressChangeEventHandler handler = ProgressChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnSelectedFolderChanged(SelectedFolderChangedEventArgs e)
        {
            if (SelectedFolderChanged != null)
            {
                VirtualItem item = (VirtualItem)e.NewNode.Tag;

                virtualTreeView1.Enabled = false;
                buttonsToolStrip.Enabled = false;
                try
                {
                    virtualListView1.RefreshListView(item);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!e.Refresh)
                {
                    disk.CurrentDirectory = item;
                    if (!cmd.IsDisposed)
                        cmd.CurrentItem = item;

                    // raise the SelectedFolderChanged event
                    SelectedFolderChanged(this, e);

                    // check for root folder to deactivate the up button
                    if (!item.IsRoot)
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

        #region Private Methods

        private void OpenSelectedItem()
        {
            if (!disk.Busy)
            {
                String[] selected = GetSelection();
                openCmd = true;
                extractWorker.RunWorkerAsync(selected);
            }
        }

        private void CopyToClipboard(bool cut)
        {
            String[] selection = GetSelection();
            String[] items = GetSelectedItems();
            if (disk != null && !disk.Busy && selection.Length > 0)
            {
                dataObject = new ShellDataObject(extractMre, true);
                dataObject.SetData(DataFormats.FileDrop, selection);
                dataObject.SetData("Virtual Files", items);
                extractWorker.RunWorkerAsync(selection);
                MemoryStream memo = new MemoryStream(4);
                byte[] bytes = new byte[] { (byte)(cut ? 2 : 5), 0, 0, 0 };
                memo.Write(bytes, 0, bytes.Length);
                dataObject.SetData("Preferred DropEffect", memo);
                Clipboard.SetDataObject(dataObject);
            }
        }

        private void PasteFromClipboard()
        {
            IDataObject data = Clipboard.GetDataObject();
            if (!data.GetDataPresent(DataFormats.FileDrop))
                return;
            try
            {
                if (disk != null && disk.Busy) return;
                String[] files = (String[])data.GetData(DataFormats.FileDrop);
                MemoryStream stream = (MemoryStream)data.GetData("Preferred DropEffect", true);
                int flag = stream.ReadByte();
                if (flag != 2 && flag != 5)
                    return;
                bool cut = (flag == 2);
                VirtualItem dest = (VirtualItem)currentNode.Tag;
                List<String> args = new List<String>(files);
                args.Add(dest.Path);
                disk.Busy = true;
                if (cut)
                {
                    moveWorker.RunWorkerAsync(args);
                    DeletePasted(args);
                }
                else
                    copyWorker.RunWorkerAsync(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                disk.Busy = false;
            }
        }

        private void DeletePasted(List<String> pasted)
        {
            if (pasted[0].StartsWith(tempPath))
            {
                String[] files = (String[])dataObject.GetData("Virtual Files");
                for (int i = 0; i < files.Length; i++)
                {
                    if (File.Exists(pasted[i]))
                        disk.Delete(files[i]);
                    else
                        disk.DeleteDir(files[i], true);
                }
            }
        }

        private void DeleteSelected()
        {
            if (disk != null && !disk.Busy)
            {
                foreach (ListViewItem lvi in virtualListView1.SelectedItems)
                {
                    VirtualItem item = (VirtualItem)lvi.Tag;
                    if (item.IsFolder)
                        disk.DeleteDir(item.Path, true);
                    else
                        disk.Delete(item.Path);
                }
                RefreshExplorer();
            }
        }

        private void ExtractTempFile(String path, String virtualPath)
        {
            using (FileStream fstream = 
                    new FileStream(path, 
                                   FileMode.Open, 
                                   FileAccess.Write, 
                                   FileShare.None, 
                                   8192, 
                                   FileOptions.SequentialScan))
            {
                int fd = disk.Open(virtualPath, OPENMODE.READ);
                byte[] buffer = new byte[16384];
                int bytes = disk.Read(fd, buffer, 0, 16384);
                perfChart.UpdateValues();
                while (bytes > 0)
                {
                    fstream.Write(buffer, 0, bytes);
                    bytes = disk.Read(fd, buffer, 0, 16384);
                    perfChart.UpdateValues();
                }
                disk.Close(fd);
            }
        }

        private void ExtractTempDirectory(String path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileSystemInfo[] infos = info.GetFileSystemInfos();
            String relativePath = path.Substring(tempPath.Length + 1);
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Attributes == FileAttributes.Directory)
                    ExtractTempDirectory(infos[i].FullName);
                else
                    ExtractTempFile(infos[i].FullName, "V:\\" + relativePath + "\\" + infos[i].Name);
            }
        }

        private void CleanTempDirectory()
        {
            DirectoryInfo info = new DirectoryInfo(tempPath);
            FileInfo[] files = info.GetFiles();
            DirectoryInfo[] dirs = info.GetDirectories();
            for (int i = 0; i < files.Length; i++)
                files[i].Delete();
            for (int i = 0; i < dirs.Length; i++)
                dirs[i].Delete(true);
        }

        private void SetTempPath()
        {
            tempPath = Application.StartupPath + "\\Temp";
            Directory.CreateDirectory(tempPath);
        }

        private void MoveItem(String src, String dest)
        {
            Regex regex = new Regex(@"^[a-u|w-z]:\\\w+", RegexOptions.IgnoreCase);
            if (regex.IsMatch(src))
            {
                if (File.Exists(src))
                {
                    CopyFile(src, dest);
                    File.Delete(src);
                }
                else
                {
                    CopyDirectory(src, dest);
                    Directory.Delete(src, true);
                }
            }
            else
            {
                String destination = dest;
                if (!destination.EndsWith("\\")) destination += "\\";
                disk.Move(src, destination + Path.GetFileName(src), true);
            }
        }

        private void Copy(String src, String dest)
        {
            Regex regex = new Regex(@"^[a-u|w-z]:\\\w+", RegexOptions.IgnoreCase);
            if (regex.IsMatch(src))
            {
                if (File.Exists(src))
                    CopyFile(src, dest);
                else
                {
                    CopyDirectory(src, dest);
                }
            }
            else
            {
                String destination = dest;
                if (!destination.EndsWith("\\")) destination += "\\";
                disk.Copy(src, destination + Path.GetFileName(src), true);
                perfChart.UpdateValues();
            }
        }

        private List<String> FolderList()
        {
            List<String> list = new List<String>();
            VirtualItem item;
            foreach (ListViewItem lvi in virtualListView1.Items)
            {
                item = (VirtualItem)lvi.Tag;
                if (item.IsFolder)
                    list.Add(item.Text);
            }
            return list;
        }

        private List<String> ArchiveList()
        {
            List<String> list = new List<String>();
            VirtualItem item;
            foreach (ListViewItem lvi in virtualListView1.Items)
            {
                item = (VirtualItem)lvi.Tag;
                if (!item.IsFolder)
                    list.Add(item.Text);
            }
            return list;
        }

        private void CopyFile(String filePath, String dest)
        {
            using (FileStream fs = 
                    new FileStream(
                        filePath, 
                        FileMode.Open, 
                        FileAccess.Read, 
                        FileShare.Read, 
                        8192, 
                        FileOptions.SequentialScan))
            {
                uint used, free;
                long length = fs.Length;
                disk.Free(out used, out free);
                if (free > length)
                {
                    int copied = 0;
                    OpProgressChangedEventArgs e = new OpProgressChangedEventArgs(0);
                    OnProgressChange(e);
                    String path = dest;
                    if (String.Compare(dest, "V:\\", true) == 0)
                        path += Path.GetFileName(filePath);
                    else
                        path += "\\" + Path.GetFileName(filePath);
                    byte[] data = new byte[32768];
                    int fd = disk.Open(path, OPENMODE.CREATE | OPENMODE.WRITE, (uint)length);
                    int bytes = fs.Read(data, 0, 32768);
                    while (bytes > 0)
                    {
                        disk.Write(fd, data, 0, bytes);
                        perfChart.UpdateValues();
                        e.Progress = (int)(((float)copied / (float)length) * 100.0F);
                        copied += bytes;
                        OnProgressChange(e);
                        bytes = fs.Read(data, 0, 32768);
                    }
                    disk.Close(fd);
                    perfChart.UpdateValues();
                    e.Progress = 0;
                    OnProgressChange(e);
                }
                else
                    MessageBox.Show(
                        String.Format("No se puede copiar {0}, no hay espacio suficiente en disco", Path.GetFileName(filePath)), 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyDirectory(String src, String dest)
        {
            String path = dest;
            String srcPath;
            if (!path.EndsWith("\\")) path += "\\";
            path += Path.GetFileName(src);
            disk.CreateDir(path);
            DirectoryInfo dir = new DirectoryInfo(src);
            FileSystemInfo[] infos = dir.GetFileSystemInfos();
            for (int i = 0; i < infos.Length; i++)
            {
                srcPath = infos[i].FullName;
                Copy(srcPath, path);
            }
        }

        private String[] GetSelectedItems()
        {
            String[] selected = new String[virtualListView1.SelectedItems.Count];
            VirtualItem item;
            for (int i = 0; i < selected.Length; i++)
            {
                item = (VirtualItem)virtualListView1.SelectedItems[i].Tag;
                selected[i] = item.Path;
            }
            return selected;
        }

        private String[] GetSelection()
        {
            String[] selected = new String[virtualListView1.SelectedItems.Count];
            VirtualItem item;
            String path;
            for (int i = 0; i < selected.Length; i++)
            {
                item = (VirtualItem)virtualListView1.SelectedItems[i].Tag;
                path = tempPath + "\\" + item.Text;
                if (item.IsFolder)
                {
                    Directory.CreateDirectory(path);
                    FillTempDirectory(item.Path);
                }
                else
                    File.Create(path).Close();
                selected[i] = path;
            }
            return selected;
        }

        private void FillTempDirectory(String path)
        {
            long offset;
            DirectoryEntry parent = disk.FindEntryAbsolute(path, out offset);
            if (parent.IsFolder)
            {
                Dictionary<String, DirectoryEntry> table = disk.DirectoryLookupTable(parent.FirstCluster, false);
                foreach (String name in table.Keys)
                {
                    try
                    {
                        DirectoryEntry entry;
                        table.TryGetValue(name, out entry);
                        if (entry.IsFolder)
                        {
                            Directory.CreateDirectory(tempPath + path.Substring(2) + "\\" + name);
                            FillTempDirectory(path + "\\" + name);
                        }
                        else if (entry.IsArchive)
                            File.Create(tempPath + path.Substring(2) + "\\" + name).Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void RefreshExplorer()
        {
            TreeNode node = virtualTreeView1.SelectedNode;
            virtualListView1.Wait();
            virtualTreeView1.RefreshNode(node);
            if (node == virtualTreeView1.Root) node.Expand();
            else virtualTreeView1.RefreshNode(node);
            OnSelectedFolderChanged(new SelectedFolderChangedEventArgs(node, node, true));
        }

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

        private void ResetSplitButton(ToolStripSplitButton button)
        {
            if (button.DropDownItems.Count > 0)
            {
                button.DropDownItems.Clear();
                button.Enabled = false;
            }
        }

        private void NavigateDown(String text)
        {
            ResetSplitButton(navFordwardButton);
            AddNavBackFordwardItem(navBackButton, currentNode);
            virtualTreeView1.ExpandSubNode(text);
        }

        private void EnableControl()
        {
            virtualListView1.Enabled = true;
            virtualTreeView1.Enabled = true;
            this.Enabled = true;
        }

        private void DisableControl()
        {
            virtualListView1.Enabled = false;
            virtualTreeView1.Enabled = false;
            this.Enabled = false;
        }

        private List<String> GetLVItemsNames()
        {
            List<String> nameList = new List<String>();
            foreach (ListViewItem lvi in virtualListView1.Items)
                nameList.Add(lvi.Text);
            return nameList;
        }

        #endregion

        #region Public Methods

        public void ShowSectorView()
        {
            new SectorView(disk).ShowDialog();
        }

        public void WaitForExtraction()
        {
            extractMre.WaitOne();
        }

        public void ShowPerformanceChart()
        {
            if (disk != null && disk.Formatted)
            {
                if (perfChart == null || perfChart.IsDisposed)
                {
                    perfChart = new DiskPerformance(disk);
                }
                perfChart.UpdateValues();
                perfChart.Show();
            }
        }

        public void ShowCommandLine()
        {
            if (disk != null && disk.Formatted)
            {
                if (cmd == null || cmd.IsDisposed)
                {
                    cmd = new CommandLine(disk);
                    cmd.CurrentItem = (VirtualItem)currentNode.Tag;
                    cmd.NavCommandEntered += new NavCommandEnteredEventHandler(cmd_NavCommandEntered);
                }
                cmd.Show();
            }
        }

        public bool MountDisk()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            try
            {
                openDlg.Filter = "Archivos de Disco Virtual (*.svd)|*.svd|Todos los archivos (*.*)|*.*";
                openDlg.FilterIndex = 1;
                openDlg.Title = "Montar Disco";
                openDlg.RestoreDirectory = true;
                openDlg.Multiselect = false;
                if (openDlg.ShowDialog() == DialogResult.OK)
                {
                    if (!openDlg.FileName.EndsWith(".svd"))
                    {
                        MessageBox.Show(
                            "El archivo no tiene extension svd.\n    El disco no se ha montado.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                    disk = new Disk(openDlg.FileName);
                    disk.Mount();
                    if (disk.Formatted)
                    {
                        cmd = new CommandLine(disk);
                        cmd.NavCommandEntered += new NavCommandEnteredEventHandler(cmd_NavCommandEntered);
                        perfChart = new DiskPerformance(disk);
                        virtualTreeView1.LoadRoot(new VirtualItem(disk));
                        buttonsToolStrip.Enabled = true;
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        public bool MountDisk(String filename)
        {
            try
            {
                disk = new Disk(filename);
                disk.Mount();
                if (disk.Formatted)
                {
                    cmd = new CommandLine(disk);
                    cmd.NavCommandEntered += new NavCommandEnteredEventHandler(cmd_NavCommandEntered);
                    perfChart = new DiskPerformance(disk);
                    virtualTreeView1.LoadRoot(new VirtualItem(disk));
                    buttonsToolStrip.Enabled = true;
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        public bool UnmountDisk()
        {
            if (disk != null && !disk.Busy)
            {
                try
                {
                    if (disk.Formatted)
                    {
                        virtualTreeView1.Unload();
                        virtualListView1.Unload();
                        addressTextBox.Clear();
                        imageLabel.Image = null;
                        cmd.Close();
                        cmd = null;
                        perfChart.Close();
                        perfChart = null;
                        CleanTempDirectory();
                    }
                    disk.Dismount();
                    disk = null;
                    ResetSplitButton(navFordwardButton);
                    ResetSplitButton(navBackButton);
                    buttonsToolStrip.Enabled = false;
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        public void FormatDisk()
        {
            if (disk != null && !disk.Busy)
            {
                try
                {
                    if (!disk.Formatted)
                        formatWorker.RunWorkerAsync();
                    else if (MessageBox.Show(
                                "El disco ya tiene formato. ¿Desea formatear de nuevo?",
                                "Formatear Disco",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        formatWorker.RunWorkerAsync();
                        CleanTempDirectory();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}
