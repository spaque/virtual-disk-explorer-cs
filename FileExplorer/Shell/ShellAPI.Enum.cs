using System;

namespace FileExplorer.Shell
{
    /// <summary>
    /// Contains every function, enumeration, struct and constant from
    /// the Windows API required by FileExplorer
    /// </summary>
    public static partial class ShellAPI
    {
        /// <summary>
        /// CSIDL values provide a unique system-independent way to 
        /// identify special folders used frequently by applications.
        /// (ShlObj.h)
        /// </summary>
        public enum CSIDL
        {
            FLAG_CREATE             = 0x8000,
            ADMINTOOLS              = 0x0030,
            ALTSTARTUP              = 0x001D,
            APPDATA                 = 0x001A,
            BITBUCKET               = 0x000A,
            CDBURN_AREA             = 0x003B,
            COMMON_ADMINTOOLS       = 0x002F,
            COMMON_ALTSTARTUP       = 0x001E,
            COMMON_APPDATA          = 0x0023,
            COMMON_DESKTOPDIRECTORY = 0x0019,
            COMMON_DOCUMENTS        = 0x002E,
            COMMON_FAVORITES        = 0x001F,
            COMMON_MUSIC            = 0x0035,
            COMMON_PICTURES         = 0x0036,
            COMMON_PROGRAMS         = 0x0017,
            COMMON_STARTMENU        = 0x0016,
            COMMON_STARTUP          = 0x0018,
            COMMON_TEMPLATES        = 0x002D,
            COMMON_VIDEO            = 0x0037,
            CONTROLS                = 0x0003,
            COOKIES                 = 0x0021,
            DESKTOP                 = 0x0000,
            DESKTOPDIRECTORY        = 0x0010,
            DRIVES                  = 0x0011,
            FAVORITES               = 0x0006,
            FONTS                   = 0x0014,
            HISTORY                 = 0x0022,
            INTERNET                = 0x0001,
            INTERNET_CACHE          = 0x0020,
            LOCAL_APPDATA           = 0x001C,
            MYDOCUMENTS             = 0x000C,
            MYMUSIC                 = 0x000D,
            MYPICTURES              = 0x0027,
            MYVIDEO                 = 0x000E,
            NETHOOD                 = 0x0013,
            PERSONAL                = 0x0005,
            PRINTERS                = 0x0004,
            PRINTHOOD               = 0x001B,
            PROFILE                 = 0x0028,
            PROFILES                = 0x003E,
            PROGRAM_FILES           = 0x0026,
            PROGRAM_FILES_COMMON    = 0x002B,
            PROGRAMS                = 0x0002,
            RECENT                  = 0x0008,
            SENDTO                  = 0x0009,
            STARTMENU               = 0x000B,
            STARTUP                 = 0x0007,
            SYSTEM                  = 0x0025,
            TEMPLATES               = 0x0015,
            WINDOWS                 = 0x0024
        }

        /// <summary>
        /// Defines the values used with the IShellFolder::GetDisplayNameOf 
        /// and IShellFolder::SetNameOf methods to specify the type of file or 
        /// folder names used by those methods (uFlags parameter).
        /// (ShObjIdl.h)
        /// </summary>
        [Flags]
        public enum SHGNO
        {
            NORMAL          = 0x0000,
            INFOLDER        = 0x0001,
            FOREDITING      = 0x1000,
            FORADDRESSBAR   = 0x4000,
            FORPARSING      = 0x8000
        }

        /// <summary>
        /// Flags to specify which path is to be returned 
        /// with SHGetFolderPath (dwFlags parameter).
        /// (ShlObj.h)
        /// </summary>
        [Flags]
        public enum SHGFP
        {
            TYPE_CURRENT = 0x00, // current value for user, verify it exists
            TYPE_DEFAULT = 0x01  // default value, may not exist
        }

