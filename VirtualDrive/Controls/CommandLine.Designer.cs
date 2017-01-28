namespace VirtualDrive.Controls
{
    partial class CommandLine
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
            this.cmdOutput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.command = new System.Windows.Forms.TextBox();
            this.commandWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // cmdOutput
            // 
            this.cmdOutput.AcceptsReturn = true;
            this.cmdOutput.AcceptsTab = true;
            this.cmdOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOutput.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOutput.Location = new System.Drawing.Point(12, 32);
            this.cmdOutput.Multiline = true;
            this.cmdOutput.Name = "cmdOutput";
            this.cmdOutput.ReadOnly = true;
            this.cmdOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cmdOutput.Size = new System.Drawing.Size(512, 284);
            this.cmdOutput.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Comando";
            // 
            // command
            // 
            this.command.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.command.Location = new System.Drawing.Point(70, 6);
            this.command.Name = "command";
            this.command.Size = new System.Drawing.Size(454, 20);
            this.command.TabIndex = 0;
            // 
            // commandWorker
            // 
            this.commandWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.commandWorker_DoWork);
            this.commandWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.commandWorker_RunWorkerCompleted);
            // 
            // CommandLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 328);
            this.Controls.Add(this.command);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdOutput);
            this.Name = "CommandLine";
            this.ShowIcon = false;
            this.Text = "Línea de Comandos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cmdOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox command;
        private System.ComponentModel.BackgroundWorker commandWorker;
    }
}