using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using FileExplorer.Shell;

namespace FileExplorer.Controls
{
    public delegate void ListViewLoadedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Custom ListView control used in the explorer UserControl
    /// </summary>
    internal sealed class ExplorerListView : ListView
    {
        #region Fields

        private ShellImageList imageList;
        private ExplorerListSorter sorter;
        // BackgroundWorker for loading the list items
        private BackgroundWorker worker;
        private ManualResetEvent mre;

        #endregion

        public event ListViewLoadedEventHandler ListViewLoaded;

        #region Constructor

        public ExplorerListView() : base()
        {
            imageList = ShellImageList.Instance;
            mre = new ManualResetEvent(true);
            HandleCreated += new EventHandler(ExplorerListView_HandleCreated);
            VisibleChanged += new EventHandler(ExplorerListView_VisibleChanged);
            sorter = new ExplorerListSorter();
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            HideSelection = false;
            SetColumns();
            View = View.Details;
            worker = new BackgroundWorker();
            InitializeBackgroundWorker();
        }

        #endregion

        #region Events

        void ExplorerListView_HandleCreated(object sender, EventArgs e)
        {
            imageList.SetSmallImageList(this);
            imageList.SetLargeImageList(this);
        }

        void ExplorerListView_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                imageList.SetSmallImageList(this);
                imageList.SetLargeImageList(this);
            }
        }

        private void GetListViewItems(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            Stopwatch watch = new Stopwatch();
            ShellItem parent = e.Argument as ShellItem;
            watch.Reset();
            watch.Start();
            List<ShellItem> subFolders = parent.GetFolders(false);
            List<ShellItem> subFiles = parent.GetFiles();
            watch.Stop();
            int foldersCount = subFolders.Count;
            int filesCount = subFiles.Count;
            ListViewItem[] files = new ListViewItem[filesCount];
            ListViewItem[] folders = new ListViewItem[foldersCount];

            if (foldersCount + filesCount > 0)
            {
                mre.WaitOne();
                Thread t1 = null;
                Thread t2 = null;
                Thread t3 = null;
                Thread t4 = null;
                bool slow = watch.ElapsedMilliseconds > 800;
                if (slow)
                {
                    int quarter = filesCount >> 2;
                    int half = filesCount >> 1;
                    t1 = new Thread(delegate() { LoadListViewItems(files, subFiles, 0, quarter); });
                    t2 = new Thread(delegate() { LoadListViewItems(files, subFiles, quarter, half); });
                    t3 = new Thread(delegate() { LoadListViewItems(files, subFiles, half, half + quarter); });
                    t4 = new Thread(delegate() { LoadListViewItems(files, subFiles, half + quarter, filesCount); });
                    t1.SetApartmentState(ApartmentState.MTA);
                    t2.SetApartmentState(ApartmentState.MTA);
                    t3.SetApartmentState(ApartmentState.MTA);
                    t4.SetApartmentState(ApartmentState.MTA);
                    t1.Start();
                    t2.Start();
                    t3.Start();
                    t4.Start();
                    LoadListViewItems(folders, subFolders, 0, foldersCount);
                }
                else
                {
                    t1 = new Thread(delegate() { LoadListViewItems(files, subFiles, 0, filesCount); });
                    t1.Start();
                    LoadListViewItems(folders, subFolders, 0, foldersCount);
                }
                Array.Sort(folders, sorter);
                Items.AddRange(folders);

                if (slow)
                {
                    t1.Join();
                    t2.Join();
                    t3.Join();
                    t4.Join();
                }
                else
                    t1.Join();
            }
            worker.ReportProgress(100, files);
        }

        private void AddListViewItems(object sender, ProgressChangedEventArgs e)
        {
            ListViewItem[] items = (ListViewItem[])e.UserState;
            Array.Sort(items, sorter);
            Items.AddRange(items);
            mre.Reset();
            Thread t = new Thread(new ThreadStart(LoadListViewDetails));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void UpdateListViewDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            ListViewLoaded(this, new EventArgs());
        }

        #endregion

        #region Private Methods

        private void InitializeBackgroundWorker()
        {
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(GetListViewItems);
            worker.ProgressChanged += new ProgressChangedEventHandler(AddListViewItems);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateListViewDone);
        }

        private void SetColumns()
        {
            ColumnHeader ch = new ColumnHeader();
            ch.Text = "Nombre";
            ch.Width = 180;
            Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Tamaño";
            ch.Width = 80;
            ch.TextAlign = HorizontalAlignment.Right;
            Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Tipo";
            ch.Width = 100;
            Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Fecha de modificación";
            ch.Width = 130;
            Columns.Add(ch);
        }

        private void LoadListViewItems(ListViewItem[] lvItems, List<ShellItem> shItems, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                ListViewItem lvi = new ListViewItem(shItems[i].Text);
                lvi.Tag = shItems[i];
                lvItems[i] = lvi;
            }
        }

        private void LoadListViewDetails()
        {
            DateTime empty = new DateTime(1, 1, 1, 0, 0, 0);

            foreach (ListViewItem lvi in Items)
            {
                ShellItem item = (ShellItem)lvi.Tag;
                if (!item.IsFolder && item.IsFileSystem)
                {
                    if (item.Length > 1024)
                        lvi.SubItems.Add(String.Format("{0:#,###} KB", item.Length >> 10));
                    else
                        lvi.SubItems.Add(String.Format("{0:##0}  B", item.Length));
                }
                else
                    lvi.SubItems.Add("");
                lvi.SubItems.Add(item.Type);
                if (item.IsDisk)
                    lvi.SubItems.Add("");
                else
                    if (item.LastWriteTime == empty)
                        lvi.SubItems.Add("");
                    else
                        lvi.SubItems.Add(
                            item.LastWriteTime.ToShortDateString() +
                            " " +
                            item.LastWriteTime.ToShortTimeString());
                lvi.ImageIndex = imageList.GetIconIndex(item, false);
            }
            mre.Set();
        }

        #endregion

        #region Public Methods

        public void Wait()
        {
            mre.WaitOne();
        }

        public void RefreshListView(ShellItem parent)
        {
            mre.WaitOne();
            this.Cursor = Cursors.WaitCursor;
            foreach (ListViewItem lvi in Items)
            {
                ShellItem item = (ShellItem)lvi.Tag;
                if (!item.IsFolder)
                    item.Dispose();
            }
            Items.Clear();
            worker.RunWorkerAsync(parent);
        }

        #endregion
    }
}