        /// <summary>
        /// Indicate flags used by IShellFolder::GetAttributesOf to 
        /// specify the requested attributes (rgfInOut parameter).
        /// (ShObjIdl.h)
        /// </summary>
        [Flags]
        public enum SFGAO : uint
        {
            NONE            = 0x00000000,
            BROWSABLE       = 0x08000000,
            CANCOPY         = 0x00000001,
            CANDELETE       = 0x00000020,
            CANLINK         = 0x00000004,
            CANMONIKER      = 0x00400000,
            CANMOVE         = 0x00000002,
            CANRENAME       = 0x00000010,
            CAPABILITYMASK  = 0x00000177,
            COMPRESSED      = 0x04000000,
            CONTENTSMASK    = 0x80000000,
            DISPLAYATTRMASK = 0x000FC000,
            DROPTARGET      = 0x00000100,
            ENCRYPTED       = 0x00002000,
            FILESYSANCESTOR = 0x10000000,
            FILESYSTEM      = 0x40000000,
            FOLDER          = 0x20000000,
            GHOSTED         = 0x00008000,
            HASPROPSHEET    = 0x00000040,
            HASSTORAGE      = 0x00400000,
            HASSUBFOLDER    = 0x80000000,
            HIDDEN          = 0x00080000,
            ISSLOW          = 0x00004000,
            LINK            = 0x00010000,
            NEWCONTENT      = 0x00200000,
            NONENUMERATED   = 0x00100000,
            READONLY        = 0x00040000,
            REMOVABLE       = 0x02000000,
            SHARE           = 0x00020000,
            STORAGE         = 0x00000008,
            STORAGEANCESTOR = 0x00800000,
            STORAGECAPMASK  = 0x70C50008,
            STREAM          = 0x00400000,
            VALIDATE        = 0x01000000
        }

        /// <summary>
        /// Determines the type of items included in an enumeration. 
        /// These values are used with the IShellFolder::EnumObjects 
        /// method (grfFlags parameter).
        /// (ShObjIdl.h)
        /// </summary>
        [Flags]
        public enum SHCONTF
        {
            NONE                = 0x0000,
            FOLDERS             = 0x0020,
            NONFOLDERS          = 0x0040,
            INCLUDEHIDDEN       = 0x0080,
            INIT_ON_FIRST_NEXT  = 0x0100,
            NETPRINTERSRCH      = 0x0200,
            SHAREABLE           = 0x0400,
            STORAGE             = 0x0800,
        }

        /// <summary>
        /// Specifies how the shortcut menu can be changed when calling
        /// IContextMenu::QueryContextMenu (uFlags parameter).
        /// (ShlObj.h)
        /// </summary>
        [Flags]
        public enum CMF : uint
        {
            NORMAL          = 0x00000000,
            DEFAULTONLY     = 0x00000001,
            VERBSONLY       = 0x00000002,
            EXPLORE         = 0x00000004,
            NOVERBS         = 0x00000008,
            CANRENAME       = 0x00000010,
            NODEFAULT       = 0x00000020,
            INCLUDESTATIC   = 0x00000040,
            EXTENDEDVERBS   = 0x00000100,
            RESERVED        = 0xffff0000
        }

        /// <summary>
        /// Flags specifying the information to return when calling
        /// IContextMenu::GetCommandString (uFlags parameter).
        /// (ShlObj.h)
        /// </summary>
        [Flags]
        public enum GCS : uint
        {
            VERBA       = 0x00000000, // canonical verb
            HELPTEXTA   = 0x00000001, // help text (for status bar)
            VALIDATEA   = 0x00000002, // validate command exists
            VERBW       = 0x00000004, // canonical verb (unicode)
            HELPTEXTW   = 0x00000005, // help text (unicode)
            VALIDATEW   = 0x00000006  // validate command exists (unicode)
        }

        /// <summary>
        /// Flags that specify the file information to
        /// retrieve with SHGetFileInfo (uFlags parameter).
        /// (ShellAPI.h)
        /// </summary>
        [Flags]
        public enum SHGFI : uint
        {
            NONE                = 0x00000000,
            ADDOVERLAYS         = 0x00000020,
            ATTR_SPECIFIED      = 0x00020000,
            ATTRIBUTES          = 0x00000800,
            DISPLAYNAME         = 0x00000200,
            EXETYPE             = 0x00002000,
            ICON                = 0x00000100,
            ICONLOCATION        = 0x00001000,
            LARGEICON           = 0x00000000,
            LINKOVERLAY         = 0x00008000,
            OPENICON            = 0x00000002,
            OVERLAYINDEX        = 0x00000040,
            PIDL                = 0x00000008,
            SELECTED            = 0x00010000,
            SHELLICONSIZE       = 0x00000004,
            SMALLICON           = 0x00000001,
            SYSICONINDEX        = 0x00004000,
            TYPENAME            = 0x00000400,
            USEFILEATTRIBUTES   = 0x00000010
        }

