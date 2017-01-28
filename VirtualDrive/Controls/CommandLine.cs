using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VirtualDrive.FileSystem.FAT32;
using VirtualDrive.Shell;

namespace VirtualDrive.Controls
{
    public delegate void NavCommandEnteredEventHandler(object sender, CommandEventArgs e);

    public partial class CommandLine : Form
    {
        #region Fields

        private Disk disk;
        private VirtualItem currentItem;
        private List<String> cmdHistory;
        private int cmdHistoryIndex;

        #endregion

        public event NavCommandEnteredEventHandler NavCommandEntered;

        #region Constructor

        public CommandLine(Disk dsk)
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            disk = dsk;
            cmdHistory = new List<String>();
            command.KeyDown += new KeyEventHandler(command_KeyDown);
            cmdOutput.MaxLength = 100;
        }

        #endregion

        #region Properties

        public VirtualItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                Text = "Línea de Comandos - " + currentItem.Path;
            }
        }

        #endregion

        #region Private Methods

        private void CommandEntered()
        {
            String cmd = command.Text;
            cmdHistory.Add(cmd);
            cmd = cmd.Trim();
            cmdHistoryIndex = cmdHistory.Count;
            command.Clear();
            if (Regex.IsMatch(cmd, @"^\s+$"))
                return;
            if (String.Compare(cmd, "exit", true) == 0)
            {
                this.Close();
                return;
            }
            if (String.Compare(cmd, "cd ..", true) == 0)
            {
                NavCommandEntered(this, new CommandEventArgs(CMDTYPE.NAV_UP, ""));
                return;
            }
            if (cmd.StartsWith("cd "))
            {
                NavCommandEntered(this, new CommandEventArgs(CMDTYPE.NAV_DOWN, cmd.Substring(3)));
                return;
            }
            if (String.Compare(cmd, "cls", true) == 0)
            {
                cmdOutput.Clear();
                return;
            }
            commandWorker.RunWorkerAsync(cmd);
        }

        #endregion

        #region Events

        void command_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && command.Text.Length > 0)
            {
                CommandEntered();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (cmdHistoryIndex == 0)
                {
                    command.Clear();
                    cmdHistoryIndex--;
                }
                else if (cmdHistoryIndex > 0)
                    command.Text = cmdHistory[--cmdHistoryIndex];
                else command.Clear();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (cmdHistoryIndex == cmdHistory.Count - 1)
                {
                    command.Clear();
                    cmdHistoryIndex++;
                }
                else if (cmdHistoryIndex < cmdHistory.Count - 1)
                    command.Text = cmdHistory[++cmdHistoryIndex];
                else command.Clear();
                e.Handled = true;
            }
        }

        private void commandWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String cmd = (String)e.Argument;

            String output = disk.RunCommand(cmd);
            StringBuilder sb = new StringBuilder();
            sb.Append(currentItem.Path);
            sb.Append("> ");
            sb.Append(cmd);
            sb.AppendLine();
            if (output.Length > 0)
                sb.Append(output);
            else
                sb.Append("Comando incorrecto");
            sb.AppendLine();
            sb.AppendLine();
            e.Result = new String[] { cmd, sb.ToString() };
        }

        private void commandWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String[] result = (String[])e.Result;
            String cmd = result[0];
            String output = result[1];
            cmdOutput.AppendText(output);
            cmdOutput.SelectionStart = cmdOutput.Text.Length;
            cmdOutput.ScrollToCaret();
            if (cmd.StartsWith("mkdir ", StringComparison.OrdinalIgnoreCase) ||
                 cmd.StartsWith("rmdir ", StringComparison.OrdinalIgnoreCase) ||
                 cmd.StartsWith("del ", StringComparison.OrdinalIgnoreCase) ||
                 cmd.StartsWith("copy ", StringComparison.OrdinalIgnoreCase) ||
                 cmd.StartsWith("move ", StringComparison.OrdinalIgnoreCase) ||
                 cmd.StartsWith("rename ", StringComparison.OrdinalIgnoreCase))
                NavCommandEntered(this, new CommandEventArgs(CMDTYPE.NAV_MOD, ""));
        }

        #endregion
    }
}