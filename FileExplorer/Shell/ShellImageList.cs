using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FileExplorer.Shell
{
    public sealed class ShellImageList
    {
        #region Fields

        // we use a singleton pattern to allow just one global instance of the class
        private static ShellImageList instance = null;
        private static readonly object padlock = new object();

        private IntPtr smallImageListHandle, largeImageListHandle;
        private Hashtable imageTable;

        private const int SIL_SMALL = 1;
        private const int SIL_NORMAL = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to get the system ImageList.
        /// </summary>
        private ShellImageList()
        {
            imageTable = new Hashtable();
            bool res;

            res = ShellAPI.FileIconInit(true);

            ShellAPI.SHGFI uFlags = ShellAPI.SHGFI.USEFILEATTRIBUTES |
                                    ShellAPI.SHGFI.SYSICONINDEX |
                                    ShellAPI.SHGFI.SMALLICON;
            ShellAPI.SHFILEINFO sfiSmall = new ShellAPI.SHFILEINFO();
            // get a handle to the system image list for small icons
            smallImageListHandle = 
                ShellAPI.SHGetFileInfo(
                    ".txt", 
                    ShellAPI.FILE_ATTRIBUTE.NORMAL, 
                    ref sfiSmall, 
                    ShellAPI.cbFileInfo, 
                    uFlags);

            uFlags = ShellAPI.SHGFI.USEFILEATTRIBUTES |
                     ShellAPI.SHGFI.SYSICONINDEX |
                     ShellAPI.SHGFI.LARGEICON;
            ShellAPI.SHFILEINFO sfiLarge = new ShellAPI.SHFILEINFO();
            // get a handle to the system image list for large icons
            largeImageListHandle =
                ShellAPI.SHGetFileInfo(
                    ".txt",
                    ShellAPI.FILE_ATTRIBUTE.NORMAL,
                    ref sfiLarge,
                    ShellAPI.cbFileInfo,
                    uFlags);
        }

        #endregion

        #region Properties

        public static ShellImageList Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new ShellImageList();
                    return instance;
                }
            }
        }

        public IntPtr SmallImageList { get { return smallImageListHandle; } }

        public IntPtr LargeImageList { get { return largeImageListHandle; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the icon index for the given ShellItem.
        /// </summary>
        /// <param name="item">
        /// ShellItem for which the icon index is calculated.
        /// </param>
        /// <param name="selected">
        /// True if the item is selected, false otherwise.
        /// </param>
        public int GetIconIndex(ShellItem item, bool selected)
        {
            bool hasOverlay = false;
            int index = item.ImageIndex;
            int res;
            ShellAPI.FILE_ATTRIBUTE dwFileAttrib = 0;
            ShellAPI.SHGFI uFlags = ShellAPI.SHGFI.ICON |
                                    ShellAPI.SHGFI.PIDL |
                                    ShellAPI.SHGFI.SYSICONINDEX;
            int key = item.ImageIndex << 8;
            // check for overlays
            if (item.IsLink)
            {
                key = key | 1;
                uFlags = uFlags | ShellAPI.SHGFI.LINKOVERLAY;
                hasOverlay = true;
            }
            if (item.IsShared)
            {
                key = key | 2;
                uFlags = uFlags | ShellAPI.SHGFI.ADDOVERLAYS;
                hasOverlay = true;
            }
            // not really an overlay, but handled the same
            if (selected)
            {
                key = key | 4;
                uFlags = uFlags | ShellAPI.SHGFI.OPENICON;
                hasOverlay = true;
            }

            if (imageTable.ContainsKey(key)) {
                res = (int)imageTable[key];
            } // for non-overlay icons we already have the index            
            else if (!hasOverlay && !item.IsHidden)
            {
                res = index;
                imageTable[key] = index;
            } // don't have icon index for an overlay            
            else
            {
                if (item.IsFileSystem && !item.IsDisk && !item.IsFolder)
                {
                    uFlags |= ShellAPI.SHGFI.USEFILEATTRIBUTES;
                    dwFileAttrib = ShellAPI.FILE_ATTRIBUTE.NORMAL;
                }
                ShellAPI.SHFILEINFO sfiSmall = new ShellAPI.SHFILEINFO();
                ShellAPI.SHGetFileInfo(item.FullPidl, dwFileAttrib, ref sfiSmall, ShellAPI.cbFileInfo, uFlags | ShellAPI.SHGFI.SMALLICON);

                ShellAPI.SHFILEINFO sfiLarge = new ShellAPI.SHFILEINFO();
                ShellAPI.SHGetFileInfo(item.FullPidl, dwFileAttrib, ref sfiLarge, ShellAPI.cbFileInfo, uFlags | ShellAPI.SHGFI.LARGEICON);

                lock (padlock)
                {
                    // add overlaid icon to the image list
                    res = ShellAPI.ImageList_ReplaceIcon(smallImageListHandle, -1, sfiSmall.hIcon);
                    ShellAPI.ImageList_ReplaceIcon(largeImageListHandle, -1, sfiLarge.hIcon);
                }

                ShellAPI.DestroyIcon(sfiSmall.hIcon);
                ShellAPI.DestroyIcon(sfiLarge.hIcon);

                imageTable[key] = res;
            }
            return res;
        }

        /// <summary>
        /// Returns a GDI+ copy of the icon from the 
        /// ImageList at the specified index.
        /// </summary>
        /// <param name="index">
        /// Index of the image.
        /// </param>
        /// <param name="small">
        /// True to get a small size Icon or false otherwise.
        /// </param>
        /// <returns>
        /// The specified Icon if successful or null otherwise.
        /// </returns>
        public Icon GetIcon(int index, bool small)
        {
            IntPtr iconPtr;
            if (small)
                iconPtr = ShellAPI.ImageList_GetIcon(smallImageListHandle, index, ShellAPI.ILD.NORMAL);
            else
                iconPtr = ShellAPI.ImageList_GetIcon(largeImageListHandle, index, ShellAPI.ILD.NORMAL);

            if (iconPtr != IntPtr.Zero)
            {
                Icon icon = Icon.FromHandle(iconPtr);
                Icon res = (Icon)icon.Clone();
                ShellAPI.DestroyIcon(iconPtr);
                return res;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Associates a small image list with a TreeView control.
        /// </summary>
        /// <param name="tv">
        /// TreeView to associate the image list.
        /// </param>
        public void SetSmallImageList(TreeView tv)
        {
            ShellAPI.SendMessage(
                tv.Handle, 
                ShellAPI.WM.TVM_SETIMAGELIST, 
                SIL_NORMAL, 
                smallImageListHandle);
        }

        /// <summary>
        /// Associates a small image list with a ListView control.
        /// </summary>
        /// <param name="lv">
        /// ListView to associate the image list.
        /// </param>
        public void SetSmallImageList(ListView lv)
        {
            ShellAPI.SendMessage(
                lv.Handle,
                ShellAPI.WM.LVM_SETIMAGELIST,
                SIL_SMALL,
                smallImageListHandle);
        }

        /// <summary>
        /// Associates a large image list with a ListView control.
        /// </summary>
        /// <param name="lv">
        /// ListView to associate the image list.
        /// </param>
        public void SetLargeImageList(ListView lv)
        {
            ShellAPI.SendMessage(
                lv.Handle,
                ShellAPI.WM.LVM_SETIMAGELIST,
                SIL_NORMAL,
                largeImageListHandle);
        }

        #endregion
    }
}