        /// <summary>
        /// Flags that specify the file information to retrieve
        /// with SHGetFileInfo (dwFileAttributes parameter).
        /// (WinNT.h)
        /// </summary>
        [Flags]
        public enum FILE_ATTRIBUTE : uint
        {
            NONE                = 0x00000000,
            READONLY            = 0x00000001,
            HIDDEN              = 0x00000002,
            SYSTEM              = 0x00000004,
            DIRECTORY           = 0x00000010,
            ARCHIVE             = 0x00000020,
            DEVICE              = 0x00000040,
            NORMAL              = 0x00000080,
            TEMPORARY           = 0x00000100,
            SPARSE_FILE         = 0x00000200,
            REPARSE_POINT       = 0x00000400,
            COMPRESSED          = 0x00000800,
            OFFLINE             = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED           = 0x00004000
        }

        /// <summary>
        /// Specifies how TrackPopupMenuEx positions the
        /// shortcut menu horizontally (fuFlags parameter)
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum TPM : uint
        {
            LEFTBUTTON      = 0x0000,
            RIGHTBUTTON     = 0x0002,
            LEFTALIGN       = 0x0000,
            CENTERALIGN     = 0x0004,
            RIGHTALIGN      = 0x0008,
            TOPALIGN        = 0x0000,
            VCENTERALIGN    = 0x0010,
            BOTTOMALIGN     = 0x0020,
            HORIZONTAL      = 0x0000,
            VERTICAL        = 0x0040,
            NONOTIFY        = 0x0080,
            RETURNCMD       = 0x0100,
            RECURSE         = 0x0001,
            HORPOSANIMATION = 0x0400,
            HORNEGANIMATION = 0x0800,
            VERPOSANIMATION = 0x1000,
            VERNEGANIMATION = 0x2000,
            NOANIMATION     = 0x4000,
            LAYOUTRTL       = 0x8000
        }

        /// <summary>
        /// Flags used with the CMINVOKECOMMANDINFOEX structure (fMask field)
        /// (ShlObj.h - ShellAPI.h)
        /// </summary>
        [Flags]
        public enum CMIC : uint
        {
            NONE            = 0x00000000,
            HOTKEY          = 0x00000020,
            ICON            = 0x00000010,
            HASTITLE        = 0x00080000, // not defined in shellapi
            HASLINKNAME     = 0x00010000, // not defined in shellapi
            FLAG_NO_UI      = 0x00000400,
            NO_CONSOLE      = 0x00008000,
            FLAG_SEP_VDM    = 0x00020000, // not defined in shellapi
            PTINVOKE        = 0x20000000,
            SHIFT_DOWN      = 0x10000000,
            CONTROL_DOWN    = 0x40000000,
            UNICODE         = 0x00004000,
            ASYNCOK         = 0x00100000,
            NOZONECHECKS    = 0x00800000,
            FLAG_LOG_USAGE  = 0x04000000
        }

        /// <summary>
        /// Flags that specify the drawing style when 
        /// calling ImageList_GetIcon (flags parameter).
        /// (CommCtrl.h)
        /// </summary>
        [Flags]
        public enum ILD : uint
        {
            NORMAL          = 0x00000000,
            TRANSPARENT     = 0x00000001,
            MASK            = 0x00000010,
            IMAGE           = 0x00000020,
            ROP             = 0x00000040,
            BLEND25         = 0x00000002,
            BLEND50         = 0x00000004,
            OVERLAYMASK     = 0x00000F00,
            PRESERVEALPHA   = 0x00001000,
            SCALE           = 0x00002000,
            DPISCALE        = 0x00004000,
            SELECTED        = 0x00000004,
            FOCUS           = 0x00000002,
            BLEND           = 0x00000004
        }

