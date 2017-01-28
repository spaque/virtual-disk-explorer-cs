using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using FileExplorer.Shell;
using VirtualDrive.FileSystem.FAT32;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    public partial class DiskProperties : Form
    {
        private Disk disk;

        public DiskProperties(Disk disk)
        {
            InitializeComponent();
            this.disk = disk;
            InitializeControls();
        }

        private void InitializeControls()
        {
            ShellImageList imageList = ShellImageList.Instance;
            uint usedBytes, freeBytes;
            float used, free;
            disk.Free(out usedBytes, out freeBytes);
            iconPictureBox.Image = imageList.GetIcon(8, false).ToBitmap();
            labelTextBox.Text = disk.BootSector.VolumeLabel;
            usedBytesLabel.Text = String.Format("{0:#,###} bytes", usedBytes);

            if (freeBytes > 0)
                freeBytesLabel.Text = String.Format("{0:#,###} bytes", freeBytes);
            else
                freeBytesLabel.Text = "0 bytes";

            if (usedBytes > 1048576)
            {
                used = (float)usedBytes / 1048576.0f;
                usedLabel.Text = String.Format("{0:0.00} MB", used);
            }
            else if (usedBytes > 1024)
            {
                used = (float)usedBytes / 1024.0f;
                usedLabel.Text = String.Format("{0:0.00} KB", used);
            }
            else
            {
                usedLabel.Text = String.Format("{0:#,###} B", usedBytes);
            }

            if (freeBytes > 1048576)
            {
                free = (float)freeBytes / 1048576.0f;
                freeLabel.Text = String.Format("{0:0.00} MB", free);
            }
            else if (freeBytes > 1024)
            {
                free = (float)freeBytes / 1024.0f;
                freeLabel.Text = String.Format("{0:0.00} KB", free);
            }
            else if (freeBytes > 0)
            {
                freeLabel.Text = String.Format("{0:#,###} B", freeBytes);
            }
            else
                freeLabel.Text = "0 B";

            piePictureBox.Image = PieImage(100, 100, usedBytes / 1024, freeBytes / 1024);
        }

        private Bitmap PieImage(int width, int height, float used, float free)
        {
            // Create a new image and erase the background
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            SolidBrush brush = new SolidBrush(Form.DefaultBackColor);
            graphics.FillRectangle(brush, 0, 0, width, height);
            brush.Dispose();

            // Create brushes for coloring the pie chart
            SolidBrush usedBrush = new SolidBrush(Color.Blue);
            SolidBrush freeBrush = new SolidBrush(Color.Magenta);

            // Sum the inputs to get the total
            float total = used + free;

            // Draw the pie chart
            float end = (used / total) * 360.0f;
            graphics.FillPie(usedBrush, 0.0f, 0.0f, width, height, 0.0f, end);
            graphics.FillPie(freeBrush, 0.0f, 0.0f, width, height, end, 360.0f - end);

            // Clean up the brush resources
            usedBrush.Dispose();
            freeBrush.Dispose();

            return bitmap;
        }
    }
}