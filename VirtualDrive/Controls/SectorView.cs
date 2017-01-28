using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

using VirtualDrive.FileSystem.FAT32;

namespace VirtualDrive.Controls
{
    public partial class SectorView : Form
    {
        private Disk disk;

        public SectorView(Disk disk)
        {
            InitializeComponent();
            this.disk = disk;
        }

        private void seeSectorButton_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSector((int)numericUpDown1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSector(int sector)
        {
            ushort sectorBytes = disk.BootSector.BytesPerSector;
            byte[] data = new byte[sectorBytes];
            int hexCharsCount = 0;
            StringBuilder hexSb = new StringBuilder();
            StringBuilder charSb = new StringBuilder();
            using (FileStream fstream = 
                    new FileStream(disk.DiskPath, 
                                   FileMode.Open, 
                                   FileAccess.Read, 
                                   FileShare.ReadWrite))
            {
                long offset = sector * sectorBytes;
                fstream.Seek(offset, SeekOrigin.Begin);
                fstream.Read(data, 0, sectorBytes);
            }
            for (int i = 0; i < sectorBytes; i++)
            {
                hexSb.AppendFormat("{0:x2}", data[i]);
                char currentChar = (char)data[i];
                if (Char.IsControl(currentChar))
                    currentChar = '.';
                charSb.Append(currentChar);
                hexCharsCount++;
                if ((i + 1) % 12 == 0)
                {
                    hexSb.AppendLine();
                    charSb.AppendLine();
                    hexCharsCount = 0;
                }
                else if (hexCharsCount == 2)
                {
                    hexSb.Append(" ");
                    hexCharsCount = 0;
                }
            }
            hexTextBox.Text = hexSb.ToString();
            charTextBox.Text = charSb.ToString();
        }
    }
}