        /// <summary>
        /// ShowWindow() Commands
        /// Specifies how the window is to be shown.
        /// (WinUser.h)
        /// </summary>
        public enum SW : int
        {
            HIDE            = 0,
            SHOWNORMAL      = 1,
            NORMAL          = 1,
            SHOWMINIMIZED   = 2,
            SHOWMAXIMIZED   = 3,
            MAXIMIZE        = 3,
            SHOWNOACTIVATE  = 4,
            SHOW            = 5,
            MINIMIZE        = 6,
            SHOWMINNOACTIVE = 7,
            SHOWNA          = 8,
            RESTORE         = 9,
            SHOWDEFAULT     = 10,
            FORCEMINIMIZE   = 11,
            MAX             = 11
        }

        /// <summary>
        /// Window message flags
        /// (WinUser.h)
        /// </summary>
        public enum WM : uint
        {
            ACTIVATE                = 0x0006,
            ACTIVATEAPP             = 0x001C,
            AFXFIRST                = 0x0360,
            AFXLAST                 = 0x037F,
            APP                     = 0x8000,
            ASKCBFORMATNAME         = 0x030C,
            CANCELJOURNAL           = 0x004B,
            CANCELMODE              = 0x001F,
            CAPTURECHANGED          = 0x0215,
            CHANGECBCHAIN           = 0x030D,
            CHAR                    = 0x0102,
            CHARTOITEM              = 0x002F,
            CHILDACTIVATE           = 0x0022,
            CLEAR                   = 0x0303,
            CLOSE                   = 0x0010,
            COMMAND                 = 0x0111,
            COMPACTING              = 0x0041,
            COMPAREITEM             = 0x0039,
            CONTEXTMENU             = 0x007B,
            COPY                    = 0x0301,
            COPYDATA                = 0x004A,
            CREATE                  = 0x0001,
            CTLCOLORBTN             = 0x0135,
            CTLCOLORDLG             = 0x0136,
            CTLCOLOREDIT            = 0x0133,
            CTLCOLORLISTBOX         = 0x0134,
            CTLCOLORMSGBOX          = 0x0132,
            CTLCOLORSCROLLBAR       = 0x0137,
            CTLCOLORSTATIC          = 0x0138,
            CUT                     = 0x0300,
            DEADCHAR                = 0x0103,
            DELETEITEM              = 0x002D,
            DESTROY                 = 0x0002,
            DESTROYCLIPBOARD        = 0x0307,
            DEVICECHANGE            = 0x0219,
            DEVMODECHANGE           = 0x001B,
            DISPLAYCHANGE           = 0x007E,
            DRAWCLIPBOARD           = 0x0308,
            DRAWITEM                = 0x002B,
            DROPFILES               = 0x0233,
            ENABLE                  = 0x000A,
            ENDSESSION              = 0x0016,
            ENTERIDLE               = 0x0121,
            ENTERMENULOOP           = 0x0211,
            ENTERSIZEMOVE           = 0x0231,
            ERASEBKGND              = 0x0014,
            EXITMENULOOP            = 0x0212,
            EXITSIZEMOVE            = 0x0232,
            FONTCHANGE              = 0x001D,
            GETDLGCODE              = 0x0087,
            GETFONT                 = 0x0031,
            GETHOTKEY               = 0x0033,
            GETICON                 = 0x007F,
            GETMINMAXINFO           = 0x0024,
            GETOBJECT               = 0x003D,
            GETSYSMENU              = 0x0313,
            GETTEXT                 = 0x000D,
            GETTEXTLENGTH           = 0x000E,
            HANDHELDFIRST           = 0x0358,
            HANDHELDLAST            = 0x035F,
            HELP                    = 0x0053,
            HOTKEY                  = 0x0312,
            HSCROLL                 = 0x0114,
            HSCROLLCLIPBOARD        = 0x030E,
            ICONERASEBKGND          = 0x0027,
            IME_CHAR                = 0x0286,
            IME_COMPOSITION         = 0x010F,
            IME_COMPOSITIONFULL     = 0x0284,
            IME_CONTROL             = 0x0283,
            IME_ENDCOMPOSITION      = 0x010E,
            IME_KEYDOWN             = 0x0290,
            IME_KEYLAST             = 0x010F,
            IME_KEYUP               = 0x0291,
            IME_NOTIFY              = 0x0282,
            IME_REQUEST             = 0x0288,
            IME_SELECT              = 0x0285,
            IME_SETCONTEXT          = 0x0281,
            IME_STARTCOMPOSITION    = 0x010D,
            INITDIALOG              = 0x0110,
            INITMENU                = 0x0116,
            INITMENUPOPUP           = 0x0117,
            INPUTLANGCHANGE         = 0x0051,
            INPUTLANGCHANGEREQUEST  = 0x0050,
            KEYDOWN                 = 0x0100,
            KEYFIRST                = 0x0100,
            KEYLAST                 = 0x0108,
            KEYUP                   = 0x0101,
            KILLFOCUS               = 0x0008,
            LBUTTONDBLCLK           = 0x0203,
            LBUTTONDOWN             = 0x0201,
            LBUTTONUP               = 0x0202,
            LVM_GETEDITCONTROL      = 0x1018,
            LVM_SETIMAGELIST        = 0x1003,
            MBUTTONDBLCLK           = 0x0209,
            MBUTTONDOWN             = 0x0207,
            MBUTTONUP               = 0x0208,
            MDIACTIVATE             = 0x0222,
            MDICASCADE              = 0x0227,
            MDICREATE               = 0x0220,
            MDIDESTROY              = 0x0221,
            MDIGETACTIVE            = 0x0229,
            MDIICONARRANGE          = 0x0228,
            MDIMAXIMIZE             = 0x0225,
            MDINEXT                 = 0x0224,
            MDIREFRESHMENU          = 0x0234,
            MDIRESTORE              = 0x0223,
            MDISETMENU              = 0x0230,
            MDITILE                 = 0x0226,
            MEASUREITEM             = 0x002C,
            MENUCHAR                = 0x0120,
            MENUCOMMAND             = 0x0126,
            MENUDRAG                = 0x0123,
            MENUGETOBJECT           = 0x0124,
            MENURBUTTONUP           = 0x0122,
            MENUSELECT              = 0x011F,
            MOUSEACTIVATE           = 0x0021,
            MOUSEFIRST              = 0x0200,
            MOUSEHOVER              = 0x02A1,
            MOUSELAST               = 0x020A,
            MOUSELEAVE              = 0x02A3,
            MOUSEMOVE               = 0x0200,
            MOUSEWHEEL              = 0x020A,
            MOVE                    = 0x0003,
            MOVING                  = 0x0216,
            NCACTIVATE              = 0x0086,
            NCCALCSIZE              = 0x0083,
            NCCREATE                = 0x0081,
            NCDESTROY               = 0x0082,
            NCHITTEST               = 0x0084,
            NCLBUTTONDBLCLK         = 0x00A3,
            NCLBUTTONDOWN           = 0x00A1,
            NCLBUTTONUP             = 0x00A2,
            NCMBUTTONDBLCLK         = 0x00A9,
            NCMBUTTONDOWN           = 0x00A7,
            NCMBUTTONUP             = 0x00A8,
            NCMOUSEHOVER            = 0x02A0,
            NCMOUSELEAVE            = 0x02A2,
            NCMOUSEMOVE             = 0x00A0,
            NCPAINT                 = 0x0085,
            NCRBUTTONDBLCLK         = 0x00A6,
            NCRBUTTONDOWN           = 0x00A4,
            NCRBUTTONUP             = 0x00A5,
            NEXTDLGCTL              = 0x0028,
            NEXTMENU                = 0x0213,
            NOTIFY                  = 0x004E,
            NOTIFYFORMAT            = 0x0055,
            NULL                    = 0x0000,
            PAINT                   = 0x000F,
            PAINTCLIPBOARD          = 0x0309,
            PAINTICON               = 0x0026,
            PALETTECHANGED          = 0x0311,
            PALETTEISCHANGING       = 0x0310,
            PARENTNOTIFY            = 0x0210,
            PASTE                   = 0x0302,
            PENWINFIRST             = 0x0380,
            PENWINLAST              = 0x038F,
            POWER                   = 0x0048,
            PRINT                   = 0x0317,
            PRINTCLIENT             = 0x0318,
            QUERYDRAGICON           = 0x0037,
            QUERYENDSESSION         = 0x0011,
            QUERYNEWPALETTE         = 0x030F,
            QUERYOPEN               = 0x0013,
            QUEUESYNC               = 0x0023,
            QUIT                    = 0x0012,
            RBUTTONDBLCLK           = 0x0206,
            RBUTTONDOWN             = 0x0204,
            RBUTTONUP               = 0x0205,
            RENDERALLFORMATS        = 0x0306,
            RENDERFORMAT            = 0x0305,
            SETCURSOR               = 0x0020,
            SETFOCUS                = 0x0007,
            SETFONT                 = 0x0030,
            SETHOTKEY               = 0x0032,
            SETICON                 = 0x0080,
            SETMARGINS              = 0x00D3,
            SETREDRAW               = 0x000B,
            SETTEXT                 = 0x000C,
            SETTINGCHANGE           = 0x001A,
            SHOWWINDOW              = 0x0018,
            SIZE                    = 0x0005,
            SIZECLIPBOARD           = 0x030B,
            SIZING                  = 0x0214,
            SPOOLERSTATUS           = 0x002A,
            STYLECHANGED            = 0x007D,
            STYLECHANGING           = 0x007C,
            SYNCPAINT               = 0x0088,
            SYSCHAR                 = 0x0106,
            SYSCOLORCHANGE          = 0x0015,
            SYSCOMMAND              = 0x0112,
            SYSDEADCHAR             = 0x0107,
            SYSKEYDOWN              = 0x0104,
            SYSKEYUP                = 0x0105,
            TCARD                   = 0x0052,
            TIMECHANGE              = 0x001E,
            TIMER                   = 0x0113,
            TVM_GETEDITCONTROL      = 0x110F,
            TVM_SETIMAGELIST        = 0x1109,
            UNDO                    = 0x0304,
            UNINITMENUPOPUP         = 0x0125,
            USER                    = 0x0400,
            USERCHANGED             = 0x0054,
            VKEYTOITEM              = 0x002E,
            VSCROLL                 = 0x0115,
            VSCROLLCLIPBOARD        = 0x030A,
            WINDOWPOSCHANGED        = 0x0047,
            WINDOWPOSCHANGING       = 0x0046,
            WININICHANGE            = 0x001A,
            SH_NOTIFY               = 0x0401
        }

