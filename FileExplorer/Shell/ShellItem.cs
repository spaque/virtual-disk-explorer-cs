using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using FileExplorer.Shell.Interfaces;

namespace FileExplorer.Shell
{
    public sealed class ShellItem : 
        IComparable<ShellItem>, IDisposable, IEquatable<ShellItem>
    {
        #region Fields

        private static ShellItem desktopItem;
        private static String systemFolderName;
        private static String myComputerName;
        private static String myDocumentsName;

        private ShellItem parent;
        private IShellFolder folder;
        private IntPtr relPidl;
        private IntPtr fullPidl;
        private String text, path, type, toolTip;
        private int imageIndex;

        private bool isDesktop;
        private bool isFolder;
        private bool isLink;
        private bool isShared;
        private bool isFileSystem;
        private bool isHidden;
        private bool hasSubFolder;
        private bool isBrowsable;
        private bool isDisk;
        private bool canRename;
        private bool canMove;
        private bool canCopy;
        private bool canDelete;
        private bool disposed;

        private bool hasItemInfo;
        private bool hasToolTip;
        private bool hasExtraInfo;

        private DateTime lastWriteTime;
        private long length;

        private List<ShellItem> subFolders;

        private int sortFlag;

        private object padlock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the desktop shell item.
        /// </summary>
        /// <param name="explorer">
        /// ShellExplorer class that uses the shell item.
        /// </param>
        /// <param name="pidl">
        /// Pointer to the desktop PIDL.
        /// </param>
        /// <param name="folderPtr">
        /// Pointer to the IShellFolder interface for the desktop. 
        /// </param>
        public ShellItem()
        {
            int hr;
            IntPtr folderPtr;

            // Obtain the IShellFolder interface for the Desktop item
            hr = ShellAPI.SHGetDesktopFolder(out folderPtr);
            if (hr != ShellAPI.S_OK)
                Marshal.ThrowExceptionForHR(hr);
            folder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(
                folderPtr, 
                typeof(IShellFolder));

            hr = ShellAPI.SHGetSpecialFolderLocation(IntPtr.Zero, ShellAPI.CSIDL.DESKTOP, out relPidl);
            if (hr != ShellAPI.S_OK)
                Marshal.ThrowExceptionForHR(hr);
            fullPidl = ShellAPI.ILClone(relPidl);

            SetDesktopAttributes();

            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGetFileInfo(relPidl,
                                   0,
                                   ref info,
                                   ShellAPI.cbFileInfo,
                                   ShellAPI.SHGFI.PIDL |
                                   ShellAPI.SHGFI.TYPENAME | 
                                   ShellAPI.SHGFI.DISPLAYNAME |
                                   ShellAPI.SHGFI.SYSICONINDEX);
            text = info.szDisplayName;
            path = info.szDisplayName;
            type = info.szTypeName;
            imageIndex = info.iIcon;

            InitMyComputer();
            InitMyDocuments();

            hasItemInfo = true;
            desktopItem = this;
            Marshal.Release(folderPtr);
        }

        /// <summary>
        /// Constructor for either a folder or a file item.
        /// </summary>
        /// <param name="explorer">
        /// ShellExplorer class that uses this item.
        /// </param>
        /// <param name="parent">
        /// The ShellItem class of the parent.
        /// </param>
        /// <param name="pidl">
        /// Relative PIDL of this item.
        /// </param>
        public ShellItem(ShellItem parent, IntPtr pidl)
        {
            if (pidl == IntPtr.Zero)
                throw new ArgumentException("Zero PIDL");
            IntPtr folderPtr = IntPtr.Zero;
            this.parent = parent;
            relPidl = pidl;
            fullPidl = ShellAPI.ILCombine(parent.fullPidl, pidl);
            SetAttributes();
            imageIndex = -1;
            if (isFolder)
            {
                int hr;
                hr = parent.folder.BindToObject(
                        pidl,
                        IntPtr.Zero,
                        ref ShellAPI.IID_IShellFolder,
                        out folderPtr);
                if (hr != ShellAPI.S_OK)
                    Marshal.ThrowExceptionForHR(hr);
                folder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(
                            folderPtr,
                            typeof(IShellFolder));
                if (folderPtr != IntPtr.Zero)
                    Marshal.Release(folderPtr);
            }
        }

        ~ShellItem()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        public ShellItem DesktopItem { get { return desktopItem; } }

        public String SystemFolderName { get { return systemFolderName; } }

        public String MyComputerName { get { return myComputerName; } }

        public String MyDocumentsName { get { return myDocumentsName; } }

        public ShellItem Parent { get { return parent; } }

        public IShellFolder Folder { get { return folder; } }

        public IntPtr RelPidl { get { return relPidl; } }

        public IntPtr FullPidl { get { return fullPidl; } }

        public String Text
        {
            get
            {
                if (!hasItemInfo)
                    SetItemInfo();
                return text;
            }
        }

        public String Path { get { return path; } }

        public String Type
        {
            get
            {
                if (!hasItemInfo)
                    SetItemInfo();
                return type;
            }
        }

        public int ImageIndex {
            get
            {
                if (imageIndex < 0)
                {
                    if (!hasItemInfo)
                        SetItemInfo();
                    ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
                    ShellAPI.SHGFI uFlags = 
                        ShellAPI.SHGFI.PIDL | 
                        ShellAPI.SHGFI.SYSICONINDEX;
                    ShellAPI.FILE_ATTRIBUTE dwAttr = 0;
                    if (isFileSystem && !isFolder)
                    {
                        uFlags |= ShellAPI.SHGFI.USEFILEATTRIBUTES;
                        dwAttr = ShellAPI.FILE_ATTRIBUTE.NORMAL;
                    }
                    IntPtr h = ShellAPI.SHGetFileInfo(fullPidl, dwAttr, ref info, ShellAPI.cbFileInfo, uFlags);
                    if (info.iIcon == 0)
                        imageIndex = 3;
                    else
                        imageIndex = info.iIcon;
                }
                return imageIndex;
            }
        }

        public bool IsDesktop { get { return isDesktop; } }

        public bool IsFolder { get { return isFolder; } }

        public bool IsLink { get { return isLink; } }

        public bool IsShared { get { return isShared; } }

        public bool IsFileSystem { get { return isFileSystem; } }

        public bool IsHidden { get { return isHidden; } }

        public bool HasSubFolder { get { return hasSubFolder; } }

        public bool IsExplorable { get { return isBrowsable; } }

        public bool IsDisk { get { return isDisk; } }

        public bool CanRename { get { return canRename; } }

        public bool CanMove { get { return canMove; } }

        public bool CanCopy { get { return canCopy; } }

        public bool CanDelete { get { return canDelete; } }

        public bool Disposed { get { return disposed; } }

        public int SortFlag
        {
            get
            {
                if (!hasItemInfo)
                    SetItemInfo();
                return sortFlag;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                if (!hasExtraInfo)
                    SetExtraInfo();
                return lastWriteTime;
            }
        }

        public long Length
        {
            get
            {
                if (!hasExtraInfo)
                    SetExtraInfo();
                return length;
            }
        }

        public String ToolTip
        {
            get
            {
                if (!hasToolTip)
                {
                    IntPtr queryInfoPtr;
                    IQueryInfo queryInfo;
                    ShellItem parent = this.parent != null ? this.parent : this;
                    int hr = parent.folder.GetUIObjectOf(
                        IntPtr.Zero,
                        1,
                        new IntPtr[] { relPidl },
                        ref ShellAPI.IID_IQueryInfo,
                        IntPtr.Zero,
                        out queryInfoPtr);
                    if (hr == ShellAPI.S_OK)
                    {
                        queryInfo =
                            (IQueryInfo)Marshal.GetTypedObjectForIUnknown(
                                queryInfoPtr, typeof(IQueryInfo));
                        queryInfo.GetInfoTip(ShellAPI.QITIPF.DEFAULT, out toolTip);
                        Marshal.Release(queryInfoPtr);
                        Marshal.ReleaseComObject(queryInfo);
                        hasToolTip = true;
                    }
                    else
                    {
                        toolTip = "";
                        hasToolTip = true;
                    }
                }
                return toolTip;
            }
        }

        #endregion

        #region Init Methods

        /// <summary>
        /// Retrieves the names of both My Computer and the
        /// system folder, since they are commonly used.
        /// </summary>
        private void InitMyComputer()
        {
            IntPtr tempPidl;
            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();

            // My Computer
            ShellAPI.SHGetSpecialFolderLocation(
                IntPtr.Zero,
                ShellAPI.CSIDL.DRIVES,
                out tempPidl);
            ShellAPI.SHGetFileInfo(
                tempPidl,
                0,
                ref info,
                ShellAPI.cbFileInfo,
                ShellAPI.SHGFI.DISPLAYNAME |
                ShellAPI.SHGFI.TYPENAME |
                ShellAPI.SHGFI.PIDL);
            systemFolderName = info.szTypeName;
            myComputerName = info.szDisplayName;
            ShellAPI.ILFree(tempPidl);
        }

        /// <summary>
        /// Retrieves the name of My Documents
        /// </summary>
        private void InitMyDocuments()
        {
            IntPtr tempPidl;
            uint pchEaten;
            ShellAPI.SFGAO attr = 0;
            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();

            folder.ParseDisplayName(
                IntPtr.Zero,
                IntPtr.Zero,
                "::{450d8fba-ad25-11d0-98a8-0800361b1103}",
                out pchEaten,
                out tempPidl,
                ref attr);
            ShellAPI.SHGetFileInfo(
                tempPidl,
                0,
                ref info,
                ShellAPI.cbFileInfo,
                ShellAPI.SHGFI.PIDL | ShellAPI.SHGFI.DISPLAYNAME);
            myDocumentsName = info.szDisplayName;
            ShellAPI.ILFree(tempPidl);
        }

        private void SetItemInfo()
        {
            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI uFlags = 
                ShellAPI.SHGFI.TYPENAME | 
                ShellAPI.SHGFI.PIDL;
            ShellAPI.FILE_ATTRIBUTE dwAttr = 0;
            if (isFileSystem && !isFolder)
            {
                uFlags |= ShellAPI.SHGFI.USEFILEATTRIBUTES;
                dwAttr = ShellAPI.FILE_ATTRIBUTE.NORMAL;
            }
            SetText();
            IntPtr h = ShellAPI.SHGetFileInfo(fullPidl, dwAttr, ref info, ShellAPI.cbFileInfo, uFlags);
            type = info.szTypeName;
            if (text.Length == 0)
                text = path;
            sortFlag = ComputeSortFlag();
            hasItemInfo = true;
        }

        /// <summary>
        /// Retrives the name of the ShellItem.
        /// </summary>
        private void SetText()
        {
            // reserve memory for an unmanaged STRRET structure
            // uType -> 4 bytes
            // union -> MAX_PATH * 2
            IntPtr str = Marshal.AllocCoTaskMem((ShellAPI.MAX_PATH << 1) + 4);
            // set strret to 0
            Marshal.WriteInt32(str, 0, 0);
            StringBuilder buf = new StringBuilder(ShellAPI.MAX_PATH);
            int hr;

            hr = parent.folder.GetDisplayNameOf(relPidl,
                                                ShellAPI.SHGNO.INFOLDER,
                                                str);
            if (hr == ShellAPI.S_OK)
            {
                ShellAPI.StrRetToBuf(str, relPidl, buf, ShellAPI.MAX_PATH);
                text = buf.ToString();
            }
            Marshal.FreeCoTaskMem(str);
        }

        /// <summary>
        /// Retrives the path of the ShellItem.
        /// </summary>
        private void SetPath()
        {
            IntPtr str = Marshal.AllocCoTaskMem((ShellAPI.MAX_PATH << 1) + 4);
            Marshal.WriteInt32(str, 0, 0);
            StringBuilder buf = new StringBuilder(ShellAPI.MAX_PATH);
            int hr;

            hr = parent.folder.GetDisplayNameOf(
                relPidl, ShellAPI.SHGNO.FORADDRESSBAR | ShellAPI.SHGNO.FORPARSING, str);

            if (hr == ShellAPI.S_OK)
            {
                ShellAPI.StrRetToBuf(str, relPidl, buf, ShellAPI.MAX_PATH);
                path = buf.ToString();
            }
            Marshal.FreeCoTaskMem(str);
        }

        /// <summary>
        /// Sets the attributes for the desktop item.
        /// </summary>
        private void SetDesktopAttributes()
        {
            isDesktop = true;
            isFolder = true;
            isLink = false;
            isShared = false;
            isFileSystem = true;
            isHidden = false;
            hasSubFolder = true;
            isBrowsable = true;
            isDisk = false;
            canRename = false;
            canCopy = false;
            canMove = false;
            canDelete = false;
        }

        /// <summary>
        /// Sets the attributes of the folder/file.
        /// </summary>
        private void SetAttributes()
        {
            ShellAPI.SFGAO attr = ShellAPI.SFGAO.BROWSABLE |
                                  ShellAPI.SFGAO.FILESYSTEM |
                                  ShellAPI.SFGAO.HASSUBFOLDER |
                                  ShellAPI.SFGAO.FOLDER |
                                  ShellAPI.SFGAO.LINK |
                                  ShellAPI.SFGAO.SHARE |
                                  ShellAPI.SFGAO.CANCOPY |
                                  ShellAPI.SFGAO.CANDELETE |
                                  ShellAPI.SFGAO.CANMOVE |
                                  ShellAPI.SFGAO.CANRENAME;
            parent.folder.GetAttributesOf(1, new IntPtr[] { relPidl }, ref attr);

            isDesktop = false;
            isBrowsable = (attr & ShellAPI.SFGAO.BROWSABLE) != 0;
            isFileSystem = (attr & ShellAPI.SFGAO.FILESYSTEM) != 0;
            hasSubFolder = (attr & ShellAPI.SFGAO.HASSUBFOLDER) != 0;
            isFolder = (attr & ShellAPI.SFGAO.FOLDER) != 0;
            isLink = (attr & ShellAPI.SFGAO.LINK) != 0;
            isShared = (attr & ShellAPI.SFGAO.SHARE) != 0;
            canCopy = (attr & ShellAPI.SFGAO.CANCOPY) != 0;
            canDelete = (attr & ShellAPI.SFGAO.CANDELETE) != 0;
            canMove = (attr & ShellAPI.SFGAO.CANMOVE) != 0;
            canRename = (attr & ShellAPI.SFGAO.CANRENAME) != 0;

            SetPath();

            isDisk = (path.Length == 3) && (path.EndsWith(@":\"));

            if (isFolder && isFileSystem)
            {
                attr = ShellAPI.SFGAO.STREAM;
                parent.folder.GetAttributesOf(1, new IntPtr[] { relPidl }, ref attr);
                if ((attr & ShellAPI.SFGAO.STREAM) != 0)
                    isFolder = false;
            }
        }

        private void SetExtraInfo()
        {
            if (!isFolder && isFileSystem && File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                lastWriteTime = fi.LastAccessTime;
                length = fi.Length;
            }
            else if (isFolder && isFileSystem && Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                lastWriteTime = di.LastWriteTime;
            }
            hasExtraInfo = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the requested items of this folder as an List.
        /// </summary>
        /// <param name="flags">
        /// SHCONTF flags indicating which items to return.
        /// </param>
        /// <returns>
        /// A List containing the requested items.
        /// </returns>
        private List<ShellItem> GetFolderContents()
        {
            ShellAPI.SHCONTF flags = 
                ShellAPI.SHCONTF.INCLUDEHIDDEN | ShellAPI.SHCONTF.FOLDERS;
            List<ShellItem> contents = new List<ShellItem>();
            IntPtr enumPtr = IntPtr.Zero;
            IEnumIDList itemEnum = null;
            IntPtr subItem = IntPtr.Zero;
            int celtFetched;
            int hr;

            if (folder != null)
            {
                hr = folder.EnumObjects(IntPtr.Zero, flags, out enumPtr);
                if (hr == ShellAPI.S_OK)
                {
                    itemEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumPtr, typeof(IEnumIDList));
                    hr = itemEnum.Next(1, out subItem, out celtFetched);
                    while (hr == ShellAPI.S_OK && celtFetched > 0)
                    {
                        contents.Add(new ShellItem(this, subItem));
                        hr = itemEnum.Next(1, out subItem, out celtFetched);
                    }
                }
                if (itemEnum != null)
                {
                    Marshal.ReleaseComObject(itemEnum);
                    Marshal.Release(enumPtr);
                    contents.TrimExcess();
                }
            }
            return contents;
        }

        private List<IntPtr> GetFilesPidl()
        {
            List<IntPtr> contents = new List<IntPtr>();
            IntPtr enumPtr = IntPtr.Zero;
            IEnumIDList itemEnum = null;
            IntPtr subItem = IntPtr.Zero;
            int celtFetched;
            int hr;
            ShellAPI.SHCONTF flags = 
                ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;

            if (folder != null)
            {
                if (isDesktop || parent.isDesktop)
                {
                    hr = folder.EnumObjects(IntPtr.Zero, flags, out enumPtr);
                    if (hr == ShellAPI.S_OK)
                    {
                        itemEnum = 
                            (IEnumIDList)Marshal.GetTypedObjectForIUnknown(
                                enumPtr, typeof(IEnumIDList));
                        hr = itemEnum.Next(1, out subItem, out celtFetched);
                        while (hr == ShellAPI.S_OK && celtFetched > 0)
                        {
                            ShellAPI.SFGAO attr = ShellAPI.SFGAO.FOLDER |
                                                  ShellAPI.SFGAO.STREAM;
                            folder.GetAttributesOf(
                                1, new IntPtr[] { subItem }, ref attr);
                            // if it's not a folder or it's a zip file add the item to the result
                            if ((attr & ShellAPI.SFGAO.FOLDER) == 0 ||
                                (((attr & ShellAPI.SFGAO.FOLDER) != 0) &&
                                 ((attr & ShellAPI.SFGAO.STREAM) != 0)))
                                contents.Add(subItem);
                            else
                                ShellAPI.ILFree(subItem);
                            hr = itemEnum.Next(1, out subItem, out celtFetched);
                        }
                    }
                }
                else
                {
                    hr = folder.EnumObjects(IntPtr.Zero, flags, out enumPtr);
                    if (hr == ShellAPI.S_OK)
                    {
                        itemEnum = 
                            (IEnumIDList)Marshal.GetTypedObjectForIUnknown(
                                enumPtr, typeof(IEnumIDList));
                        hr = itemEnum.Next(1, out subItem, out celtFetched);
                        while (hr == ShellAPI.S_OK && celtFetched > 0)
                        {
                            contents.Add(subItem);
                            hr = itemEnum.Next(1, out subItem, out celtFetched);
                        }
                    }
                }
                if (itemEnum != null)
                {
                    Marshal.ReleaseComObject(itemEnum);
                    Marshal.Release(enumPtr);
                    contents.TrimExcess();
                }
            }
            return contents;
        }

        /// <summary>
        /// Returns the requested items of this folder as an List.
        /// </summary>
        /// <param name="flags">
        /// SHCONTF flags indicating which items to return.
        /// </param>
        /// <returns>
        /// A List containing the PIDL of the requested items.
        /// </returns>
        private List<IntPtr> GetFoldersPidl()
        {
            ShellAPI.SHCONTF flags = 
                ShellAPI.SHCONTF.INCLUDEHIDDEN | ShellAPI.SHCONTF.FOLDERS;
            List<IntPtr> contents = new List<IntPtr>();
            IntPtr enumPtr = IntPtr.Zero;
            IEnumIDList itemEnum = null;
            IntPtr subItem = IntPtr.Zero;
            int celtFetched;
            int hr;

            if (folder != null)
            {
                hr = folder.EnumObjects(IntPtr.Zero, flags, out enumPtr);
                if (hr == ShellAPI.S_OK)
                {
                    itemEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumPtr, typeof(IEnumIDList));
                    hr = itemEnum.Next(1, out subItem, out celtFetched);
                    while (hr == ShellAPI.S_OK && celtFetched > 0)
                    {
                        contents.Add(subItem);
                        hr = itemEnum.Next(1, out subItem, out celtFetched);
                    }
                }
                if (itemEnum != null)
                {
                    Marshal.ReleaseComObject(itemEnum);
                    Marshal.Release(enumPtr);
                    contents.TrimExcess();
                }
            }
            return contents;
        }

        private void LoadShellItems(int id, ShellItem[] items, List<IntPtr> pidls, int start, int end)
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            for (int i = start; i < end; i++)
            {
                items[i] = new ShellItem(this, pidls[i]);
            }
            watch.Stop();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the sub-folders of this ShellItem.
        /// </summary>
        /// <param name="refresh">
        /// True to update the contents of this item.
        /// </param>
        /// <returns>
        /// A list of ShellItems.
        /// </returns>
        public List<ShellItem> GetFolders(bool refresh)
        {
            if (isFolder)
            {
                if (refresh || subFolders == null)
                    RefreshFolders();
                return subFolders;
            }
            else
                return new List<ShellItem>();
        }

        /// <summary>
        /// Returns the sub-files of this ShellItem.
        /// </summary>
        /// <returns>
        /// A list of ShellItems.
        /// </returns>
        public List<ShellItem> GetFiles()
        {
            if (isFolder)
            {
                ShellItem[] items;
                List<ShellItem> res;
                List<IntPtr> pidls = GetFilesPidl();
                int count = pidls.Count;
                items = new ShellItem[count];
                res = new List<ShellItem>(count);

                bool largeFolder = count > 90;
                bool files = count != 0;
                Thread t1 = null;
                Thread t2 = null;
                Thread t3 = null;
                if (files)
                {
                    if (largeFolder)
                    {
                        int quarter = count >> 2;
                        int half = count >> 1;
                        t1 = new Thread(delegate() { LoadShellItems(1, items, pidls, 0, quarter); });
                        t2 = new Thread(delegate() { LoadShellItems(2, items, pidls, quarter, half); });
                        t3 = new Thread(delegate() { LoadShellItems(3, items, pidls, half, quarter + half); });
                        t1.SetApartmentState(ApartmentState.MTA);
                        t2.SetApartmentState(ApartmentState.MTA);
                        t3.SetApartmentState(ApartmentState.MTA);
                        t1.Start();
                        t2.Start();
                        t3.Start();
                        LoadShellItems(4, items, pidls, quarter + half, count);
                    }
                    else
                    {
                        LoadShellItems(11, items, pidls, 0, count);
                    }
                }

                if (files)
                {
                    if (largeFolder)
                    {
                        t1.Join();
                        t2.Join();
                        t3.Join();
                    }
                    res.AddRange(items);
                }
                return res;
            }
            return new List<ShellItem>();
        }

        /// <summary>
        /// Updates the sub-folders of this ShellItem.
        /// </summary>
        /// <returns>
        /// Returns true if there were any changes.
        /// </returns>
        public bool RefreshFolders()
        {
            bool refresh = false;
            if (isFolder)
            {
                if (subFolders == null)
                {
                    subFolders = GetFolderContents();
                    // Changed from unexamined to examined
                    refresh = true;
                }
                else
                {
                    List<IntPtr> curPidls = GetFoldersPidl();
                    if (curPidls.Count < 1 && subFolders.Count > 0)
                    {
                        // Nothing there anymore
                        foreach (ShellItem sh in subFolders)
                            sh.Dispose();
                        subFolders = new List<ShellItem>();
                        // Changed from had some to have none
                        refresh = true;
                    }
                    else if (subFolders.Count < 1)
                    {
                        // Didn't have any before
                        subFolders = GetFolderContents();
                        // Changed from had none to have some
                        refresh = true;
                    }
                    else
                    {
                        // Some before some now. Are they the same?
                        List<ShellItem> invalidFolders = new List<ShellItem>();
                        List<IntPtr> compList = new List<IntPtr>(curPidls);
                        IntPtr[] oldPidls = new IntPtr[subFolders.Count];
                        bool match;

                        for (int i = 0; i < subFolders.Count; i++)
                            oldPidls[i] = subFolders[i].relPidl;
                        // Check for changes in the folder
                        for (int iOld = 0; iOld < subFolders.Count; iOld++)
                        {
                            match = false;
                            for (int iNew = 0; iNew < compList.Count; iNew++)
                            {
                                if (ShellAPI.ILIsEqual(compList[iNew], oldPidls[iOld]))
                                {
                                    compList.RemoveAt(iNew);
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                // The folder is not there anymore
                                invalidFolders.Add(subFolders[iOld]);
                                refresh = true;
                            }
                        }
                        foreach (ShellItem sh in invalidFolders)
                        {
                            sh.Dispose();
                            subFolders.Remove(sh);
                        }
                        // Anything remaining in compList is a new entry
                        if (compList.Count > 0)
                        {
                            refresh = true;
                            foreach (IntPtr ptr in compList)
                                subFolders.Add(new ShellItem(this, ShellAPI.ILClone(ptr)));
                        }
                    }
                    foreach (IntPtr ptr in curPidls)
                        ShellAPI.ILFree(ptr);
                }
            }

            return refresh;
        }

        /// <summary>
        /// Test whether this ShellItem is ancestor of another ShellItem.
        /// </summary>
        /// <param name="item">
        /// The ShellItem that specifies the child.
        /// </param>
        /// <param name="inmediate">
        /// A bool value that is set to TRUE to test for immediate parents,
        /// or FALSE to test for any parents of item.
        /// </param>
        /// <returns>
        /// Returns TRUE if this is a parent of item. 
        /// If fImmediate is set to TRUE, the function only returns TRUE if
        /// this is the immediate parent of item or if they are the same item.
        /// Otherwise, the function returns FALSE.
        /// </returns>
        public bool IsAncestorOf(ShellItem item, bool inmediate)
        {
            return ShellAPI.ILIsParent(fullPidl, item.fullPidl, inmediate);
        }

        #endregion

        #region IComparable

        /// <summary>
        /// Compares the current ShellItem with another ShellItem.
        /// </summary>
        /// <param name="other">
        /// An ShellItem to compare with this instance.
        /// </param>
        /// <returns>
        /// Less than zero      -> This instance is less than other.
        /// Zero                -> This instance is equal to other.
        /// Greater than zero   -> This instance is greater than other.
        /// </returns>
        public int CompareTo(ShellItem other)
        {
            if (other == null)
                return 1;
            int cmp = other.SortFlag - SortFlag;
            if (cmp != 0)
                return cmp;
            if (isDisk)
                return String.Compare(other.path, path, true);
            return String.Compare(other.text, text, true);
        }

        /// <summary>
        /// Calculates the sort key for this item based on its attributes.
        /// </summary>
        /// <returns>
        /// Returns the sort key of the item.
        /// </returns>
        private int ComputeSortFlag()
        {
            if (isFolder)
            {
                if (isDisk)
                    return 1;
                if (String.Compare(text, myDocumentsName, true) == 0 &&
                    String.Compare(type, systemFolderName, true) == 0)
                    return 2;
                if (String.Compare(text, myComputerName, true) == 0)
                    return 3;
                if (String.Compare(type, systemFolderName, true) == 0)
                {
                    if (!isBrowsable)
                        return 4;
                    else
                        return 5;
                }
                if (!isBrowsable)
                    return 6;
                return 7;
            }
            else
            {
                if (String.Compare(type, systemFolderName, true) == 0)
                    return 8;
                return 9;
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Method to release allocated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take this object off the finalization queue to prevent
            // finalization code for this object from executing a second time
            GC.SuppressFinalize(this);
            if (subFolders != null)
            {
                foreach (ShellItem item in subFolders)
                    item.Dispose();
                subFolders = null;
            }
        }

        /// <summary>
        /// Deallocates resources.
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing) { }
                if (folder != null)
                {
                    try
                    {
                        Marshal.ReleaseComObject(folder);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    finally
                    {
                        folder = null;
                    }
                }
                if (relPidl != IntPtr.Zero)
                {
                    ShellAPI.ILFree(relPidl);
                    relPidl = IntPtr.Zero;
                }
                if (fullPidl != IntPtr.Zero)
                {
                    ShellAPI.ILFree(fullPidl);
                    fullPidl = IntPtr.Zero;
                }
            }
        }

        #endregion

        #region IEquatable<ShellItem>

        /// <summary>
        /// Determines whether the specified ShellItem 
        /// is equal to the current instance.
        /// </summary>
        /// <param name="other">
        /// The ShellItem to compare with the current instance.
        /// </param>
        /// <returns>
        /// true if the specified ShellItem is equal to the current instance; 
        /// otherwise, false.
        /// </returns>
        public bool Equals(ShellItem other)
        {
            return (this.CompareTo(other) == 0);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a String that represents the current ShellItem.
        /// </summary>
        /// <returns>
        /// A String that represents the current ShellItem.
        /// </returns>
        public override String ToString()
        {
            return text;
        }

        /// <summary>
        /// Serves as a hash function for a particular ShellItem.
        /// </summary>
        /// <returns>
        /// A hash code for the current ShellItem.
        /// </returns>
        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        #endregion
    }
}
