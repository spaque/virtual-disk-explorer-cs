using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualDrive.Controls
{
    public enum CMDTYPE
    {
        NAV_UP,
        NAV_DOWN,
        NAV_MOD
    }

    public class CommandEventArgs : EventArgs
    {
        private CMDTYPE cmdType;
        private String text;

        public CommandEventArgs(CMDTYPE type, String _text)
        {
            cmdType = type;
            text = _text;
        }

        public CMDTYPE CmdType { get { return cmdType; } }

        public String Text { get { return text; } }
    }
}