        /// <summary>
        /// Specifies how GetMenuDefaultItem searches for menu items.
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum GMDI : uint
        {
            ZERO            = 0x0000,
            USEDISABLED     = 0x0001,
            GOINTOPOPUPS    = 0x0002
        }

        /// <summary>
        /// Menu flags for Add/Check/EnableMenuItem().
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum MF : uint
        {
            INSERT      = 0x00000000,
            CHANGE      = 0x00000080,
            APPEND      = 0x00000100,
            DELETE      = 0x00000200,
            REMOVE      = 0x00001000,
            BYCOMMAND   = 0x00000000,
            BYPOSITION  = 0x00000400
        }

        /// <summary>
        /// Menu flags for Add/Check/EnableMenuItem().
        /// Specifies the content type of the new menu item.
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum MFT : uint
        {
            BITMAP          = 0x00000004,
            MENUBARBREAK    = 0x00000020,
            MENUBREAK       = 0x00000040,
            OWNERDRAW       = 0x00000100,
            RADIOCHECK      = 0x00000200,
            RIGHTJUSTIFY    = 0x00004000,
            RIGHTORDER      = 0x00002000,
            SEPARATOR       = 0x00000800,
            STRING          = 0x00000000
        }

        /// <summary>
        /// Menu flags for Add/Check/EnableMenuItem().
        /// Specifies the state of the new menu item.
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum MFS : uint
        {
            GRAYED      = 0x00000003,
            DISABLED    = 0x00000003,
            CHECKED     = 0x00000008,
            HILITE      = 0x00000080,
            ENABLED     = 0x00000000,
            UNCHECKED   = 0x00000000,
            UNHILITE    = 0x00000000,
            DEFAULT     = 0x00001000
        }

