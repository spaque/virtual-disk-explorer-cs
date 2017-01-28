using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

using FileExplorer.Shell;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    public delegate void ListViewLoadedEventHandler(object sender, EventArgs e);

    internal class VirtualListView : ListView
    {
        #region Fields

        private ShellImageList imageList;
        private VirtualListViewSorter sorter;
        private BackgroundWorker worker;
        private ManualResetEvent mre;

        #endregion

        public event ListViewLoadedEventHandler ListViewLoaded;

        #region Constructor

        public VirtualListView() : base()
        {
            imageList = ShellImageList.Instance;
            mre = new ManualResetEvent(true);
            HandleCreated += new EventHandler(VirtualListView_HandleCreated);
            VisibleChanged += new EventHandler(VirtualListView_VisibleChanged);
            sorter = new VirtualListViewSorter();
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            HideSelection = false;
            ShowItemToolTips = true;
            SetColumns();
            View = View.Details;
            worker = new BackgroundWorker();
            InitializeBackgroundWorker();
        }

        #endregion

        #region Events

        void VirtualListView_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                imageList.SetSmallImageList(this);
                imageList.SetLargeImageList(this);
            }
        }

        void VirtualListView_HandleCreated(object sender, EventArgs e)
        {
            imageList.SetLargeImageList(this);
            imageList.SetSmallImageList(this);
        }

        private void GetListViewItems(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            VirtualItem vItem = (VirtualItem)e.Argument;
            List<VirtualItem> subItems = vItem.GetItems(false,true);
            int count = subItems.Count;
            ListViewItem[] lvItems = new ListViewItem[count];

            for (int i = 0; i < count; i++)
            {
                ListViewItem lvi = new ListViewItem(subItems[i].Text);
                lvi.Tag = subItems[i];
                lvItems[i] = lvi;
            }
            Array.Sort(lvItems, sorter);
            worker.ReportProgress(100, lvItems);
        }

        private void AddListViewItems(object sender, ProgressChangedEventArgs e)
        {
            ListViewItem[] lvItems = (ListViewItem[])e.UserState;
            Items.AddRange(lvItems);
            mre.Reset();
            Thread t = new Thread(new ThreadStart(LoadListViewDetails));
            t.Start();
        }

        private void UpdateListViewDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            ListViewLoaded(this, new EventArgs());
        }

        #endregion

        #region Private Methods

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

        private void InitializeBackgroundWorker()
        {
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(GetListViewItems);
            worker.ProgressChanged += new ProgressChangedEventHandler(AddListViewItems);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateListViewDone);
        }

        private void LoadListViewDetails()
        {
            foreach (ListViewItem lvi in Items)
            {
                try
                {
                    VirtualItem item = (VirtualItem)lvi.Tag;
                    if (!item.IsFolder)
                    {
                        if (item.Size > 1024)
                            lvi.SubItems.Add(String.Format("{0:#,###} KB", item.Size >> 10));
                        else
                            lvi.SubItems.Add(String.Format("{0:##0}  B", item.Size));
                    }
                    else
                        lvi.SubItems.Add("");
                    lvi.SubItems.Add(item.Type);
                    lvi.SubItems.Add(
                        item.ModifiedDate.ToShortDateString() +
                        " " +
                        item.ModifiedDate.ToShortTimeString());
                    lvi.ImageIndex = item.ImageIndex;
                    lvi.ToolTipText = item.ToolTip;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            mre.Set();
        }

        #endregion

        #region Public Methods

        public void Unload()
        {
            Items.Clear();
        }

        public void Wait()
        {
            mre.WaitOne();
        }

        public void RefreshListView(VirtualItem parent)
        {
            mre.WaitOne();
            Cursor = Cursors.WaitCursor;
            Items.Clear();
            worker.RunWorkerAsync(parent);
        }

        #endregion
    }
}
