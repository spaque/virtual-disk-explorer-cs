using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using VirtualDrive.Shell;
using VirtualDrive.FileSystem.FAT32;
using FileExplorer.Shell;

namespace VirtualDrive.Controls
{
    public partial class EntryProperties : Form
    {
        public EntryProperties(VirtualItem item, Disk disk)
        {
            InitializeComponent();
            InitializeControls(item, disk);
        }

        private void InitializeControls(VirtualItem item, Disk disk)
        {
            ShellImageList imageList = ShellImageList.Instance;
            long size;
            long sizeInDisk;
            float shortSize;
            float shortSizeInDisk;
            this.Text = "Propiedades de " + item.Text;
            pictureBox1.Image = imageList.GetIcon(item.ImageIndex, false).ToBitmap();
            nameTextBox.Text = item.Text;
            typeLabel.Text = item.Type;
            pathLabel.Text = Path.GetDirectoryName(item.Path);
            clusterLabel.Text = String.Format("{0:#,###}", item.FirstCluster);
            if (item.IsFolder)
                size = disk.EntrySize(item.FirstCluster);
            else
                size = item.Size;
            double sectors = Math.Ceiling((double)size / (double)BootSector.SectorSize);
            sizeInDisk = (long)sectors * BootSector.SectorSize;
            if (size > 1048576)
            {
                shortSize = (float)size / 1048576.0f;
                entrySizeLabel.Text = String.Format("{0:0.00} MB ({1:#,###} bytes)", shortSize, size);
            }
            else if (size > 1024)
            {
                shortSize = (float)size / 1024.0f;
                entrySizeLabel.Text = String.Format("{0:0.00} KB ({1:#,###} bytes)", shortSize, size);
            }
            else
            {
                entrySizeLabel.Text = String.Format("{0:#,###;Zero} bytes", size);
            }

            if (sizeInDisk > 1048576)
            {
                shortSizeInDisk = (float)sizeInDisk / 1048576.0f;
                totalSizeLabel.Text = String.Format("{0:0.00} MB ({1:#,###} bytes)", shortSizeInDisk, sizeInDisk);
            }
            else if (sizeInDisk > 1024)
            {
                shortSizeInDisk = (float)sizeInDisk / 1024.0f;
                totalSizeLabel.Text = String.Format("{0:0.00} KB ({1:#,###} bytes)", shortSizeInDisk, sizeInDisk);
            }
            else
                totalSizeLabel.Text = String.Format("{0:#,###;Zero} bytes", sizeInDisk);
            createdLabel.Text = item.ModifiedDate.ToLongDateString() + ", " + 
                                item.ModifiedDate.ToLongTimeString();
        }
    }
}