        /// <summary>
        /// Specifies the content of the new menu item.
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum MIIM : uint
        {
            NONE        = 0x00000000,
            BITMAP      = 0x00000080,
            CHECKMARKS  = 0x00000008,
            DATA        = 0x00000020,
            FTYPE       = 0x00000100,
            ID          = 0x00000002,
            STATE       = 0x00000001,
            STRING      = 0x00000040,
            SUBMENU     = 0x00000004,
            TYPE        = 0x00000010
        }

        /// <summary>
        /// Standard clipboard formats.
        /// (WinUser.h)
        /// </summary>
        public enum CF
        {
            NONE            = 0,
            BITMAP          = 2,
            DIB             = 8,
            DIBV5           = 17,
            DIF             = 5,
            DSPBITMAP       = 0x0082,
            DSPENHMETAFILE  = 0x008e,
            DSPMETAFILEPICT = 0x0083,
            DSPTEXT         = 0x0081,
            ENHMETAFILE     = 14,
            GDIOBJFIRST     = 0x0300,
            GDIOBJLAST      = 0x03ff,
            HDROP           = 15,
            LOCALE          = 16,
            MAX             = 18,
            METAFILEPICT    = 3,
            OEMTEXT         = 7,
            OWNERDISPLAY    = 0x0080,
            PALETTE         = 9,
            PENDATA         = 10,
            PRIVATEFIRST    = 0x0200,
            PRIVATELAST     = 0x02ff,
            RIFF            = 11,
            SYLK            = 4,
            TEXT            = 1,
            TIFF            = 6,
            UNICODETEXT     = 13,
            WAVE            = 12
        }

