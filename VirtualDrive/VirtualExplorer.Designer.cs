namespace VirtualDrive
{
    partial class VirtualExplorer
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
            this.components = new System.ComponentModel.Container();
            this.buttonsToolStrip = new System.Windows.Forms.ToolStrip();
            this.navBackButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navFordwardButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navUpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.foldersButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.copyButton = new System.Windows.Forms.ToolStripButton();
            this.cutButton = new System.Windows.Forms.ToolStripButton();
            this.pasteButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.viewsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewLargeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDetailsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoButton = new System.Windows.Forms.ToolStripButton();
            this.addressToolStrip = new System.Windows.Forms.ToolStrip();
            this.addressLabel = new System.Windows.Forms.ToolStripLabel();
            this.imageLabel = new System.Windows.Forms.ToolStripLabel();
            this.addressTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.verToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iconosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detallesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actualizarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nuevaCarpetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.pegarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.propiedadesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cortarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copiarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.eliminarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cambiarNombreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.propiedadesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.diskContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.formatearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.propiedadesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.virtualTreeView1 = new VirtualDrive.Controls.VirtualTreeView();
            this.virtualListView1 = new VirtualDrive.Controls.VirtualListView();
            this.buttonsToolStrip.SuspendLayout();
            this.addressToolStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.lvContextMenuStrip.SuspendLayout();
            this.itemContextMenu.SuspendLayout();
            this.diskContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonsToolStrip
            // 
            this.buttonsToolStrip.AutoSize = false;
            this.buttonsToolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.buttonsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navBackButton,
            this.navFordwardButton,
            this.navUpButton,
            this.toolStripSeparator1,
            this.foldersButton,
            this.refreshButton,
            this.toolStripSeparator7,
            this.copyButton,
            this.cutButton,
            this.pasteButton,
            this.deleteButton,
            this.toolStripSeparator8,
            this.viewsButton,
            this.infoButton});
            this.buttonsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.buttonsToolStrip.Name = "buttonsToolStrip";
            this.buttonsToolStrip.Size = new System.Drawing.Size(729, 35);
            this.buttonsToolStrip.TabIndex = 0;
            this.buttonsToolStrip.Text = "toolStrip1";
            // 
            // navBackButton
            // 
            this.navBackButton.Image = global::VirtualDrive.Properties.Resources.bt_left_128x128;
            this.navBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navBackButton.Name = "navBackButton";
            this.navBackButton.Size = new System.Drawing.Size(73, 32);
            this.navBackButton.Text = "Atrás";
            // 
            // navFordwardButton
            // 
            this.navFordwardButton.Image = global::VirtualDrive.Properties.Resources.bt_right_128x128;
            this.navFordwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFordwardButton.Name = "navFordwardButton";
            this.navFordwardButton.Size = new System.Drawing.Size(92, 32);
            this.navFordwardButton.Text = "Adelante";
            // 
            // navUpButton
            // 
            this.navUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navUpButton.Image = global::VirtualDrive.Properties.Resources.bt_up_128x128;
            this.navUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navUpButton.Name = "navUpButton";
            this.navUpButton.Size = new System.Drawing.Size(29, 32);
            this.navUpButton.Text = "Arriba";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // foldersButton
            // 
            this.foldersButton.AutoSize = false;
            this.foldersButton.Checked = true;
            this.foldersButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.foldersButton.Image = global::VirtualDrive.Properties.Resources.folder_128x128;
            this.foldersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.foldersButton.Name = "foldersButton";
            this.foldersButton.Size = new System.Drawing.Size(80, 32);
            this.foldersButton.Text = "Carpetas";
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::VirtualDrive.Properties.Resources.refresh_128x128;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(29, 32);
            this.refreshButton.Text = "Actualizar";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 35);
            // 
            // copyButton
            // 
            this.copyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyButton.Image = global::VirtualDrive.Properties.Resources.copy_256x256;
            this.copyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(29, 32);
            this.copyButton.Text = "Copiar";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cutButton.Image = global::VirtualDrive.Properties.Resources.cut_256x256;
            this.cutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cutButton.Name = "cutButton";
            this.cutButton.Size = new System.Drawing.Size(29, 32);
            this.cutButton.Text = "Cortar";
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pasteButton.Image = global::VirtualDrive.Properties.Resources.paste_256x256;
            this.pasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(29, 32);
            this.pasteButton.Text = "Pegar";
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::VirtualDrive.Properties.Resources.delete_256x256;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(29, 32);
            this.deleteButton.Text = "Eliminar";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 35);
            // 
            // viewsButton
            // 
            this.viewsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewLargeMenuItem,
            this.viewListMenuItem,
            this.viewDetailsMenuItem});
            this.viewsButton.Image = global::VirtualDrive.Properties.Resources.view_128x128;
            this.viewsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewsButton.Name = "viewsButton";
            this.viewsButton.Size = new System.Drawing.Size(38, 32);
            this.viewsButton.Text = "Vistas";
            // 
            // viewLargeMenuItem
            // 
            this.viewLargeMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewLargeMenuItem.Name = "viewLargeMenuItem";
            this.viewLargeMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewLargeMenuItem.Text = "Iconos";
            // 
            // viewListMenuItem
            // 
            this.viewListMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewListMenuItem.Name = "viewListMenuItem";
            this.viewListMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewListMenuItem.Text = "Lista";
            // 
            // viewDetailsMenuItem
            // 
            this.viewDetailsMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewDetailsMenuItem.Name = "viewDetailsMenuItem";
            this.viewDetailsMenuItem.Size = new System.Drawing.Size(152, 22);
            this.viewDetailsMenuItem.Text = "Detalles";
            // 
            // infoButton
            // 
            this.infoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.infoButton.Image = global::VirtualDrive.Properties.Resources.InfoBox_256x256;
            this.infoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(29, 32);
            this.infoButton.Text = "Información";
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // addressToolStrip
            // 
            this.addressToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addressLabel,
            this.imageLabel,
            this.addressTextBox});
            this.addressToolStrip.Location = new System.Drawing.Point(0, 35);
            this.addressToolStrip.Name = "addressToolStrip";
            this.addressToolStrip.Size = new System.Drawing.Size(729, 25);
            this.addressToolStrip.TabIndex = 1;
            this.addressToolStrip.Text = "toolStrip1";
            // 
            // addressLabel
            // 
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(53, 22);
            this.addressLabel.Text = "Dirección";
            // 
            // imageLabel
            // 
            this.imageLabel.AutoSize = false;
            this.imageLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.imageLabel.Name = "imageLabel";
            this.imageLabel.Size = new System.Drawing.Size(25, 22);
            // 
            // addressTextBox
            // 
            this.addressTextBox.AutoSize = false;
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.ReadOnly = true;
            this.addressTextBox.Size = new System.Drawing.Size(600, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.virtualTreeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.virtualListView1);
            this.splitContainer1.Size = new System.Drawing.Size(729, 265);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 2;
            // 
            // lvContextMenuStrip
            // 
            this.lvContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verToolStripMenuItem,
            this.actualizarToolStripMenuItem,
            this.nuevaCarpetaToolStripMenuItem,
            this.toolStripSeparator6,
            this.pegarToolStripMenuItem,
            this.toolStripSeparator2,
            this.propiedadesToolStripMenuItem});
            this.lvContextMenuStrip.Name = "lvContextMenuStrip";
            this.lvContextMenuStrip.Size = new System.Drawing.Size(155, 126);
            // 
            // verToolStripMenuItem
            // 
            this.verToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iconosToolStripMenuItem,
            this.listaToolStripMenuItem,
            this.detallesToolStripMenuItem});
            this.verToolStripMenuItem.Name = "verToolStripMenuItem";
            this.verToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.verToolStripMenuItem.Text = "Ver";
            // 
            // iconosToolStripMenuItem
            // 
            this.iconosToolStripMenuItem.Name = "iconosToolStripMenuItem";
            this.iconosToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.iconosToolStripMenuItem.Text = "Iconos";
            this.iconosToolStripMenuItem.Click += new System.EventHandler(this.iconosToolStripMenuItem_Click);
            // 
            // listaToolStripMenuItem
            // 
            this.listaToolStripMenuItem.Name = "listaToolStripMenuItem";
            this.listaToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.listaToolStripMenuItem.Text = "Lista";
            this.listaToolStripMenuItem.Click += new System.EventHandler(this.listaToolStripMenuItem_Click);
            // 
            // detallesToolStripMenuItem
            // 
            this.detallesToolStripMenuItem.Checked = true;
            this.detallesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.detallesToolStripMenuItem.Name = "detallesToolStripMenuItem";
            this.detallesToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.detallesToolStripMenuItem.Text = "Detalles";
            this.detallesToolStripMenuItem.Click += new System.EventHandler(this.detallesToolStripMenuItem_Click);
            // 
            // actualizarToolStripMenuItem
            // 
            this.actualizarToolStripMenuItem.Name = "actualizarToolStripMenuItem";
            this.actualizarToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.actualizarToolStripMenuItem.Text = "Actualizar";
            this.actualizarToolStripMenuItem.Click += new System.EventHandler(this.actualizarToolStripMenuItem_Click);
            // 
            // nuevaCarpetaToolStripMenuItem
            // 
            this.nuevaCarpetaToolStripMenuItem.Name = "nuevaCarpetaToolStripMenuItem";
            this.nuevaCarpetaToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.nuevaCarpetaToolStripMenuItem.Text = "Nueva Carpeta";
            this.nuevaCarpetaToolStripMenuItem.Click += new System.EventHandler(this.nuevaCarpetaToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(151, 6);
            // 
            // pegarToolStripMenuItem
            // 
            this.pegarToolStripMenuItem.Name = "pegarToolStripMenuItem";
            this.pegarToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.pegarToolStripMenuItem.Text = "Pegar";
            this.pegarToolStripMenuItem.Click += new System.EventHandler(this.pegarToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(151, 6);
            // 
            // propiedadesToolStripMenuItem
            // 
            this.propiedadesToolStripMenuItem.Name = "propiedadesToolStripMenuItem";
            this.propiedadesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.propiedadesToolStripMenuItem.Text = "Propiedades";
            this.propiedadesToolStripMenuItem.Click += new System.EventHandler(this.propiedadesToolStripMenuItem_Click);
            // 
            // itemContextMenu
            // 
            this.itemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirToolStripMenuItem,
            this.toolStripSeparator3,
            this.cortarToolStripMenuItem,
            this.copiarToolStripMenuItem,
            this.toolStripSeparator4,
            this.eliminarToolStripMenuItem,
            this.cambiarNombreToolStripMenuItem,
            this.toolStripSeparator5,
            this.propiedadesToolStripMenuItem1});
            this.itemContextMenu.Name = "itemContextMenu";
            this.itemContextMenu.Size = new System.Drawing.Size(163, 154);
            // 
            // abrirToolStripMenuItem
            // 
            this.abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            this.abrirToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.abrirToolStripMenuItem.Text = "Abrir";
            this.abrirToolStripMenuItem.Click += new System.EventHandler(this.abrirToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(159, 6);
            // 
            // cortarToolStripMenuItem
            // 
            this.cortarToolStripMenuItem.Name = "cortarToolStripMenuItem";
            this.cortarToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.cortarToolStripMenuItem.Text = "Cortar";
            this.cortarToolStripMenuItem.Click += new System.EventHandler(this.cortarToolStripMenuItem_Click);
            // 
            // copiarToolStripMenuItem
            // 
            this.copiarToolStripMenuItem.Name = "copiarToolStripMenuItem";
            this.copiarToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.copiarToolStripMenuItem.Text = "Copiar";
            this.copiarToolStripMenuItem.Click += new System.EventHandler(this.copiarToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(159, 6);
            // 
            // eliminarToolStripMenuItem
            // 
            this.eliminarToolStripMenuItem.Name = "eliminarToolStripMenuItem";
            this.eliminarToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.eliminarToolStripMenuItem.Text = "Eliminar";
            this.eliminarToolStripMenuItem.Click += new System.EventHandler(this.eliminarToolStripMenuItem_Click);
            // 
            // cambiarNombreToolStripMenuItem
            // 
            this.cambiarNombreToolStripMenuItem.Name = "cambiarNombreToolStripMenuItem";
            this.cambiarNombreToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.cambiarNombreToolStripMenuItem.Text = "Cambiar nombre";
            this.cambiarNombreToolStripMenuItem.Click += new System.EventHandler(this.cambiarNombreToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(159, 6);
            // 
            // propiedadesToolStripMenuItem1
            // 
            this.propiedadesToolStripMenuItem1.Name = "propiedadesToolStripMenuItem1";
            this.propiedadesToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.propiedadesToolStripMenuItem1.Text = "Propiedades";
            this.propiedadesToolStripMenuItem1.Click += new System.EventHandler(this.propiedadesToolStripMenuItem1_Click);
            // 
            // diskContextMenu
            // 
            this.diskContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formatearToolStripMenuItem,
            this.toolStripSeparator9,
            this.propiedadesToolStripMenuItem2});
            this.diskContextMenu.Name = "diskContextMenu";
            this.diskContextMenu.Size = new System.Drawing.Size(141, 54);
            // 
            // formatearToolStripMenuItem
            // 
            this.formatearToolStripMenuItem.Name = "formatearToolStripMenuItem";
            this.formatearToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.formatearToolStripMenuItem.Text = "Formatear";
            this.formatearToolStripMenuItem.Click += new System.EventHandler(this.formatearToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(137, 6);
            // 
            // propiedadesToolStripMenuItem2
            // 
            this.propiedadesToolStripMenuItem2.Name = "propiedadesToolStripMenuItem2";
            this.propiedadesToolStripMenuItem2.Size = new System.Drawing.Size(140, 22);
            this.propiedadesToolStripMenuItem2.Text = "Propiedades";
            this.propiedadesToolStripMenuItem2.Click += new System.EventHandler(this.propiedadesToolStripMenuItem2_Click);
            // 
            // virtualTreeView1
            // 
            this.virtualTreeView1.AllowDrop = true;
            this.virtualTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.virtualTreeView1.HideSelection = false;
            this.virtualTreeView1.HotTracking = true;
            this.virtualTreeView1.Location = new System.Drawing.Point(0, 0);
            this.virtualTreeView1.Name = "virtualTreeView1";
            this.virtualTreeView1.ShowLines = false;
            this.virtualTreeView1.ShowRootLines = false;
            this.virtualTreeView1.Size = new System.Drawing.Size(243, 265);
            this.virtualTreeView1.TabIndex = 0;
            // 
            // virtualListView1
            // 
            this.virtualListView1.AllowDrop = true;
            this.virtualListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.virtualListView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.virtualListView1.HideSelection = false;
            this.virtualListView1.LabelEdit = true;
            this.virtualListView1.Location = new System.Drawing.Point(0, 0);
            this.virtualListView1.Name = "virtualListView1";
            this.virtualListView1.ShowItemToolTips = true;
            this.virtualListView1.Size = new System.Drawing.Size(484, 265);
            this.virtualListView1.TabIndex = 0;
            this.virtualListView1.UseCompatibleStateImageBehavior = false;
            this.virtualListView1.View = System.Windows.Forms.View.Details;
            // 
            // VirtualExplorer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.addressToolStrip);
            this.Controls.Add(this.buttonsToolStrip);
            this.Name = "VirtualExplorer";
            this.Size = new System.Drawing.Size(729, 325);
            this.buttonsToolStrip.ResumeLayout(false);
            this.buttonsToolStrip.PerformLayout();
            this.addressToolStrip.ResumeLayout(false);
            this.addressToolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.lvContextMenuStrip.ResumeLayout(false);
            this.itemContextMenu.ResumeLayout(false);
            this.diskContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip buttonsToolStrip;
        private System.Windows.Forms.ToolStripSplitButton navBackButton;
        private System.Windows.Forms.ToolStrip addressToolStrip;
        private System.Windows.Forms.ToolStripSplitButton navFordwardButton;
        private System.Windows.Forms.ToolStripButton navUpButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton foldersButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripLabel addressLabel;
        private System.Windows.Forms.ToolStripLabel imageLabel;
        private System.Windows.Forms.ToolStripTextBox addressTextBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripDropDownButton viewsButton;
        private System.Windows.Forms.ToolStripMenuItem viewLargeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDetailsMenuItem;
        private VirtualDrive.Controls.VirtualListView virtualListView1;
        private VirtualDrive.Controls.VirtualTreeView virtualTreeView1;
        private System.Windows.Forms.ContextMenuStrip lvContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iconosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detallesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actualizarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nuevaCarpetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem propiedadesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip itemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cortarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copiarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem eliminarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cambiarNombreToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem propiedadesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem pegarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton copyButton;
        private System.Windows.Forms.ToolStripButton cutButton;
        private System.Windows.Forms.ToolStripButton pasteButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton infoButton;
        private System.Windows.Forms.ContextMenuStrip diskContextMenu;
        private System.Windows.Forms.ToolStripMenuItem formatearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem propiedadesToolStripMenuItem2;
    }
}
