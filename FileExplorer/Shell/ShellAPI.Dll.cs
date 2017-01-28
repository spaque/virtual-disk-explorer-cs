using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using FileExplorer.Shell.Interfaces;

namespace FileExplorer.Shell
{
    /// <summary>
    /// Contains every function, enumeration, struct and constant from
    /// the Windows API required by FileExplorer.
    /// </summary>
    public static partial class ShellAPI
    {
        #region Shell32

        [DllImport("shell32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHFindFiles(
            IntPtr pidlFolder,
            IntPtr pidlSaveFile);

        [DllImport("shell32.dll", 
                   CharSet = CharSet.Unicode,
                   SetLastError = true)]
        public static extern int SHFileOperation(
            [In] 
            ref SHFILEOPSTRUCT lpFileOp);

        /// <summary>
        /// Reinitializes the system image list.
        /// </summary>
        /// <param name="fRestoreCache">
        /// TRUE to restore the system image cache from disk; FALSE otherwise.
        /// </param>
        /// <returns>
        /// TRUE if the cache was successfully refreshed, 
        /// FALSE if the initialization failed.
        /// </returns>
        [DllImport("shell32.dll",
                    EntryPoint = "#660",
                    SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileIconInit(
            [MarshalAs(UnmanagedType.Bool)]
            bool fRestoreCache);

        /// <summary>
        /// Retrieves information about an object in the file system, 
        /// such as a file, a folder, a directory, or a drive root.
        /// </summary>
        /// <param name="pszPath">
        /// Pointer to a null-terminated string of maximum length 
        /// MAX_PATH that contains the path and file name. Both 
        /// absolute and relative paths are valid.
        /// </param>
        /// <param name="dwFileAttributes">
        /// Combination of one or more file attribute flags. If uFlags does not 
        /// include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.
        /// </param>
        /// <param name="sfi">
        /// Address of a SHFILEINFO structure to receive the file information.
        /// </param>
        /// <param name="cbFileInfo">
        /// Size, in bytes, of the SHFILEINFO structure 
        /// pointed to by the psfi parameter.
        /// </param>
        /// <param name="uFlags">
        /// Flags that specify the file information to retrieve.
        /// uFlags does not include the PIDL flag.
        /// </param>
        /// <returns>
        /// If uFlags does not contain SHGFI_EXETYPE or SHGFI_SYSICONINDEX, 
        /// the return value is nonzero if successful, or zero otherwise.
        /// If uFlags contains the SHGFI_EXETYPE flag, the return 
        /// value specifies the type of the executable file.
        /// </returns>
        [DllImport("shell32.dll",
                   EntryPoint="SHGetFileInfo",
                   ExactSpelling=false,
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(
            String          pszPath,
            FILE_ATTRIBUTE  dwFileAttributes,
            ref SHFILEINFO  sfi,
            int             cbFileInfo,
            SHGFI           uFlags);

        /// <summary>
        /// Retrieves information about an object in the file system, 
        /// such as a file, a folder, a directory, or a drive root.
        /// </summary>
        /// <param name="ppidl">
        /// The address of an ITEMIDLIST (PIDL) structure that contains the 
        /// list of item identifiers that uniquely identifies the file within 
        /// the Shell's namespace. The pointer to an item identifier list (PIDL) 
        /// must be a fully qualified PIDL. Relative PIDLs are not allowed.
        /// </param>
        /// <param name="dwFileAttributes">
        /// Combination of one or more file attribute flags. If uFlags does not 
        /// include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.
        /// </param>
        /// <param name="sfi">
        /// Address of a SHFILEINFO structure to receive the file information.
        /// </param>
        /// <param name="cbFileInfo">
        /// Size, in bytes, of the SHFILEINFO structure 
        /// pointed to by the psfi parameter.
        /// </param>
        /// <param name="uFlags">
        /// Flags that specify the file information to retrieve.
        /// uFlags includes the PIDL flag.
        /// </param>
        /// <returns>
        /// If uFlags does not contain SHGFI_EXETYPE or SHGFI_SYSICONINDEX, 
        /// the return value is nonzero if successful, or zero otherwise.
        /// If uFlags contains the SHGFI_EXETYPE flag, the return 
        /// value specifies the type of the executable file.
        /// </returns>
        [DllImport("shell32.dll",
                   EntryPoint = "SHGetFileInfo",
                   ExactSpelling = false,
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern IntPtr SHGetFileInfo(
            IntPtr          ppidl,
            FILE_ATTRIBUTE  dwFileAttributes,
            ref SHFILEINFO  sfi,
            int             cbFileInfo,
            SHGFI           uFlags);

        /// <summary>
        /// Takes the CSIDL of a folder and returns the path.
        /// </summary>
        /// <param name="hwndOwner">
        /// Handle to an owner window.
        /// </param>
        /// <param name="nFolder">
        /// A CSIDL value that identifies the folder whose path is to 
        /// be retrieved. Only real folders are valid. If a virtual 
        /// folder is specified, this function will fail.
        /// </param>
        /// <param name="hToken">
        /// An access token that can be used to represent a particular user.
        /// </param>
        /// <param name="dwFlags">
        /// Flags to specify which path is to be returned. It 
        /// is used for cases where the folder associated with 
        /// a CSIDL may be moved or renamed by the user.
        /// </param>
        /// <param name="pszPath">
        /// Pointer to a null-terminated string of length MAX_PATH which 
        /// will receive the path. If an error occurs or S_FALSE is 
        /// returned, this string will be empty.
        /// </param>
        /// <returns>
        /// Returns standard HRESULT codes, including:
        /// S_FALSE - SHGetFolderPathA only. The CSIDL in nFolder is valid, 
        ///           but the folder does not exist.
        /// E_FAIL  - SHGetFolderPathW only. The CSIDL in nFolder is valid, 
        ///           but the folder does not exist.
        /// E_INVALIDARG - The CSIDL in nFolder is not valid. 
        /// </returns>
        [DllImport("shell32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern Int32 SHGetFolderPath(
            IntPtr  hwndOwner,
            CSIDL   nFolder,
            IntPtr  hToken,
            SHGFP   dwFlags,
            StringBuilder pszPath);

        /// <summary>
        /// Retrieves the IShellFolder interface for the desktop folder, 
        /// which is the root of the Shell's namespace.
        /// </summary>
        /// <param name="ppshf">
        /// Address that receives an IShellFolder 
        /// interface pointer for the desktop folder.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or 
        /// an OLE-defined error result otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern Int32 SHGetDesktopFolder(
            out IntPtr ppshf);

        /// <summary>
        /// Retrieves a pointer to the ITEMIDLIST structure of a special folder.
        /// </summary>
        /// <param name="hwndOwner">
        /// Handle to the owner window the client should specify 
        /// if it displays a dialog box or message box.
        /// </param>
        /// <param name="nFolder">
        /// A CSIDL value that identifies the folder of interest.
        /// </param>
        /// <param name="ppidl">
        /// A pointer to an item identifier list (PIDL) specifying the folder's 
        /// location relative to the root of the namespace (the desktop).
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   EntryPoint = "SHGetSpecialFolderLocation",
                   ExactSpelling = true,
                   CharSet = CharSet.Ansi,
                   SetLastError = true)]
        public static extern Int32 SHGetSpecialFolderLocation(
            IntPtr      hwndOwner,
            CSIDL       nFolder,
            out IntPtr  ppidl);

        /// <summary>
        /// This function takes the pointer to a fully-qualified item 
        /// identifier list (PIDL), and returns a specified interface 
        /// pointer on the parent object.
        /// </summary>
        /// <param name="pidl">
        /// The item's pointer to an item identifier list (PIDL).
        /// </param>
        /// <param name="riid">
        /// The REFIID of one of the interfaces exposed 
        /// by the item's parent object.
        /// </param>
        /// <param name="ppv">
        /// A pointer to the interface specified by riid. 
        /// You must release the object when you are finished.
        /// </param>
        /// <param name="ppidlLast">
        /// The item's PIDL relative to the parent folder. This PIDL can be 
        /// used with many of the methods supported by the parent folder's 
        /// interfaces. If you set ppidlLast to NULL, the PIDL is not returned.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern Int32 SHBindToParent(
            IntPtr      pidl,
            ref Guid    riid,
            out IntPtr  ppv,
            out IntPtr  ppidlLast);

        /// <summary>
        /// Registers a window that receives notifications 
        /// from the file system or shell.
        /// </summary>
        /// <param name="hwnd">
        /// Handle to the window that receives the 
        /// change or notification messages.
        /// </param>
        /// <param name="fSources">
        /// One or more of the following values that indicate the 
        /// type of events for which to receive notifications.
        /// </param>
        /// <param name="fEvents">
        /// Change notification events for which to receive notification.
        /// </param>
        /// <param name="wMsg">
        /// Message to be posted to the window procedure.
        /// </param>
        /// <param name="cEntries">
        /// Number of entries in the pfsne array.
        /// </param>
        /// <param name="pfsne">
        /// Array of SHChangeNotifyEntry structures that contain the 
        /// notifications. This array should always be set to one when 
        /// calling SHChangeNotifyRegister or SHChangeNotifyDeregister 
        /// will not work properly.
        /// </param>
        /// <returns>
        /// Returns a positive integer registration identifier (ID). 
        /// Returns zero if out of memory or in response to invalid parameters.
        /// </returns>
        [DllImport("shell32.dll", 
                   SetLastError = true)]
        public static extern uint SHChangeNotifyRegister(
            IntPtr  hwnd,
            SHCNRF  fSources,
            SHCNE   fEvents,
            WM      wMsg,
            int     cEntries,
            [MarshalAs(UnmanagedType.LPArray)]
            SHChangeNotifyEntry[] pfsne);

        /// <summary>
        /// Unregisters the client's window process 
        /// from receiving SHChangeNotify.
        /// </summary>
        /// <param name="hNotify">
        /// Specifies the registration identifier (ID) 
        /// returned by SHChangeNotifyRegister.
        /// </param>
        /// <returns>
        /// Returns TRUE if the specified client was found 
        /// and removed; returns FALSE otherwise.
        /// </returns>
        [DllImport("shell32.dll", 
                   SetLastError = true)]
        public static extern bool SHChangeNotifyDeregister(
            uint hNotify);

        /// <summary>
        /// Converts an item identifier list to a file system path.
        /// </summary>
        /// <param name="pidl">
        /// Address of an item identifier list that specifies 
        /// a file or directory location relative to the root 
        /// of the namespace (the desktop).
        /// </param>
        /// <param name="pszPath">
        /// Address of a buffer to receive the file system path. 
        /// This buffer must be at least MAX_PATH characters in size.
        /// </param>
        /// <returns>
        /// Returns TRUE if successful, or FALSE otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool SHGetPathFromIDList(
            IntPtr          pidl,
            StringBuilder   pszPath);

        /// <summary>
        /// SHGetRealIDL converts a simple PIDL to a full PIDL.
        /// </summary>
        /// <param name="psf">
        /// Pointer to an instance of IShellFolder whose simple 
        /// pointer to an item identifier list (PIDL) is to be converted.
        /// </param>
        /// <param name="pidlSimple">
        /// The simple PIDL to be converted.
        /// </param>
        /// <param name="ppidlReal">
        /// Pointer to the full converted PIDL. 
        /// If the function fails, this parameter is set to NULL.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern Int32 SHGetRealIDL(
            IShellFolder psf,
            IntPtr       pidlSimple,
            out IntPtr   ppidlReal);

        /// <summary>
        /// Clones an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">
        /// Pointer the ITEMIDLIST structure to be cloned.
        /// </param>
        /// <returns>
        /// Returns a pointer to a copy of the ITEMIDLIST structure pointed to by pidl.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern IntPtr ILClone(
            IntPtr pidl);

        /// <summary>
        /// Combines two ITEMIDLIST structures.
        /// </summary>
        /// <param name="pidl1">
        /// Pointer to the first ITEMIDLIST structure.
        /// </param>
        /// <param name="pidl2">
        /// Pointer to the second ITEMIDLIST structure. 
        /// This structure is appended to the structure pointed to by pidl1.
        /// </param>
        /// <returns>
        /// Returns an ITEMIDLIST containing the combined structures. 
        /// If you set either pidl1 or pidl2 to NULL, the returned 
        /// ITEMIDLIST structure is a clone of the non-NULL parameter. 
        /// Returns NULL if pidl1 and pidl2 are both set to NULL.
        /// </returns>
        [DllImport("shell32.dll",
                    SetLastError = true)]
        public static extern IntPtr ILCombine(
            IntPtr pidl1,
            IntPtr pidl2);

        /// <summary>
        /// Returns the ITEMIDLIST structure 
        /// associated with a specified file path.
        /// </summary>
        /// <param name="pwszPath">
        /// A NULL-terminated Unicode string that contains the path. 
        /// This string should be no more than MAX_PATH characters in length, 
        /// including the terminating NULL character.
        /// </param>
        /// <returns>
        /// Returns a pointer to an ITEMIDLIST structure 
        /// that corresponds to the path.
        /// </returns>
        [DllImport("shell32.dll",
                    CharSet = CharSet.Auto,
                    SetLastError = true)]
        public static extern IntPtr ILCreateFromPath(
            [MarshalAs(UnmanagedType.LPTStr)]
            String pwszPath);

        /// <summary>
        /// Determines whether a specified ITEMIDLIST structure 
        /// is the child of another ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidlParent">
        /// Pointer to the parent ITEMIDLIST structure.
        /// </param>
        /// <param name="pidlChild">
        /// Pointer to the child ITEMIDLIST structure.
        /// </param>
        /// <returns>
        /// Returns a pointer to the child's simple ITEMIDLIST structure 
        /// if pidlChild is a child of pidlParent. The returned structure 
        /// consists of pidlChild, minus the SHITEMID structures that make 
        /// up pidlParent. Returns NULL if pidlChild is not a child of pidlParent.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern IntPtr ILFindChild(
            IntPtr pidlParent,
            IntPtr pidlChild);

