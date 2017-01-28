using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using VirtualDrive.FileSystem.FAT32;
using FileExplorer.Shell;

namespace VirtualDrive.Shell
{
    public class VirtualItem : IComparable<VirtualItem>, IEquatable<VirtualItem>
    {
        #region Fields

        private VirtualItem parent;
        private String text, alias, type, path;

        private int imageIndex;

        private bool isRoot;
        private bool isFolder;
        private bool isHidden;

        private DateTime modifiedDate;

        private uint firstCluster;
        private uint size;

        private List<VirtualItem> subFolders;
        private List<VirtualItem> subFiles;

        private int sortFlag;

        private Disk disk;

        #endregion

        #region Constructors

        public VirtualItem(Disk disk)
        {
            this.disk = disk;
            imageIndex = 8;
            text = "Disco Virtual (V:)";
            alias = "V:\\";
            path = "V:\\";
            isRoot = true;
            isFolder = true;
            firstCluster = disk.RootCluster;
        }

        public VirtualItem(Disk disk, VirtualItem _parent, String longName, DirectoryEntry entry)
        {
            this.disk = disk;
            parent = _parent;
            imageIndex = -1;
            isFolder = entry.IsFolder;
            isRoot = false;
            isHidden = entry.IsHidden;
            text = longName;
            alias = entry.Name;
            modifiedDate = entry.ModifiedDate;
            firstCluster = entry.FirstCluster;
            StringBuilder sb = new StringBuilder(parent.path);
            if (!parent.isRoot)
                sb.Append("\\");
            sb.Append(text);
            path = sb.ToString();
            if (isFolder)
            {
                imageIndex = 3;
                type = "Carpeta de archivos";
            }
            else
            {
                sortFlag = 1;
                size = entry.FileSize;
                if (!text.Contains("."))
                {
                    imageIndex = 0;
                    type = "Archivo";
                }
            }
        }

        #endregion

        #region Properties

        public bool IsRoot { get { return isRoot; } }

        public bool IsFolder { get { return isFolder; } }

        public bool IsHidden { get { return isHidden; } }

        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        public String Path
        {
            get { return path; }
            set { path = value; }
        }

        public String Type
        {
            get
            {
                if (type == null)
                {
                    ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
                    ShellAPI.SHGetFileInfo(DirectoryEntry.Extension(text),
                                           ShellAPI.FILE_ATTRIBUTE.NORMAL,
                                           ref info,
                                           ShellAPI.cbFileInfo,
                                           ShellAPI.SHGFI.USEFILEATTRIBUTES |
                                           ShellAPI.SHGFI.TYPENAME);
                    type = info.szTypeName;
                }
                return type;
            }
        }

        public int ImageIndex
        {
            get
            {
                if (imageIndex < 0)
                {
                    ShellAPI.SHFILEINFO info = new ShellAPI.SHFILEINFO();
                    ShellAPI.SHGetFileInfo(DirectoryEntry.Extension(text),
                                           ShellAPI.FILE_ATTRIBUTE.NORMAL,
                                           ref info,
                                           ShellAPI.cbFileInfo,
                                           ShellAPI.SHGFI.USEFILEATTRIBUTES |
                                           ShellAPI.SHGFI.SYSICONINDEX);
                    imageIndex = info.iIcon;
                }
                return imageIndex;
            }
        }

        public DateTime ModifiedDate { get { return modifiedDate; } }

        public uint FirstCluster { get { return firstCluster; } }

        public uint Size { get { return size; } }

        public String ToolTip
        {
            get
            {
                long itemSize;
                if (isFolder)
                {
                    itemSize = disk.EntrySize(firstCluster);
                }
                else
                    itemSize = size;
                StringBuilder sb = new StringBuilder("Alias: ");
                sb.AppendLine(alias);
                sb.Append("Tipo: ");
                sb.AppendLine(type);
                sb.Append("Tamaño: ");
                if (itemSize > 1048576)
                    sb.AppendFormat("{0:#,###} MB", itemSize >> 20);
                else if (itemSize > 1024)
                    sb.AppendFormat("{0:#,###} KB", itemSize >> 10);
                else
                    sb.AppendFormat("{0:##0}  B", itemSize);
                return sb.ToString();
            }
        }

        #endregion

        #region Public Methods

        public List<VirtualItem> GetFolders(bool refresh)
        {
            if (isFolder)
            {
                if (subFolders == null || refresh)
                {
                    try
                    {
                        subFolders = GetSubDirectories();
                    }
                    catch (Exception ex)
                    {
                        subFolders = new List<VirtualItem>();
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return subFolders;
            }
            else
                return new List<VirtualItem>();
        }

        public List<VirtualItem> GetItems(bool refreshFolders, bool refreshFiles)
        {
            List<VirtualItem> result = new List<VirtualItem>();
            if (isFolder)
            {
                if (subFolders == null || refreshFolders)
                {
                    try
                    {
                        subFolders = GetSubDirectories();
                    }
                    catch (Exception ex)
                    {
                        subFolders = new List<VirtualItem>();
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                result.AddRange(subFolders);
                if (subFiles == null || refreshFiles)
                {
                    try
                    {
                        subFiles = GetSubFiles();
                    }
                    catch (Exception ex)
                    {
                        subFiles = new List<VirtualItem>();
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                result.AddRange(subFiles);
            }
            return result;
        }

        public bool IsAncestorOf(VirtualItem item, bool inmediate)
        {
            if (item.isRoot)
                return false;
            if (isRoot)
                return true;
            if (inmediate)
                return this.Equals(item.parent);
            return !parent.Equals(item.parent) && item.path.Contains(path);
        }

        #endregion

        #region Private Methods

        private List<VirtualItem> GetSubDirectories()
        {
            List<DirectoryEntry> dirs = disk.GetDirectoryContents(firstCluster);
            List<VirtualItem> result = new List<VirtualItem>();
            String name;
            int i = 0;
            while (i < dirs.Count)
            {
                DirectoryEntry entry = dirs[i];
                if (entry.Type == ENTRYTYPE.SFN && entry.IsFolder)
                {
                    name = DirectoryEntry.ExtractLongName(dirs, ref i);
                    if (name != String.Empty)
                        result.Add(new VirtualItem(disk, this, name, entry));
                    else
                        result.Add(new VirtualItem(disk, this, entry.Name, entry));
                }
                else
                    i++;
            }
            return result;
        }

        private List<VirtualItem> GetSubFiles()
        {
            List<DirectoryEntry> dirs = disk.GetDirectoryContents(firstCluster);
            List<VirtualItem> result = new List<VirtualItem>();
            String name;
            int i = 0;
            while (i < dirs.Count)
            {
                DirectoryEntry entry = dirs[i];
                if (entry.Type == ENTRYTYPE.SFN && !entry.IsFolder)
                {
                    name = DirectoryEntry.ExtractLongName(dirs, ref i);
                    if (name != String.Empty)
                        result.Add(new VirtualItem(disk, this, name, entry));
                    else
                        result.Add(new VirtualItem(disk, this, entry.Name, entry));
                }
                else
                    i++;
            }
            return result;
        }

        #endregion

        #region IComparable<VirtualItem>

        public int CompareTo(VirtualItem other)
        {
            if (other == null)
                return 1;
            int cmp = other.sortFlag - sortFlag;
            if (cmp != 0)
                return cmp;
            return String.Compare(other.text, text, true);
        }

        #endregion

        #region IEquatable<VirtualItem>

        public bool Equals(VirtualItem other)
        {
            return (CompareTo(other) == 0);
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return text;
        }

        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        #endregion
    }
}
