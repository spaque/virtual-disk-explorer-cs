namespace Virtuality_Explorer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploradorDeWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exploradorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.líneaDeComandosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rendimientoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verSectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.montarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crearDiscoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.montarToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.desmontarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeVirtualityExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.explorer1 = new FileExplorer.Explorer();
            this.virtualExplorer1 = new VirtualDrive.VirtualExplorer();
            this.createDiskWorker = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.verToolStripMenuItem,
            this.montarToolStripMenuItem,
            this.ayudaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.salirToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.archivoToolStripMenuItem.Text = "&Archivo";
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.salirToolStripMenuItem.MouseLeave += new System.EventHandler(this.salirToolStripMenuItem_MouseLeave);
            this.salirToolStripMenuItem.MouseEnter += new System.EventHandler(this.salirToolStripMenuItem_MouseEnter);
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // verToolStripMenuItem
            // 
            this.verToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exploradorDeWindowsToolStripMenuItem,
            this.exploradorToolStripMenuItem,
            this.líneaDeComandosToolStripMenuItem,
            this.rendimientoToolStripMenuItem,
            this.verSectorToolStripMenuItem});
            this.verToolStripMenuItem.Name = "verToolStripMenuItem";
            this.verToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.verToolStripMenuItem.Text = "&Ver";
            // 
            // exploradorDeWindowsToolStripMenuItem
            // 
            this.exploradorDeWindowsToolStripMenuItem.Name = "exploradorDeWindowsToolStripMenuItem";
            this.exploradorDeWindowsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.exploradorDeWindowsToolStripMenuItem.Text = "Explorador de Windows";
            this.exploradorDeWindowsToolStripMenuItem.MouseLeave += new System.EventHandler(this.exploradorDeWindowsToolStripMenuItem_MouseLeave);
            this.exploradorDeWindowsToolStripMenuItem.MouseEnter += new System.EventHandler(this.exploradorDeWindowsToolStripMenuItem_MouseEnter);
            this.exploradorDeWindowsToolStripMenuItem.Click += new System.EventHandler(this.exploradorDeWindowsToolStripMenuItem_Click);
            // 
            // exploradorToolStripMenuItem
            // 
            this.exploradorToolStripMenuItem.Name = "exploradorToolStripMenuItem";
            this.exploradorToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.exploradorToolStripMenuItem.Text = "Explorador de Disco Virtual";
            this.exploradorToolStripMenuItem.MouseLeave += new System.EventHandler(this.exploradorToolStripMenuItem_MouseLeave);
            this.exploradorToolStripMenuItem.MouseEnter += new System.EventHandler(this.exploradorToolStripMenuItem_MouseEnter);
            this.exploradorToolStripMenuItem.Click += new System.EventHandler(this.exploradorToolStripMenuItem_Click);
            // 
            // líneaDeComandosToolStripMenuItem
            // 
            this.líneaDeComandosToolStripMenuItem.Name = "líneaDeComandosToolStripMenuItem";
            this.líneaDeComandosToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.líneaDeComandosToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.líneaDeComandosToolStripMenuItem.Text = "Línea de Comandos";
            this.líneaDeComandosToolStripMenuItem.MouseLeave += new System.EventHandler(this.líneaDeComandosToolStripMenuItem_MouseLeave);
            this.líneaDeComandosToolStripMenuItem.MouseEnter += new System.EventHandler(this.líneaDeComandosToolStripMenuItem_MouseEnter);
            this.líneaDeComandosToolStripMenuItem.Click += new System.EventHandler(this.líneaDeComandosToolStripMenuItem_Click);
            // 
            // rendimientoToolStripMenuItem
            // 
            this.rendimientoToolStripMenuItem.Name = "rendimientoToolStripMenuItem";
            this.rendimientoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.rendimientoToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.rendimientoToolStripMenuItem.Text = "Rendimiento";
            this.rendimientoToolStripMenuItem.MouseLeave += new System.EventHandler(this.rendimientoToolStripMenuItem_MouseLeave);
            this.rendimientoToolStripMenuItem.MouseEnter += new System.EventHandler(this.rendimientoToolStripMenuItem_MouseEnter);
            this.rendimientoToolStripMenuItem.Click += new System.EventHandler(this.rendimientoToolStripMenuItem_Click);
            // 
            // verSectorToolStripMenuItem
            // 
            this.verSectorToolStripMenuItem.Name = "verSectorToolStripMenuItem";
            this.verSectorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.verSectorToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.verSectorToolStripMenuItem.Text = "Ver Sector";
            this.verSectorToolStripMenuItem.MouseLeave += new System.EventHandler(this.verSectorToolStripMenuItem_MouseLeave);
            this.verSectorToolStripMenuItem.MouseEnter += new System.EventHandler(this.verSectorToolStripMenuItem_MouseEnter);
            this.verSectorToolStripMenuItem.Click += new System.EventHandler(this.verSectorToolStripMenuItem_Click);
            // 
            // montarToolStripMenuItem
            // 
            this.montarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.crearDiscoToolStripMenuItem,
            this.montarToolStripMenuItem1,
            this.desmontarToolStripMenuItem,
            this.formatearToolStripMenuItem});
            this.montarToolStripMenuItem.Name = "montarToolStripMenuItem";
            this.montarToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.montarToolStripMenuItem.Text = "&Disco";
            // 
            // crearDiscoToolStripMenuItem
            // 
            this.crearDiscoToolStripMenuItem.Name = "crearDiscoToolStripMenuItem";
            this.crearDiscoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.crearDiscoToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.crearDiscoToolStripMenuItem.Text = "Crear Disco";
            this.crearDiscoToolStripMenuItem.MouseLeave += new System.EventHandler(this.crearDiscoToolStripMenuItem_MouseLeave);
            this.crearDiscoToolStripMenuItem.MouseEnter += new System.EventHandler(this.crearDiscoToolStripMenuItem_MouseEnter);
            this.crearDiscoToolStripMenuItem.Click += new System.EventHandler(this.crearDiscoToolStripMenuItem_Click);
            // 
            // montarToolStripMenuItem1
            // 
            this.montarToolStripMenuItem1.Name = "montarToolStripMenuItem1";
            this.montarToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.montarToolStripMenuItem1.Size = new System.Drawing.Size(176, 22);
            this.montarToolStripMenuItem1.Text = "Montar";
            this.montarToolStripMenuItem1.MouseLeave += new System.EventHandler(this.montarToolStripMenuItem1_MouseLeave);
            this.montarToolStripMenuItem1.MouseEnter += new System.EventHandler(this.montarToolStripMenuItem1_MouseEnter);
            this.montarToolStripMenuItem1.Click += new System.EventHandler(this.montarToolStripMenuItem1_Click);
            // 
            // desmontarToolStripMenuItem
            // 
            this.desmontarToolStripMenuItem.Name = "desmontarToolStripMenuItem";
            this.desmontarToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.desmontarToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.desmontarToolStripMenuItem.Text = "Desmontar";
            this.desmontarToolStripMenuItem.MouseLeave += new System.EventHandler(this.desmontarToolStripMenuItem_MouseLeave);
            this.desmontarToolStripMenuItem.MouseEnter += new System.EventHandler(this.desmontarToolStripMenuItem_MouseEnter);
            this.desmontarToolStripMenuItem.Click += new System.EventHandler(this.desmontarToolStripMenuItem_Click);
            // 
            // formatearToolStripMenuItem
            // 
            this.formatearToolStripMenuItem.Name = "formatearToolStripMenuItem";
            this.formatearToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.formatearToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.formatearToolStripMenuItem.Text = "Formatear";
            this.formatearToolStripMenuItem.MouseLeave += new System.EventHandler(this.formatearToolStripMenuItem_MouseLeave);
            this.formatearToolStripMenuItem.MouseEnter += new System.EventHandler(this.formatearToolStripMenuItem_MouseEnter);
            this.formatearToolStripMenuItem.Click += new System.EventHandler(this.formatearToolStripMenuItem_Click);
            // 
            // ayudaToolStripMenuItem
            // 
            this.ayudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acercaDeVirtualityExplorerToolStripMenuItem});
            this.ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            this.ayudaToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.ayudaToolStripMenuItem.Text = "A&yuda";
            // 
            // acercaDeVirtualityExplorerToolStripMenuItem
            // 
            this.acercaDeVirtualityExplorerToolStripMenuItem.Name = "acercaDeVirtualityExplorerToolStripMenuItem";
            this.acercaDeVirtualityExplorerToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.acercaDeVirtualityExplorerToolStripMenuItem.Text = "Acerca de Virtuality Explorer";
            this.acercaDeVirtualityExplorerToolStripMenuItem.MouseLeave += new System.EventHandler(this.acercaDeVirtualityExplorerToolStripMenuItem_MouseLeave);
            this.acercaDeVirtualityExplorerToolStripMenuItem.MouseEnter += new System.EventHandler(this.acercaDeVirtualityExplorerToolStripMenuItem_MouseEnter);
            this.acercaDeVirtualityExplorerToolStripMenuItem.Click += new System.EventHandler(this.acercaDeVirtualityExplorerToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel1,
            this.progressBar,
            this.statusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 534);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel1
            // 
            this.statusLabel1.AutoSize = false;
            this.statusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusLabel1.Name = "statusLabel1";
            this.statusLabel1.Size = new System.Drawing.Size(336, 17);
            this.statusLabel1.Spring = true;
            this.statusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.AutoSize = false;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            this.progressBar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusLabel3
            // 
            this.statusLabel3.AutoSize = false;
            this.statusLabel3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLabel3.Name = "statusLabel3";
            this.statusLabel3.Size = new System.Drawing.Size(250, 17);
            this.statusLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.explorer1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.virtualExplorer1);
            this.splitContainer1.Size = new System.Drawing.Size(784, 510);
            this.splitContainer1.SplitterDistance = 255;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 2;
            // 
            // explorer1
            // 
            this.explorer1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.explorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorer1.Location = new System.Drawing.Point(0, 0);
            this.explorer1.Name = "explorer1";
            this.explorer1.Size = new System.Drawing.Size(784, 255);
            this.explorer1.TabIndex = 0;
            // 
            // virtualExplorer1
            // 
            this.virtualExplorer1.AllowDrop = true;
            this.virtualExplorer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.virtualExplorer1.Location = new System.Drawing.Point(0, 0);
            this.virtualExplorer1.Name = "virtualExplorer1";
            this.virtualExplorer1.Size = new System.Drawing.Size(784, 253);
            this.virtualExplorer1.TabIndex = 0;
            // 
            // createDiskWorker
            // 
            this.createDiskWorker.WorkerReportsProgress = true;
            this.createDiskWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.createDiskWorker_DoWork);
            this.createDiskWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.createDiskWorker_ProgressChanged);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(784, 556);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Virtuality Explorer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem montarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acercaDeVirtualityExplorerToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel3;
        private FileExplorer.Explorer explorer1;
        private VirtualDrive.VirtualExplorer virtualExplorer1;
        private System.Windows.Forms.ToolStripMenuItem crearDiscoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem montarToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem formatearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploradorDeWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploradorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem líneaDeComandosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem desmontarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rendimientoToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker createDiskWorker;
        private System.Windows.Forms.ToolStripMenuItem verSectorToolStripMenuItem;
    }
}