        /// <summary>
        /// Returns a pointer to the last SHITEMID 
        /// structure in an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to an ITEMIDLIST structure.
        /// </param>
        /// <returns>
        /// Pointer to the last SHITEMID structure in pidl.
        /// </returns>
        [DllImport("shell32.dll",
           SetLastError = true)]
        public static extern IntPtr ILFindLastID(
            IntPtr pidl);

        /// <summary>
        /// Frees an ITEMIDLIST structure allocated by the Shell.
        /// </summary>
        /// <param name="pidl">
        /// A pointer to the ITEMIDLIST structure to be freed. 
        /// This parameter can be NULL.
        /// </param>
        /// <remarks>
        /// ILFree is often used with ITEMIDLIST structures allocated by one 
        /// of the other ILXXX functions, but it can be used to free any 
        /// such structure returned by the Shell—for instance, the 
        /// ITEMIDLIST structure returned by SHBrowseForFolder 
        /// or used in a call to SHGetFolderLocation.
        /// </remarks>
        [DllImport("shell32.dll",
           SetLastError = true)]
        public static extern void ILFree(
            IntPtr pidl);

        /// <summary>
        /// Tests whether an ITEMIDLIST structure is the 
        /// parent of another ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidlParent">
        /// Pointer to an ITEMIDLIST (PIDL) structure that specifies 
        /// the parent. This must be an absolute PIDL.
        /// </param>
        /// <param name="pidlBelow">
        /// Pointer to an ITEMIDLIST (PIDL) structure that specifies 
        /// the child. This must be an absolute PIDL.
        /// </param>
        /// <param name="fImmediate">
        /// A Boolean value that is set to TRUE to test for immediate parents 
        /// of pidlBelow, or FALSE to test for any parents of pidlBelow.
        /// </param>
        /// <returns>
        /// Returns TRUE if pidlParent is a parent of pidlBelow. 
        /// If fImmediate is set to TRUE, the function only returns TRUE 
        /// if pidlParent is the immediate parent of pidlBelow. 
        /// Otherwise, the function returns FALSE.
        /// </returns>
        [DllImport("shell32.dll",
           SetLastError = true)]
        public static extern bool ILIsParent(
            IntPtr pidlParent,
            IntPtr pidlBelow,
            bool fImmediate);

