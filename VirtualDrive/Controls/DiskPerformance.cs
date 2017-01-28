using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VirtualDrive.FileSystem.FAT32;

namespace VirtualDrive.Controls
{
    public partial class DiskPerformance : Form
    {
        #region Fields

        private Disk disk;
        private decimal maxAchievedRate;

        #endregion

        #region Constructor

        public DiskPerformance(Disk disk)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.disk = disk;
            speedComboBox.SelectedIndex = 1;
        }

        #endregion

        #region Public Methods

        public void UpdateValues()
        {
            uint used, free;
            decimal rate = (decimal)disk.GetRate();
            if (rate > maxAchievedRate)
                maxAchievedRate = rate;
            disk.Free(out used, out free);
            performanceChart1.AddValue(rate);
            usedLabel.Text = String.Format("{0:0.000} MB", (double)used / 1048576.0);
            freeLabel.Text = String.Format("{0:0.000} MB", (double)free / 1048576.0);
            maxRateLabel.Text = String.Format("{0:0.000} MB/s", maxAchievedRate);
            statisticsLabel.Text = disk.Contadores();
        }

        public void AddPerfValue(decimal value)
        {
            performanceChart1.AddValue(value);
        }

        public void DiskUse(uint used, uint free)
        {
            usedLabel.Text = String.Format("{0:0.000} MB", (double)used / 1048576.0);
            freeLabel.Text = String.Format("{0:0.000} MB", (double)free / 1048576.0);
        }

        public void DiskStatistics(String stats)
        {
            statisticsLabel.Text = stats;
        }

        #endregion

        #region Private Methods

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            disk.SetRate((float)numericUpDown1.Value);
            performanceChart1.MaxRate = numericUpDown1.Value;
        }

        #endregion

        #region Events

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item = (String)speedComboBox.SelectedItem;
            if (String.Compare(item, "Alta", true) == 0)
            {
                performanceChart1.TimerInterval = 500;
            }
            else if (String.Compare(item, "Normal", true) == 0)
            {
                performanceChart1.TimerInterval = 1000;
            }
            else if (String.Compare(item, "Lenta", true) == 0)
            {
                performanceChart1.TimerInterval = 2000;
            }
        }

        #endregion
    }
}