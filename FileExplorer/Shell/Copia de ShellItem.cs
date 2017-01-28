using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using FileExplorer.Shell.Interfaces;

namespace FileExplorer.Shell
{
    internal sealed class ShellItem : IComparable, IEnumerable, IDisposable
    {
        private ShellExplorer explorer;
        private ShellItem parent;
        private IShellFolder folder;
        private IntPtr folderPtr;
        private ShellItemList subFiles, subFolders;
        private PIDL relPidl;
        private string text, path, type;
        private int imageIndex, selectedImageIndex;

        private bool isFolder, isLink, isShared, isFileSystem, isHidden,
                       hasSubFolder, isExplorable, isDisk, filesExpanded,
                       foldersExpanded, canRename, canRead, updateFolder;
        private bool disposed = false;

        #region Constructors

        /// <summary>
        /// Constructor for the desktop shell item.
        /// </summary>
        /// <param name="explorer">
        /// Explorer class that uses the shell item.
        /// </param>
        /// <param name="pidl">
        /// Pointer to the desktop PIDL.
        /// </param>
        /// <param name="folderPtr">
        /// Pointer to the IShellFolder interface for the desktop. 
        /// </param>
        internal ShellItem(ShellExplorer explorer, IntPtr pidl, IntPtr folderPtr)
        {
            this.explorer = explorer;
            parent = null;
            this.folderPtr = folderPtr;
            // get managed object
            this.folder = 
                (IShellFolder)Marshal.GetTypedObjectForIUnknown(folderPtr, typeof(IShellFolder));
            subFiles = new ShellItemList(this);
            subFolders = new ShellItemList(this);
            relPidl = new PIDL(pidl, false);

            text = "Desktop";
            path = "Desktop";
            SetDesktopAttributes();

            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGetFileInfo(relPidl.Ptr,
                                   0,
                                   ref info,
                                   ShellAPI.cbFileInfo,
                                   ShellAPI.SHGFI.PIDL |
                                   ShellAPI.SHGFI.TYPENAME |
                                   ShellAPI.SHGFI.SYSICONINDEX);
            type = info.szTypeName;

            ShellImageList.SetIconIndex(this, info.iIcon, false);
            ShellImageList.SetIconIndex(this, info.iIcon, true);
        }

        /// <summary>
        /// Constructor for a folder item.
        /// </summary>
        /// <param name="explorer">
        /// Explorer class that uses the shell item.
        /// </param>
        /// <param name="parent">
        /// Parent shell item for this item.
        /// </param>
        /// <param name="pidl">
        /// Pointer to the folder PIDL.
        /// </param>
        /// <param name="folderPtr">
        /// Pointer to the IShellFolder interface for the folder.
        /// </param>
        internal ShellItem(ShellExplorer explorer, ShellItem parent, IntPtr pidl, IntPtr folderPtr)
        {
            this.explorer = explorer;
            this.parent = parent;
            this.folderPtr = folderPtr;
            folder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(folderPtr, typeof(IShellFolder));
            subFiles = new ShellItemList(this);
            subFolders = new ShellItemList(this);
            relPidl = new PIDL(pidl, false);
            SetText();
            SetPath();
            SetInfo();
            SetFolderAttributes();
        }

        /// <summary>
        /// Constructor for a file item.
        /// </summary>
        /// <param name="explorer">
        /// Explorer class that uses the shell item.
        /// </param>
        /// <param name="parent">
        /// Parent shell item for this item.
        /// </param>
        /// <param name="pidl">
        /// Pointer to the file PIDL.
        /// </param>
        internal ShellItem(ShellExplorer explorer, ShellItem parent, IntPtr pidl)
        {
            this.explorer = explorer;
            this.parent = parent;
            folderPtr = IntPtr.Zero;
            folder = null;
            subFiles = null;
            subFolders = null;
            relPidl = new PIDL(pidl, false);
            SetText();
            SetPath();
            SetInfo();
            SetFileAttributes();
        }

        #endregion

        #region Properties

        internal ShellExplorer Explorer { get { return explorer; } }

        internal ShellItem Parent { get { return parent; } }