        /// <summary>
        /// Tests whether two ITEMIDLIST structures are equal.
        /// </summary>
        /// <param name="pidl1">
        /// The first ITEMIDLIST structure.
        /// </param>
        /// <param name="pidl2">
        /// The second ITEMIDLIST structure.
        /// </param>
        /// <returns>
        /// Returns TRUE if the two structures are equal, FALSE otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern bool ILIsEqual(
            IntPtr pidl1,
            IntPtr pidl2);

        /// <summary>
        /// Removes the last SHITEMID structure from an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to the ITEMIDLIST structure to be shortened. When the 
        /// function returns, this variable points to the shortened structure.
        /// </param>
        /// <returns>
        /// Returns TRUE if successful, FALSE otherwise.
        /// </returns>
        [DllImport("shell32.dll",
                   SetLastError = true)]
        public static extern bool ILRemoveLastID(
            ref IntPtr pidl);

        [DllImport("shell32.dll",
                   CharSet=CharSet.Auto,
                   SetLastError = true)]
        public static extern bool ShellExecuteEx(
            ref SHELLEXECUTEINFO lpExecInfo);

        #endregion

        #region ShlwAPI

        /// <summary>
        /// Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, 
        /// converts it to a string, and places the result in a buffer.
        /// </summary>
        /// <param name="pstr">
        /// Pointer to the STRRET structure. 
        /// When the function returns, this pointer will no longer be valid.
        /// </param>
        /// <param name="pidl">
        /// Pointer to the item's ITEMIDLIST structure.
        /// </param>
        /// <param name="pszBuf">
        /// Buffer to hold the display name. It will be returned as a 
        /// null-terminated string. If cchBuf is too small, the name 
        /// will be truncated to fit.
        /// </param>
        /// <param name="cchBuf">
        /// Size of pszBuf, in characters. If cchBuf is too small, 
        /// the string will be truncated to fit.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [DllImport("shlwapi.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern Int32 StrRetToBuf(
            IntPtr pstr,
            IntPtr pidl,
            StringBuilder pszBuf,
            int cchBuf);

        #endregion

        #region User32

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">
        /// Specifies the type of hook procedure to be installed.
        /// </param>
        /// <param name="lpfn">
        /// Pointer to the hook procedure.
        /// </param>
        /// <param name="hInstance">
        /// Handle to the DLL containing the hook procedure pointed 
        /// to by the lpfn parameter.
        /// </param>
        /// <param name="threadId">
        /// Specifies the identifier of the thread with 
        /// which the hook procedure is to be associated.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the 
        /// handle to the hook procedure. 
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hInstance,
            int threadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook 
        /// chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="idHook">
        /// Handle to the hook to be removed.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);

        /// <summary>
        /// Passes the hook information to the next 
        /// hook procedure in the current hook chain.
        /// </summary>
        /// <param name="idHook">
        /// Ignored.
        /// </param>
        /// <param name="nCode">
        /// Specifies the hook code passed to the current hook procedure.
        /// </param>
        /// <param name="wParam">
        /// Specifies the wParam value passed to the current hook procedure.
        /// </param>
        /// <param name="lParam">
        /// Specifies the lParam value passed to the current hook procedure.
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. 
        /// The current hook procedure must also return this value. 
        /// The meaning of the return value depends on the hook type.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern IntPtr CallNextHookEx(
            IntPtr idHook,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. 
        /// It calls the window procedure for the specified window and does 
        /// not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">
        /// Handle to the window whose window procedure will receive the 
        /// message. If this parameter is HWND_BROADCAST, the message is 
        /// sent to all top-level windows in the system.
        /// </param>
        /// <param name="wMsg">
        /// Specifies the message to be sent.
        /// </param>
        /// <param name="wParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <returns>
        /// The return value specifies the result of the message 
        /// processing; it depends on the message sent.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr  hWnd,
            WM      Msg,
            int     wParam,
            IntPtr  lParam);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">
        /// Handle to the icon to be destroyed. The icon must not be in use.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. 
        /// To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern bool DestroyIcon(
            IntPtr hIcon);

        /// <summary>
        /// Displays a shortcut menu at the specified location and 
        /// tracks the selection of items on the shortcut menu.
        /// </summary>
        /// <param name="hmenu">
        /// Handle to the shortcut menu to be displayed.
        /// </param>
        /// <param name="fuFlags">
        /// Specifies function options.
        /// </param>
        /// <param name="x">
        /// Horizontal location of the shortcut menu, in screen coordinates.
        /// </param>
        /// <param name="y">
        /// Vertical location of the shortcut menu, in screen coordinates.
        /// </param>
        /// <param name="hwnd">
        /// Handle to the window that owns the shortcut menu. 
        /// This window receives all messages from the menu.
        /// </param>
        /// <param name="lptpm">
        /// Pointer to a TPMPARAMS structure that specifies an area of the 
        /// screen the menu should not overlap. This parameter can be NULL.
        /// </param>
        /// <returns>
        /// If you specify TPM_RETURNCMD in the fuFlags parameter, the return 
        /// value is the menu-item identifier of the item that the user selected.
        /// If the user cancels the menu without making a selection, or if an 
        /// error occurs, then the return value is zero.
        /// If you do not specify TPM_RETURNCMD in the fuFlags parameter, 
        /// the return value is nonzero if the function succeeds and zero 
        /// if it fails. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern uint TrackPopupMenuEx(
            IntPtr hmenu,
            TPM fuFlags,
            int x,
            int y,
            IntPtr hwnd,
            IntPtr lptpm);

        /// <summary>
        /// The CreatePopupMenu function creates a drop-down 
        /// menu, submenu, or shortcut menu.
        /// </summary>
        /// <returns>
        /// If the function succeeds, the return value 
        /// is a handle to the newly created menu.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern IntPtr CreatePopupMenu();

        /// <summary>
        /// The DestroyMenu function destroys the specified menu 
        /// and frees any memory that the menu occupies.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu to be destroyed.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern bool DestroyMenu(
            IntPtr hMenu);

        /// <summary>
        /// Appends a new item to the end of the specified menu 
        /// bar, drop-down menu, submenu, or shortcut menu.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu bar, drop-down menu, submenu, 
        /// or shortcut menu to be changed.
        /// </param>
        /// <param name="uFlags">
        /// Specifies flags to control the appearance 
        /// and behavior of the new menu item.
        /// </param>
        /// <param name="uIDNewItem">
        /// Specifies either the identifier of the new menu item 
        /// or, if the uFlags parameter is set to MF_POPUP, a 
        /// handle to the drop-down menu or submenu.
        /// </param>
        /// <param name="lpNewItem">
        /// Specifies the content of the new menu item.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool AppendMenu(
            IntPtr  hMenu,
            MFT     uFlags,
            uint    uIDNewItem,
            [MarshalAs(UnmanagedType.LPTStr)]
            String  lpNewItem);

        /// <summary>
        /// The InsertMenu function inserts a new menu item 
        /// into a menu, moving other items down the menu.
        /// </summary>
        /// <param name="hmenu">
        /// Handle to the menu to be changed.
        /// </param>
        /// <param name="uPosition">
        /// Specifies the menu item before which the new menu item is 
        /// to be inserted, as determined by the uFlags parameter.
        /// </param>
        /// <param name="uflags">
        /// Specifies flags that control the interpretation of the 
        /// uPosition parameter and the content, appearance, and 
        /// behavior of the new menu item.
        /// </param>
        /// <param name="uIDNewItem">
        /// Specifies either the identifier of the new menu item or, if 
        /// the uFlags parameter has the MF_POPUP flag set, a handle to 
        /// the drop-down menu or submenu.
        /// </param>
        /// <param name="lpNewItem">
        /// Specifies the content of the new menu item.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool InsertMenu(
            IntPtr  hmenu,
            uint    uPosition,
            MFT     uflags,
            uint    uIDNewItem,
            [MarshalAs(UnmanagedType.LPTStr)]
            String  lpNewItem);

        /// <summary>
        /// The InsertMenuItem function inserts a new menu 
        /// item at the specified position in a menu.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu in which the new menu item is inserted.
        /// </param>
        /// <param name="uItem">
        /// Identifier or position of the menu item before which 
        /// to insert the new item. The meaning of this parameter 
        /// depends on the value of fByPosition.
        /// </param>
        /// <param name="fByPosition">
        /// Value specifying the meaning of uItem. 
        /// If this parameter is FALSE, uItem is a menu item identifier. 
        /// Otherwise, it is a menu item position.
        /// </param>
        /// <param name="lpmii">
        /// Pointer to a MENUITEMINFO structure that contains 
        /// information about the new menu item.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool InsertMenuItem(
            IntPtr  hMenu,
            uint    uItem,
            bool    fByPosition,
            ref MENUITEMINFO lpmii);

        /// <summary>
        /// The RemoveMenu function deletes a menu item or detaches 
        /// a submenu from the specified menu. If the menu item opens 
        /// a drop-down menu or submenu, RemoveMenu does not destroy 
        /// the menu or its handle, allowing the menu to be reused.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu to be changed. 
        /// </param>
        /// <param name="uPosition">
        /// Specifies the menu item to be deleted, 
        /// as determined by the uFlags parameter.
        /// </param>
        /// <param name="uFlags">
        /// Specifies how the uPosition parameter is interpreted.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern bool RemoveMenu(
            IntPtr  hMenu,
            uint    uPosition,
            MF      uFlags);

        /// <summary>
        /// The GetMenuItemInfo function retrieves information about a menu item.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu that contains the menu item.
        /// </param>
        /// <param name="uItem">
        /// Identifier or position of the menu item to get information about. 
        /// The meaning of this parameter depends on the value of fByPosition.
        /// </param>
        /// <param name="fByPosition">
        /// Specifies the meaning of uItem. 
        /// If this parameter is FALSE, uItem is a menu item identifier. 
        /// Otherwise, it is a menu item position.
        /// </param>
        /// <param name="lpmii">
        /// Pointer to a MENUITEMINFO structure that specifies the 
        /// information to retrieve and receives information about 
        /// the menu item. Note that you must set MENUITEMINFO.cbSize 
        /// to sizeof(MENUITEMINFO) before calling this function.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool GetMenuItemInfo(
            IntPtr  hMenu,
            uint    uItem,
            bool    fByPosition,
            ref MENUITEMINFO lpmii);

        /// <summary>
        /// The SetMenuItemInfo function changes information about a menu item.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu that contains the menu item.
        /// </param>
        /// <param name="uItem">
        /// Identifier or position of the menu item to change. 
        /// The meaning of this parameter depends on the value of fByPosition.
        /// </param>
        /// <param name="fByPosition">
        /// Value specifying the meaning of uItem. 
        /// If this parameter is FALSE, uItem is a menu item identifier. 
        /// Otherwise, it is a menu item position.
        /// </param>
        /// <param name="lpmii">
        /// Pointer to a MENUITEMINFO structure that contains information about 
        /// the menu item and specifies which menu item attributes to change.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern bool SetMenuItemInfo(
            IntPtr  hMenu,
            uint    uItem,
            bool    fByPosition,
            ref MENUITEMINFO lpmii);

        /// <summary>
        /// The GetMenuDefaultItem function determines the 
        /// default menu item on the specified menu.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu for which to retrieve the default menu item.
        /// </param>
        /// <param name="fByPos">
        /// Specifies whether to retrieve the menu item's identifier 
        /// or its position. If this parameter is FALSE, the identifier 
        /// is returned. Otherwise, the position is returned.
        /// </param>
        /// <param name="gmdiFlags">
        /// Specifies how the function searches for menu items.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern int GetMenuDefaultItem(
            IntPtr  hMenu,
            bool    fByPos,
            GMDI    gmdiFlags);

        /// <summary>
        /// The SetMenuDefaultItem function sets the default 
        /// menu item for the specified menu.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu to set the default item for.
        /// </param>
        /// <param name="uItem">
        /// Identifier or position of the new default menu item 
        /// or -1 for no default item. The meaning of this 
        /// parameter depends on the value of fByPos.
        /// </param>
        /// <param name="fByPos">
        /// Value specifying the meaning of uItem. 
        /// If this parameter is FALSE, uItem is a menu item identifier. 
        /// Otherwise, it is a menu item position.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern bool SetMenuDefaultItem(
            IntPtr  hMenu,
            uint    uItem,
            bool    fByPos);

        /// <summary>
        /// The GetSubMenu function retrieves a handle to the drop-down 
        /// menu or submenu activated by the specified menu item.
        /// </summary>
        /// <param name="hMenu">
        /// Handle to the menu.
        /// </param>
        /// <param name="nPos">
        /// Specifies the zero-based relative position in the specified 
        /// menu of an item that activates a drop-down menu or submenu.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to 
        /// the drop-down menu or submenu activated by the menu item. 
        /// If the menu item does not activate a drop-down menu or submenu, 
        /// the return value is NULL.
        /// </returns>
        [DllImport("user32.dll",
                   SetLastError = true)]
        public static extern IntPtr GetSubMenu(
            IntPtr  hMenu,
            int     nPos);

        #endregion

        #region Comctl32

        /// <summary>
        /// Replaces an image with an icon or cursor.
        /// </summary>
        /// <param name="himl">
        /// Handle to the image list.
        /// </param>
        /// <param name="index">
        /// Index of the image to replace. 
        /// If i is -1, the function appends the image to the end of the list.
        /// </param>
        /// <param name="hicon">
        /// Handle to the icon or cursor that contains 
        /// the bitmap and mask for the new image.
        /// </param>
        /// <returns>
        /// Returns the index of the image if successful, or -1 otherwise.
        /// </returns>
        [DllImport("comctl32.dll",
                   SetLastError = true)]
        public static extern int ImageList_ReplaceIcon(
            IntPtr  himl,
            int     index,
            IntPtr  hicon);

        /// <summary>
        /// Adds an image or images to an image list.
        /// </summary>
        /// <param name="himl">
        /// Handle to the image list.
        /// </param>
        /// <param name="hbmImage">
        /// Handle to the bitmap that contains the image or images. 
        /// The number of images is inferred from the width of the bitmap.
        /// </param>
        /// <param name="hbmMask">
        /// Handle to the bitmap that contains the mask. 
        /// If no mask is used with the image list, this parameter is ignored.
        /// </param>
        /// <returns>
        /// Returns the index of the first new 
        /// image if successful, or -1 otherwise.
        /// </returns>
        [DllImport("comctl32.dll",
                   SetLastError = true)]
        public static extern int ImageList_Add(
            IntPtr himl,
            IntPtr hbmImage,
            IntPtr hbmMask);

        /// <summary>
        /// Creates an icon from an image and mask in an image list.
        /// </summary>
        /// <param name="himl">
        /// Handle to the image list.
        /// </param>
        /// <param name="index">
        /// Index of the image.
        /// </param>
        /// <param name="flags">
        /// Combination of flags that specify the drawing style.
        /// </param>
        /// <returns>
        /// Returns the handle to the icon if successful, or NULL otherwise.
        /// </returns>
        [DllImport("comctl32.dll",
                   SetLastError = true)]
        public static extern IntPtr ImageList_GetIcon(
            IntPtr  himl,
            int     index,
            ILD     flags);

        #endregion

        #region Kernel32

        [DllImport("kernel32.dll",
                   CharSet = CharSet.Auto,
                   SetLastError = true)]
        public static extern uint GetDriveType(
            [MarshalAs(UnmanagedType.LPTStr)]
            String lpRootPathName);

        #endregion
    }
}
