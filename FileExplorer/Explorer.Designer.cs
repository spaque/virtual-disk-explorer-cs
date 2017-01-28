namespace FileExplorer
{
    public sealed partial class Explorer
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

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.imageLabel = new System.Windows.Forms.ToolStripLabel();
            this.addressTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.explorerTreeView1 = new FileExplorer.Controls.ExplorerTreeView();
            this.explorerListView1 = new FileExplorer.Controls.ExplorerListView();
            this.navBackButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navFordwardButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navUpButton = new System.Windows.Forms.ToolStripButton();
            this.foldersButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.cutButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.viewDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewLargeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDetailsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navBackButton,
            this.navFordwardButton,
            this.navUpButton,
            this.toolStripSeparator1,
            this.foldersButton,
            this.searchButton,
            this.refreshButton,
            this.toolStripSeparator2,
            this.copyButton,
            this.cutButton,
            this.pasteButton,
            this.deleteButton,
            this.toolStripSeparator3,
            this.viewDropDownButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(645, 32);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 32);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.imageLabel,
            this.addressTextBox});
            this.toolStrip2.Location = new System.Drawing.Point(0, 32);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(645, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.AutoSize = false;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(55, 22);
            this.toolStripLabel2.Text = "Dirección";
            // 
            // imageLabel
            // 
            this.imageLabel.AutoSize = false;
            this.imageLabel.Name = "imageLabel";
            this.imageLabel.Size = new System.Drawing.Size(22, 22);
            // 
            // addressTextBox
            // 
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.addressTextBox.ReadOnly = true;
            this.addressTextBox.Size = new System.Drawing.Size(491, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 57);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.explorerTreeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.explorerListView1);
            this.splitContainer1.Size = new System.Drawing.Size(645, 341);
            this.splitContainer1.SplitterDistance = 215;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 2;
            this.splitContainer1.TabStop = false;
            // 
            // explorerTreeView1
            // 
            this.explorerTreeView1.AllowDrop = true;
            this.explorerTreeView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.explorerTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerTreeView1.HideSelection = false;
            this.explorerTreeView1.HotTracking = true;
            this.explorerTreeView1.Location = new System.Drawing.Point(0, 0);
            this.explorerTreeView1.Name = "explorerTreeView1";
            this.explorerTreeView1.ShowLines = false;
            this.explorerTreeView1.ShowNodeToolTips = true;
            this.explorerTreeView1.Size = new System.Drawing.Size(215, 341);
            this.explorerTreeView1.Sorted = true;
            this.explorerTreeView1.TabIndex = 0;
            // 
            // explorerListView1
            // 
            this.explorerListView1.AllowDrop = true;
            this.explorerListView1.LabelEdit = true;
            this.explorerListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.explorerListView1.HideSelection = false;
            this.explorerListView1.Location = new System.Drawing.Point(0, 0);
            this.explorerListView1.Name = "explorerListView1";
            this.explorerListView1.Size = new System.Drawing.Size(428, 341);
            this.explorerListView1.TabIndex = 0;
            this.explorerListView1.UseCompatibleStateImageBehavior = false;
            this.explorerListView1.View = System.Windows.Forms.View.Details;
            // 
            // navBackButton
            // 
            this.navBackButton.AutoSize = false;
            this.navBackButton.Enabled = false;
            this.navBackButton.Image = global::FileExplorer.Properties.Resources.bt_left_128x128;
            this.navBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navBackButton.Name = "navBackButton";
            this.navBackButton.Size = new System.Drawing.Size(77, 29);
            this.navBackButton.Text = "Atrás";
            // 
            // navFordwardButton
            // 
            this.navFordwardButton.AutoSize = false;
            this.navFordwardButton.Enabled = false;
            this.navFordwardButton.Image = global::FileExplorer.Properties.Resources.bt_right_128x128;
            this.navFordwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFordwardButton.Name = "navFordwardButton";
            this.navFordwardButton.Size = new System.Drawing.Size(94, 29);
            this.navFordwardButton.Text = "Adelante";
            // 
            // navUpButton
            // 
            this.navUpButton.AutoSize = false;
            this.navUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navUpButton.Enabled = false;
            this.navUpButton.Image = global::FileExplorer.Properties.Resources.bt_up_128x128;
            this.navUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navUpButton.Name = "navUpButton";
            this.navUpButton.Size = new System.Drawing.Size(34, 29);
            this.navUpButton.Text = "Arriba";
            this.navUpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // foldersButton
            // 
            this.foldersButton.AutoSize = false;
            this.foldersButton.Checked = true;
            this.foldersButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.foldersButton.Image = global::FileExplorer.Properties.Resources.folder_128x128;
            this.foldersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.foldersButton.Margin = new System.Windows.Forms.Padding(4, 1, 0, 2);
            this.foldersButton.Name = "foldersButton";
            this.foldersButton.Size = new System.Drawing.Size(80, 29);
            this.foldersButton.Text = "Carpetas";
            // 
            // refreshButton
            // 
            this.refreshButton.AutoSize = false;
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::FileExplorer.Properties.Resources.refresh_128x128;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(31, 29);
            this.refreshButton.Text = "Actualizar";
            // 
            // searchButton
            // 
            this.searchButton.AutoSize = false;
            this.searchButton.Image = global::FileExplorer.Properties.Resources.Search_h;
            this.searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(85, 29);
            this.searchButton.Text = "Búsqueda";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.AutoSize = false;
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::FileExplorer.Properties.Resources.copy_256x256;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(30, 29);
            this.copyButton.Text = "Copiar";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.AutoSize = false;
            this.cutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutButton.Image = global::FileExplorer.Properties.Resources.cut_256x256;
            this.cutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(30, 29);
            this.cutButton.Text = "Cortar";
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.AutoSize = false;
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Image = global::FileExplorer.Properties.Resources.paste_256x256;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(30, 29);
            this.pasteButton.Text = "Pegar";
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.AutoSize = false;
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::FileExplorer.Properties.Resources.delete_256x256;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(30, 29);
            this.deleteButton.Text = "Eliminar";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // viewDropDownButton
            // 
            this.viewDropDownButton.AutoSize = false;
            this.viewDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewLargeMenuItem,
            this.viewListMenuItem,
            this.viewDetailsMenuItem});
            this.viewDropDownButton.Image = global::FileExplorer.Properties.Resources.view_128x128;
            this.viewDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewDropDownButton.Name = "viewDropDownButton";
            this.viewDropDownButton.Size = new System.Drawing.Size(40, 30);
            this.viewDropDownButton.Text = "Vistas";
            this.viewDropDownButton.ToolTipText = "Vistas";
            // 
            // viewLargeMenuItem
            // 
            this.viewLargeMenuItem.Name = "viewLargeMenuItem";
            this.viewLargeMenuItem.Size = new System.Drawing.Size(121, 22);
            this.viewLargeMenuItem.Text = "Iconos";
            this.viewLargeMenuItem.Click += new System.EventHandler(this.viewLargeMenuItem_Click);
            // 
            // viewListMenuItem
            // 
            this.viewListMenuItem.Name = "viewListMenuItem";
            this.viewListMenuItem.Size = new System.Drawing.Size(121, 22);
            this.viewListMenuItem.Text = "Lista";
            this.viewListMenuItem.Click += new System.EventHandler(this.viewListMenuItem_Click);
            // 
            // viewDetailsMenuItem
            // 
            this.viewDetailsMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewDetailsMenuItem.Name = "viewDetailsMenuItem";
            this.viewDetailsMenuItem.Size = new System.Drawing.Size(121, 22);
            this.viewDetailsMenuItem.Text = "Detalles";
            this.viewDetailsMenuItem.Click += new System.EventHandler(this.viewDetailsMenuItem_Click);
            // 
            // Explorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Explorer";
            this.Size = new System.Drawing.Size(645, 398);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSplitButton navBackButton;
        private System.Windows.Forms.ToolStripSplitButton navFordwardButton;
        private System.Windows.Forms.ToolStripButton navUpButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel imageLabel;
        private System.Windows.Forms.ToolStripTextBox addressTextBox;
        private System.Windows.Forms.ToolStripButton foldersButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton cutButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private FileExplorer.Controls.ExplorerTreeView explorerTreeView1;
        private FileExplorer.Controls.ExplorerListView explorerListView1;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton viewDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem viewLargeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDetailsMenuItem;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton searchButton;


    }
}