        internal IShellFolder Folder {
            get {
                if (updateFolder)
                {
                    Marshal.ReleaseComObject(folder);
                    Marshal.Release(folderPtr);
                    ShellAPI.HRESULT hr;
                    hr = parent.Folder.BindToObject(relPidl.Ptr,
                                                    IntPtr.Zero,
                                                    ref ShellAPI.IID_IShellFolder,
                                                    out folderPtr);
                    if (hr == ShellAPI.HRESULT.S_OK)
                        folder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(folderPtr, typeof(IShellFolder));
                    updateFolder = false;
                }
                return folder;
            }
        }

        internal ShellItemList SubFiles { get { return subFiles; } }

        internal ShellItemList SubFolders { get { return subFolders; } }

        internal PIDL RelPidl { get { return relPidl; } }

        internal PIDL FullPidl {
            get {
                PIDL pidl = new PIDL(relPidl, true);
                ShellItem current = parent;
                while (current != null)
                {
                    pidl.Insert(current.relPidl.Ptr);
                    current = current.parent;
                }
                return pidl;
            }
        }

        public string Text { get { return text; } }

        public string Path { get { return path; } }

        public string Type { get { return type; } }

        internal int ImageIndex {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        internal int SelectedImageIndex {
            get { return selectedImageIndex; }
            set { selectedImageIndex = value; }
        }

        public bool IsFolder { get { return isFolder; } }

        public bool IsLink { get { return isLink; } }

        public bool IsShared { get { return isShared; } }

        public bool IsFileSystem { get { return isFileSystem; } }

        public bool IsHidden { get { return isHidden; } }

        public bool HasSubFolder { get { return hasSubFolder; } }

        public bool IsExplorable { get { return isExplorable; } }

        public bool IsDisk { get { return isDisk; } }

        public bool FilesExpanded { get { return filesExpanded; } }

        public bool FoldersExpanded { get { return foldersExpanded; } }

        public bool CanRename { get { return canRename; } }

        public bool CanRead { get { return canRead; } }

        public bool UpdateFolder { 
            get { return updateFolder; }
            set { updateFolder = value; }
        }

        #endregion

        #region Init Properties

        /// <summary>
        /// Retrives the name of the ShellItem.
        /// </summary>
        private void SetText()
        {
            // reserve memory for an unmanaged STRRET structure
            // uType -> 4 bytes
            // union -> MAX_PATH * 2
            IntPtr strret = Marshal.AllocCoTaskMem((ShellAPI.MAX_PATH << 1) + 4);
            // set strret to 0
            Marshal.WriteInt32(strret, 0, 0);
            StringBuilder buf = new StringBuilder(ShellAPI.MAX_PATH);
            ShellAPI.HRESULT hr;

            hr = parent.folder.GetDisplayNameOf(relPidl.Ptr,
                                                ShellAPI.SHGNO.INFOLDER,
                                                strret);
            if (hr == ShellAPI.HRESULT.S_OK)
            {
                ShellAPI.StrRetToBuf(strret, relPidl.Ptr, buf, ShellAPI.MAX_PATH);
                text = buf.ToString();
            }
            Marshal.FreeCoTaskMem(strret);
        }

        /// <summary>
        /// Retrives the path of the ShellItem.
        /// </summary>
        private void SetPath()
        {
            // reserve memory for an unmanaged STRRET structure
            // uType -> 4 bytes
            // union -> MAX_PATH * 2
            IntPtr strret = Marshal.AllocCoTaskMem((ShellAPI.MAX_PATH << 1) + 4);
            // set strret to 0
            Marshal.WriteInt32(strret, 0, 0);
            StringBuilder buf = new StringBuilder(ShellAPI.MAX_PATH);
            ShellAPI.HRESULT hr;

            hr = parent.folder.GetDisplayNameOf(
                    relPidl.Ptr,
                    ShellAPI.SHGNO.FORADDRESSBAR | ShellAPI.SHGNO.FORPARSING,
                    strret);
            if (hr == ShellAPI.HRESULT.S_OK)
            {
                ShellAPI.StrRetToBuf(strret, relPidl.Ptr, buf, ShellAPI.MAX_PATH);
                text = buf.ToString();
            }
            Marshal.FreeCoTaskMem(strret);
        }

        /// <summary>
        /// Retrieve the string that describes the file's type.
        /// </summary>
        private void SetInfo()
        {
            PIDL fullPidl = FullPidl;
            ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();

            ShellAPI.SHGetFileInfo(fullPidl.Ptr,
                                   0,
                                   ref info,
                                   ShellAPI.cbFileInfo,
                                   ShellAPI.SHGFI.PIDL |
                                   ShellAPI.SHGFI.TYPENAME |
                                   ShellAPI.SHGFI.SYSICONINDEX);
            fullPidl.Free();
            type = info.szTypeName;
            ShellImageList.SetIconIndex(this, info.iIcon, false);
            ShellImageList.SetIconIndex(this, info.iIcon, true);
        }

        /// <summary>
        /// Sets the attributes for the desktop item.
        /// </summary>
        private void SetDesktopAttributes()
        {
            isFolder = true;
            isLink = false;
            isShared = false;
            isFileSystem = true;
            isHidden = false;
            hasSubFolder = true;
            isExplorable = true;
            canRename = false;
            canRead = true;
        }

        /// <summary>
        /// Sets the attributes of a folder ShellItem.
        /// </summary>
        private void SetFolderAttributes()
        {
            // folder attributes to retrieve with GetAttributesOf
            ShellAPI.SFGAO attr = ShellAPI.SFGAO.SHARE |
                                  ShellAPI.SFGAO.FILESYSTEM |
                                  ShellAPI.SFGAO.HIDDEN |
                                  ShellAPI.SFGAO.HASSUBFOLDER |
                                  ShellAPI.SFGAO.BROWSABLE |
                                  ShellAPI.SFGAO.CANRENAME |
                                  ShellAPI.SFGAO.STORAGE;
            parent.folder.GetAttributesOf(1,
                                          new IntPtr[] { relPidl.Ptr },
                                          ref attr);
            isFolder = true;
            isLink = false;
            isShared = (attr & ShellAPI.SFGAO.SHARE) != 0;
            isFileSystem = (attr & ShellAPI.SFGAO.FILESYSTEM) != 0;
            isHidden = (attr & ShellAPI.SFGAO.HIDDEN) != 0;
            hasSubFolder = (attr & ShellAPI.SFGAO.HASSUBFOLDER) != 0;
            isExplorable = (attr & ShellAPI.SFGAO.BROWSABLE) != 0;
            canRename = (attr & ShellAPI.SFGAO.CANRENAME) != 0;
            canRead = (attr & ShellAPI.SFGAO.STORAGE) != 0;
            isDisk = (path.Length == 4) && (path.EndsWith(":\\"));
        }

        /// <summary>
        /// Sets the attributes for a file ShellItem
        /// </summary>
        private void SetFileAttributes()
        {
            // file attributes to retrieve with GetAttributesOf
            ShellAPI.SFGAO attr = ShellAPI.SFGAO.LINK |
                                  ShellAPI.SFGAO.SHARE |
                                  ShellAPI.SFGAO.FILESYSTEM |
                                  ShellAPI.SFGAO.HIDDEN |
                                  ShellAPI.SFGAO.CANRENAME |
                                  ShellAPI.SFGAO.STREAM;
            parent.folder.GetAttributesOf(1,
                                          new IntPtr[] { relPidl.Ptr },
                                          ref attr);
            isFolder = false;
            isLink = (attr & ShellAPI.SFGAO.LINK) != 0;
            isShared = (attr & ShellAPI.SFGAO.SHARE) != 0;
            isFileSystem = (attr & ShellAPI.SFGAO.FILESYSTEM) != 0;
            isHidden = (attr & ShellAPI.SFGAO.HIDDEN) != 0;
            hasSubFolder = false;
            isExplorable = false;
            canRename = (attr & ShellAPI.SFGAO.CANRENAME) != 0;
            canRead = (attr & ShellAPI.SFGAO.STREAM) != 0;
            isDisk = false;
        }

        #endregion
    }
}
