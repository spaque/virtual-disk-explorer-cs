using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FileExplorer.Shell
{
    /// <summary>
    /// Contains every function, enumeration, struct and constant from
    /// the Windows API required by FileExplorer
    /// </summary>
    public static partial class ShellAPI
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;
            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr pFrom;
            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }

        /// <summary>
        /// Contains strings returned from the IShellFolder interface methods.
        /// (ShTypes.h)
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct STRRET
        {
            [FieldOffset(0)]
            public uint     uType;
            [FieldOffset(4)]            // union -> same FieldOffset
            public IntPtr   pOleStr;
            [FieldOffset(4)]
            public IntPtr   pStr;
            [FieldOffset(4)]
            public uint     uOffset;
            [FieldOffset(4)]
            public IntPtr   cStr;
        }

        /// <summary>
        /// Contains information about a file object.
        /// (ShellAPI.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr   hIcon;
            public int      iIcon;
            public SFGAO    dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public String   szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public String   szTypeName;
        }

        /// <summary>
        /// Contains extended information about a shortcut menu command.
        /// (ShlObj.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CMINVOKECOMMANDINFOEX
        {
            public int      cbSize;
            public CMIC     fMask;
            public IntPtr   hwnd;
            public IntPtr   lpVerb;
            [MarshalAs(UnmanagedType.LPStr)]
            public String   lpParameters;
            [MarshalAs(UnmanagedType.LPStr)]
            public String   lpDirectory;
            public SW       nShow;
            public int      dwHotKey;
            public IntPtr   hIcon;
            [MarshalAs(UnmanagedType.LPStr)]
            public String   lpTitle;
            public IntPtr   lpVerbW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String   lpParametersW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String   lpDirectoryW;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String   lpTitleW;
            public POINT    ptInvoke;
        }

        /// <summary>
        /// The MENUITEMINFO structure contains information about a menu item.
        /// (WinUser.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MENUITEMINFO
        {
            public MENUITEMINFO(String text)
            {
                cbSize      = cbMenuItemInfo;
                dwTypeData  = text;
                cch         = text.Length;
                fMask       = 0;
                fType       = 0;
                fState      = 0;
                wID         = 0;
                hSubMenu        = IntPtr.Zero;
                hbmpChecked     = IntPtr.Zero;
                hbmpUnchecked   = IntPtr.Zero;
                dwItemData      = IntPtr.Zero;
                hbmpItem        = IntPtr.Zero;
            }

            public int      cbSize;
            public MIIM     fMask;
            public MFT      fType;
            public MFS      fState;
            public uint     wID;
            public IntPtr   hSubMenu;
            public IntPtr   hbmpChecked;
            public IntPtr   hbmpUnchecked;
            public IntPtr   dwItemData;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String   dwTypeData;
            public int      cch;
            public IntPtr   hbmpItem;
        }

        /// <summary>
        /// The TPMPARAMS structure contains extended parameters 
        /// for the TrackPopupMenuEx function.
        /// (WinUser.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TPMPARAMS
        {
            int  cbSize;
            RECT rcExclude;
        }

        /// <summary>
        /// Contains and receives information for change notifications. 
        /// This structure is used with the SHChangeNotifyRegister 
        /// function and the SFVM_QUERYFSNOTIFY notification.
        /// (ShlObj.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHChangeNotifyEntry
        {
            public IntPtr pIdl;
            public bool   Recursively;
        }

        /// <summary>
        /// Contains two PIDLs concerning the notify message.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHNOTIFYSTRUCT
        {
            public IntPtr dwItem1;
            public IntPtr dwItem2;
        }

        /// <summary>
        /// The POINT structure defines the x and y coordinates of a point.
        /// (WinDef.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;
        }

        /// <summary>
        /// The RECT structure defines the coordinates of the 
        /// upper-left and lower-right corners of a rectangle.
        /// (WinDef.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public RECT(Rectangle rect)
            {
                left    = rect.Left;
                top     = rect.Top;
                right   = rect.Right;
                bottom  = rect.Bottom;
            }

            int left;
            int top;
            int right;
            int bottom;
        }

        /// <summary>
        /// The SIZE structure specifies the width and height of a rectangle.
        /// (WinDef.h)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        /// <summary>
        /// Specifies the FMTID/PID identifier of a column that will be 
        /// displayed by the Microsoft Windows Explorer Details view.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHCOLUMNID
        {
            Guid fmtid;
            uint pid;
        }

        /// <summary>
        /// Detailed information on an item in a Shell folder.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLDETAILS
        {
            int fmt;
            int cxChar;
            STRRET str;
        }

        /// <summary>
        /// Contains information used by ShellExecuteEx.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public ShellAPI.SEE_MASK fMask;
            public int hwnd;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpParameters;
            [MarshalAs(UnmanagedType.LPWStr)]
            public String lpDirectory;
            public ShellAPI.SW nShow;
            public int hInstApp;
            public IntPtr lpIDList;
            public String lpClass;
            public int hkeyClass;
            public int dwHotKey;
            public int hIcon;
            public int hProcess;
        }
    }
}
