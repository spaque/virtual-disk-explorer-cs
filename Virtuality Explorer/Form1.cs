using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using FileExplorer.Controls;
using FileExplorer.Shell;
using FileExplorer;
using VirtualDrive.FileSystem.FAT32;

namespace Virtuality_Explorer
{
    public partial class Form1 : Form
    {
        private ShellImageList imageList;

        #region Constructor

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            imageList = ShellImageList.Instance;

            InitializeControls();
        }

        public Form1(String disk)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            imageList = ShellImageList.Instance;

            InitializeControls();

            MountDisk(disk);
        }

        #endregion

        #region Private Methods

        private void InitializeControls()
        {
            explorer1.SelectedFolderChanged += new SelectedFolderChangedEventHandler(explorer1_SelectedFolderChanged);

            virtualExplorer1.SelectedFolderChanged += new VirtualDrive.SelectedFolderChangedEventHandler(virtualExplorer1_SelectedFolderChanged);
            virtualExplorer1.ProgressChange += new VirtualDrive.OpProgressChangeEventHandler(virtualExplorer1_OpProgressChanged);

            formatearToolStripMenuItem.Enabled = false;
            desmontarToolStripMenuItem.Enabled = false;
            líneaDeComandosToolStripMenuItem.Enabled = false;
            rendimientoToolStripMenuItem.Enabled = false;
            verSectorToolStripMenuItem.Enabled = false;
            exploradorDeWindowsToolStripMenuItem.Checked = true;
            exploradorToolStripMenuItem.Checked = true;

            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void MountDisk(String filename)
        {
            if (virtualExplorer1.MountDisk(filename))
            {
                desmontarToolStripMenuItem.Enabled = true;
                formatearToolStripMenuItem.Enabled = true;
                montarToolStripMenuItem1.Enabled = false;
                if (virtualExplorer1.DiskFormatted)
                {
                    líneaDeComandosToolStripMenuItem.Enabled = true;
                    rendimientoToolStripMenuItem.Enabled = true;
                    verSectorToolStripMenuItem.Enabled = true;
                }
            }
        }

        #endregion

        #region Events

        void virtualExplorer1_OpProgressChanged(object sender, VirtualDrive.Controls.OpProgressChangedEventArgs e)
        {
            progressBar.Value = e.Progress;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void acercaDeVirtualityExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void explorer1_SelectedFolderChanged(object sender, SelectedFolderChangedEventArgs e)
        {
            statusLabel3.Text = e.NewNode.Text;
            statusLabel3.Image = imageList.GetIcon(e.NewNode.ImageIndex, true).ToBitmap();
        }

        void virtualExplorer1_SelectedFolderChanged(object sender, VirtualDrive.Controls.SelectedFolderChangedEventArgs e)
        {
            statusLabel3.Text = e.NewNode.Text;
            statusLabel3.Image = imageList.GetIcon(e.NewNode.ImageIndex, true).ToBitmap();
        }

        void createDiskWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        void createDiskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker woker = (BackgroundWorker)sender;
            int size = (int)Disk.DiskSize;
            int current = 0;
            String filename = (String)e.Argument;
            woker.ReportProgress(0);
            using (FileStream fstream =
                new FileStream(filename,
                               FileMode.Create,
                               FileAccess.Write,
                               FileShare.None,
                               8192,
                               FileOptions.SequentialScan))
            {
                byte[] zero = new byte[4096];
                int count = (int)(Disk.DiskSize >> 12);
                for (int i = 0; i < count; i++)
                {
                    fstream.Write(zero, 0, 4096);
                    current += 4096;
                    woker.ReportProgress((int)(((float)current / (float)size) * 100.0F));
                }
            }
            woker.ReportProgress(0);
        }

        private void crearDiscoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();

            saveDlg.Filter = "Archivos de Disco Virtual (*.svd)|*.svd|Todos los archivos (*.*)|*.*";
            saveDlg.FilterIndex = 1;
            saveDlg.Title = "Crear Disco";
            saveDlg.OverwritePrompt = true;
            saveDlg.RestoreDirectory = true;
            saveDlg.ValidateNames = true;
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                createDiskWorker.RunWorkerAsync(saveDlg.FileName);
            }
        }

        private void montarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (virtualExplorer1.MountDisk())
            {
                desmontarToolStripMenuItem.Enabled = true;
                formatearToolStripMenuItem.Enabled = true;
                montarToolStripMenuItem1.Enabled = false;
                if (virtualExplorer1.DiskFormatted)
                {
                    líneaDeComandosToolStripMenuItem.Enabled = true;
                    rendimientoToolStripMenuItem.Enabled = true;
                    verSectorToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void desmontarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (virtualExplorer1.UnmountDisk())
            {
                formatearToolStripMenuItem.Enabled = false;
                desmontarToolStripMenuItem.Enabled = false;
                montarToolStripMenuItem1.Enabled = true;
                líneaDeComandosToolStripMenuItem.Enabled = false;
                rendimientoToolStripMenuItem.Enabled = false;
                verSectorToolStripMenuItem.Enabled = false;
            }
        }

        private void formatearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            virtualExplorer1.FormatDisk();
            líneaDeComandosToolStripMenuItem.Enabled = true;
            rendimientoToolStripMenuItem.Enabled = true;
            verSectorToolStripMenuItem.Enabled = true;
        }

        private void exploradorDeWindowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (exploradorDeWindowsToolStripMenuItem.Checked)
            {
                if (exploradorToolStripMenuItem.Checked)
                {
                    exploradorDeWindowsToolStripMenuItem.Checked = false;
                    splitContainer1.Panel1Collapsed = true;
                }
            }
            else
            {
                exploradorDeWindowsToolStripMenuItem.Checked = true;
                splitContainer1.Panel1Collapsed = false;
            }
        }

        private void exploradorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (exploradorToolStripMenuItem.Checked)
            {
                if (exploradorDeWindowsToolStripMenuItem.Checked)
                {
                    exploradorToolStripMenuItem.Checked = false;
                    splitContainer1.Panel2Collapsed = true;
                }
            }
            else
            {
                exploradorToolStripMenuItem.Checked = true;
                splitContainer1.Panel2Collapsed = false;
            }
        }

        private void líneaDeComandosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            virtualExplorer1.ShowCommandLine();
        }

        private void rendimientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            virtualExplorer1.ShowPerformanceChart();
        }

        private void verSectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            virtualExplorer1.ShowSectorView();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (virtualExplorer1.DiskMounted)
                {
                    if (virtualExplorer1.DiskBusy)
                    {
                        if (MessageBox.Show(
                            "El disco está ocupado. ¿Desea salir de todas formas?",
                            "Disco ocupado", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.No)
                            e.Cancel = true;
                        return;
                    }
                    if (!virtualExplorer1.UnmountDisk())
                        e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = false;
            }
        }

        private void crearDiscoToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void crearDiscoToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Crea un disco virtual nuevo (sin formatear).";
        }

        private void montarToolStripMenuItem1_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Monta un disco virtual y lo hace accesible para operación.";
        }

        private void montarToolStripMenuItem1_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void desmontarToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Desmonta el disco virtual.";
        }

        private void desmontarToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void formatearToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Formatea el disco virtual actualmente montado.";
        }

        private void formatearToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void acercaDeVirtualityExplorerToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra información sobre el programa.";
        }

        private void acercaDeVirtualityExplorerToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void exploradorDeWindowsToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra u oculta el explorador de windows.";
        }

        private void exploradorDeWindowsToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void exploradorToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra u oculta el explorador del disco virtual.";
        }

        private void exploradorToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void líneaDeComandosToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra la línea de comandos del disco virtual.";
        }

        private void líneaDeComandosToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void rendimientoToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra el rendimiento y estadísticas del disco virtual.";
        }

        private void rendimientoToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void verSectorToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Muestra el contenido de un sector.";
        }

        private void verSectorToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        private void salirToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            statusLabel1.Text = "Cierra la aplicación.";
        }

        private void salirToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            statusLabel1.Text = "";
        }

        #endregion
    }
}