        /// <summary>
        /// The ADVF enumeration values are flags used by a container object 
        /// to specify the requested behavior when setting up an advise sink 
        /// or a caching connection with an object. These values have different 
        /// meanings, depending on the type of connection in which they are 
        /// used, and each interface uses its own subset of the flags.
        /// </summary>
        [Flags]
        public enum ADVF
        {
            NONE                = 0x00,
            CACHE_FORCEBUILTIN  = 0x10,
            CACHE_NOHANDLER     = 0x08,
            CACHE_ONSAVE        = 0x20,
            DATAONSTOP          = 0x40,
            NODATA              = 0x01,
            ONLYONCE            = 0x04,
            PRIMEFIRST          = 0x02
        }

        /// <summary>
        /// Flags indicating which mouse buttons are clicked 
        /// and which modifier keys are pressed.
        /// (WinUser.h)
        /// </summary>
        [Flags]
        public enum MK : int
        {
            NONE    = 0x0000,
            LBUTTON = 0x0001,
            RBUTTON = 0x0002,
            SHIFT   = 0x0004,
            CONTROL = 0x0008,
            MBUTTON = 0x0010,
            ALT     = 0x0020
        }

        /// <summary>
        /// File System Notification flags.
        /// (ShlObj.h)
        /// </summary>
        [Flags]
        public enum SHCNE : uint
        {
            NONE                = 0x00000000,
            RENAMEITEM          = 0x00000001,
            CREATE              = 0x00000002,
            DELETE              = 0x00000004,
            MKDIR               = 0x00000008,
            RMDIR               = 0x00000010,
            MEDIAINSERTED       = 0x00000020,
            MEDIAREMOVED        = 0x00000040,
            DRIVEREMOVED        = 0x00000080,
            DRIVEADD            = 0x00000100,
            NETSHARE            = 0x00000200,
            NETUNSHARE          = 0x00000400,
            ATTRIBUTES          = 0x00000800,
            UPDATEDIR           = 0x00001000,
            UPDATEITEM          = 0x00002000,
            SERVERDISCONNECT    = 0x00004000,
            UPDATEIMAGE         = 0x00008000,
            DRIVEADDGUI         = 0x00010000,
            RENAMEFOLDER        = 0x00020000,
            FREESPACE           = 0x00040000,
            EXTENDED_EVENT      = 0x04000000,
            ASSOCCHANGED        = 0x08000000,
            DISKEVENTS          = 0x0002381F,
            GLOBALEVENTS        = 0x0C0581E0,
            ALLEVENTS           = 0x7FFFFFFF,
            INTERRUPT           = 0x80000000
        }

