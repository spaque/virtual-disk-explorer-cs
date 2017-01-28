using System;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell
{
    /// <summary>
    /// Contains every function, enumeration, struct and constant from
    /// the Windows API required by FileExplorer
    /// </summary>
    public static partial class ShellAPI
    {
        public const int    MAX_PATH    = 260;
        public const uint   CMD_FIRST   = 1;
        public const uint   CMD_LAST    = 30000;

        public const int S_OK = 0;
        public const int NO_ERROR = 0;
        public const int S_FALSE = 1;

        public static int cbFileInfo = 
            Marshal.SizeOf(typeof(SHFILEINFO));
        public static int cbMenuItemInfo = 
            Marshal.SizeOf(typeof(MENUITEMINFO));
        public static int cbTpmParams = 
            Marshal.SizeOf(typeof(TPMPARAMS));
        public static int cbInvokeCommand = 
            Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));

        public const int ALT = 32;
        public const int CTRL = 8;
        public const int SHIFT = 4;

        #region Windows Hooks

        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14;
        public const int WM_LBUTTONUP = 0x0202;

        public const uint WM_DROPFILES = 0x0233;

        public const int WH_CALLWNDPROCRET = 12;

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Shell Clipboard Formats

        public const String CFSTR_SHELLIDLIST = "Shell IDList Array";

        public const String CFSTR_SHELLIDLISTOFFSET = "Shell Object Offsets";

        public const String CFSTR_NETRESOURCES = "Net Resource";

        public const String CFSTR_FILEDESCRIPTORA = "FileGroupDescriptor";

        public const String CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";

        public const String CFSTR_FILECONTENTS = "FileContents";

        public const String CFSTR_FILENAMEA = "FileName";

        public const String CFSTR_FILENAMEW = "FileNameW";

        public const String CFSTR_PRINTERGROUP = "PrinterFreindlyName";

        public const String CFSTR_FILENAMEMAPA = "FileNameMap";

        public const String CFSTR_FILENAMEMAPW = "FileNameMapW";

        public const String CFSTR_SHELLURL = "UniformResourceLocator";

        public const String CFSTR_INETURLA = CFSTR_SHELLURL;

        public const String CFSTR_INETURLW = "UniformResourceLocatorW";

        public const String CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";

        public const String CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";

        public const String CFSTR_PASTESUCCEEDED = "Paste Succeeded";

        public const String CFSTR_INDRAGLOOP = "InShellDragLoop";

        public const String CFSTR_DRAGCONTEXT = "DragContext";

        public const String CFSTR_MOUNTEDVOLUME = "MountedVolume";

        public const String CFSTR_PERSISTEDDATAOBJECT = "PersistedDataObject";

        public const String CFSTR_TARGETCLSID = "TargetCLSID";

        public const String CFSTR_LOGICALPERFORMEDDROPEFFECT = "Logical Performed DropEffect";

        public const String CFSTR_AUTOPLAY_SHELLIDLISTS = "Autoplay Enumerated IDList Array";

        #endregion
    }
}
