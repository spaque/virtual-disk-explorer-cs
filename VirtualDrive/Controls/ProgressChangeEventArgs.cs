using System;
using System.Text;
using System.Windows.Forms;

namespace VirtualDrive.Controls
{
    public class OpProgressChangedEventArgs : EventArgs
    {
        int progress;

        public OpProgressChangedEventArgs() { }

        public OpProgressChangedEventArgs(int progress)
        {
            this.progress = progress;
        }

        public int Progress
        {
            get { return progress; }
            set { progress = value; }
        }
    }
}