        /// <summary>
        /// Indicates the type of events for which to receive notifications.
        /// </summary>
        [Flags]
        public enum SHCNRF
        {
            NONE                = 0x0000,
            InterruptLevel      = 0x0001,
            ShellLevel          = 0x0002,
            RecursiveInterrupt  = 0x1000,
            NewDelivery         = 0x8000
        }

        /// <summary>
        /// Flags directing the handling of the item from which you're 
        /// retrieving the info tip text. This value is commonly zero (DEFAULT).
        /// Used in the IQueryInfo::GetInfoTip method.
        /// </summary>
        [Flags]
        public enum QITIPF
        {
            DEFAULT         = 0x00000,
            USENAME         = 0x00001,
            LINKNOTARGET    = 0x00002,
            LINKUSETARGET   = 0x00004,
            USESLOWTIP      = 0x00008
        }

        /// <summary>
        /// Indicates the default column state in the 
        /// IShellFolder2::GetDefaultColumnState method.
        /// </summary>
        public enum SHCOLSTATEF : uint
        {
            NONE            = 0,
            TYPE_STR        = 0x1,
            TYPE_INT        = 0x2,
            TYPE_DATE       = 0x3,
            TYPEMASK        = 0xf,
            ONBYDEFAULT     = 0x10,
            SLOW            = 0x20,
            EXTENDED        = 0x40,
            SECONDARYUI     = 0x80,
            HIDDEN          = 0x100,
            PREFER_VARCMP   = 0x200
        }

        /// <summary>
        /// Set of constants used to specify the folder view type.
        /// </summary>
        public enum FOLDERVIEWMODE : uint
        {
            FIRST = 1,
            ICON = 1,
            SMALLICON = 2,
            LIST = 3,
            DETAILS = 4,
            THUMBNAIL = 5,
            TILE = 6,
            THUMBSTRIP = 7,
            LAST = 7
        }

        /// <summary>
        /// Set of flags used to specify folder view options. 
        /// They are independent of each other, and can be used in any combination.
        /// </summary>
        [Flags]
        public enum FOLDERFLAGS : uint
        {
            AUTOARRANGE = 0x1,
            ABBREVIATEDNAMES = 0x2,
            SNAPTOGRID = 0x4,
            OWNERDATA = 0x8,
            BESTFITWINDOW = 0x10,
            DESKTOP = 0x20,
            SINGLESEL = 0x40,
            NOSUBFOLDERS = 0x80,
            TRANSPARENT = 0x100,
            NOCLIENTEDGE = 0x200,
            NOSCROLL = 0x400,
            ALIGNLEFT = 0x800,
            NOICONS = 0x1000,
            SHOWSELALWAYS = 0x2000,
            NOVISIBLE = 0x4000,
            SINGLECLICKACTIVATE = 0x8000,
            NOWEBVIEW = 0x10000,
            HIDEFILENAMES = 0x20000,
            CHECKSELECT = 0x40000
        }

        public enum SEE_MASK : int
        {
            NONE = 0x00000000,
            CLASSNAME = 0x00000001,
            CLASSKEY = 0x00000003,
            // Note INVOKEIDLIST overrides IDLIST
            IDLIST = 0x00000004,
            INVOKEIDLIST = 0x0000000c,
            ICON = 0x00000010,
            HOTKEY = 0x00000020,
            NOCLOSEPROCESS = 0x00000040,
            CONNECTNETDRV = 0x00000080,
            FLAG_DDEWAIT = 0x00000100,
            DOENVSUBST =  0x00000200,
            FLAG_NO_UI = 0x00000400,
            UNICODE = 0x00004000,
            NO_CONSOLE = 0x00008000,
            ASYNCOK = 0x00100000,
            HMONITOR = 0x00200000,
            NOZONECHECKS = 0x00800000,
            NOQUERYCLASSSTORE = 0x01000000,
            WAITFORINPUTIDLE = 0x02000000,
            FLAG_LOG_USAGE = 0x04000000
        }

        [Flags]
        public enum DROPEFFECT : uint
        {
            NONE = 0,
            COPY = 1,
            MOVE = 2,
            LINK = 4,
            SCROLL = 0x80000000
        }

        public enum FO_Func : uint
        {
            MOVE = 0x0001,
            COPY = 0x0002,
            DELETE = 0x0003,
            RENAME = 0x0004
        }
    }
}
