namespace VirtualDrive.Controls
{
    partial class DiskPerformance
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            VirtualDrive.Controls.PerformanceChartStyle performanceChartStyle2 = new VirtualDrive.Controls.PerformanceChartStyle();
            VirtualDrive.Controls.ChartPen chartPen6 = new VirtualDrive.Controls.ChartPen();
            VirtualDrive.Controls.ChartPen chartPen7 = new VirtualDrive.Controls.ChartPen();
            VirtualDrive.Controls.ChartPen chartPen8 = new VirtualDrive.Controls.ChartPen();
            VirtualDrive.Controls.ChartPen chartPen9 = new VirtualDrive.Controls.ChartPen();
            VirtualDrive.Controls.ChartPen chartPen10 = new VirtualDrive.Controls.ChartPen();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.speedComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.performanceChart1 = new VirtualDrive.Controls.PerformanceChart();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.freeLabel = new System.Windows.Forms.Label();
            this.usedLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.maxRateLabel = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statisticsLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.speedComboBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.performanceChart1);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 197);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tasa de transferencia";
            // 
            // speedComboBox
            // 
            this.speedComboBox.FormattingEnabled = true;
            this.speedComboBox.Items.AddRange(new object[] {
            "Alta",
            "Normal",
            "Lenta"});
            this.speedComboBox.Location = new System.Drawing.Point(171, 164);
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.Size = new System.Drawing.Size(71, 21);
            this.speedComboBox.TabIndex = 2;
            this.speedComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Velocidad de actualización";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // performanceChart1
            // 
            this.performanceChart1.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.performanceChart1.Dock = System.Windows.Forms.DockStyle.Top;
            this.performanceChart1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.performanceChart1.Location = new System.Drawing.Point(3, 16);
            this.performanceChart1.MaxRate = new decimal(new int[] {
            800,
            0,
            0,
            65536});
            this.performanceChart1.Name = "performanceChart1";
            performanceChartStyle2.AntiAliasing = true;
            chartPen6.Color = System.Drawing.Color.LightGreen;
            chartPen6.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen6.Width = 1F;
            performanceChartStyle2.AvgLinePen = chartPen6;
            performanceChartStyle2.BackgroundColorBottom = System.Drawing.Color.DarkOliveGreen;
            performanceChartStyle2.BackgroundColorTop = System.Drawing.Color.YellowGreen;
            chartPen7.Color = System.Drawing.Color.Gold;
            chartPen7.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen7.Width = 1F;
            performanceChartStyle2.ChartLinePen = chartPen7;
            chartPen8.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen8.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen8.Width = 0.5F;
            performanceChartStyle2.HorizontalGridPen = chartPen8;
            chartPen9.Color = System.Drawing.Color.Red;
            chartPen9.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen9.Width = 0.5F;
            performanceChartStyle2.MaxRatePen = chartPen9;
            performanceChartStyle2.ShowAverageLine = true;
            performanceChartStyle2.ShowHorizontalGridLines = true;
            performanceChartStyle2.ShowVerticalGridLines = true;
            chartPen10.Color = System.Drawing.Color.DarkOliveGreen;
            chartPen10.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            chartPen10.Width = 0.5F;
            performanceChartStyle2.VerticalGridPen = chartPen10;
            this.performanceChart1.PerfChartStyle = performanceChartStyle2;
            this.performanceChart1.Size = new System.Drawing.Size(315, 139);
            this.performanceChart1.TabIndex = 0;
            this.performanceChart1.TimerInterval = 1000;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.freeLabel);
            this.groupBox2.Controls.Add(this.usedLabel);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(9, 215);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(318, 53);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Uso de disco";
            // 
            // freeLabel
            // 
            this.freeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.freeLabel.Location = new System.Drawing.Point(228, 25);
            this.freeLabel.Name = "freeLabel";
            this.freeLabel.Size = new System.Drawing.Size(82, 11);
            this.freeLabel.TabIndex = 3;
            this.freeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // usedLabel
            // 
            this.usedLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.usedLabel.Location = new System.Drawing.Point(59, 24);
            this.usedLabel.Name = "usedLabel";
            this.usedLabel.Size = new System.Drawing.Size(82, 11);
            this.usedLabel.TabIndex = 2;
            this.usedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(195, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Libre";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ocupado";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.maxRateLabel);
            this.groupBox3.Controls.Add(this.numericUpDown1);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.statisticsLabel);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(6, 274);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(321, 181);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Estadisticas de disco";
            // 
            // maxRateLabel
            // 
            this.maxRateLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.maxRateLabel.Location = new System.Drawing.Point(168, 25);
            this.maxRateLabel.Name = "maxRateLabel";
            this.maxRateLabel.Size = new System.Drawing.Size(94, 17);
            this.maxRateLabel.TabIndex = 4;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 1;
            this.numericUpDown1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(171, 45);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(43, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            800,
            0,
            0,
            65536});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(10, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Limite tasa de transferencia";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(10, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Tasa de transferencia máxima";
            // 
            // statisticsLabel
            // 
            this.statisticsLabel.Location = new System.Drawing.Point(13, 70);
            this.statisticsLabel.Name = "statisticsLabel";
            this.statisticsLabel.Size = new System.Drawing.Size(247, 98);
            this.statisticsLabel.TabIndex = 0;
            this.statisticsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DiskPerformance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 463);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "DiskPerformance";
            this.ShowIcon = false;
            this.Text = "Estadisticas del disco";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private PerformanceChart performanceChart1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label usedLabel;
        private System.Windows.Forms.Label freeLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label statisticsLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label maxRateLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox speedComboBox;

    }
}