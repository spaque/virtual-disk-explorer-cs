using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VirtualDrive.FileSystem.FAT32
{
    [Flags]
    public enum OPENMODE : uint
    {
        NONE = 0x00,
        READ = 0x01,
        WRITE = 0x02,
        CREATE = 0x04,
        TRUNCATE = 0x08,
        APPEND = 0x10
    }

    public class VirtualFile
    {
        #region Fields

        private Disk disk;
        private DirectoryEntry entry;
        private long entryAbsoluteOffset;
        private uint fileOffset;
        private OPENMODE mode;

        #endregion

        #region Constructors

        public VirtualFile(Disk disk, String path)
        {
            if (!path.StartsWith("V:", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            this.disk = disk;
            fileOffset = 0;
            mode = OPENMODE.READ;
            entry = disk.FindEntryAbsolute(path, out entryAbsoluteOffset);
        }

        public VirtualFile(Disk disk, String path, OPENMODE _mode)
        {
            if (!path.StartsWith("V:", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (_mode == OPENMODE.NONE)
                throw new ArgumentException("Modo de apertura invalido");
            if (((_mode & OPENMODE.WRITE) == 0) && ((_mode & OPENMODE.CREATE) != 0))
                throw new ArgumentException("Modo de apertura invalido");
            if (((_mode & OPENMODE.WRITE) == 0) && ((_mode & OPENMODE.TRUNCATE) != 0))
                throw new ArgumentException("Modo de apertura invalido");
            if (((_mode & OPENMODE.WRITE) == 0) && ((_mode & OPENMODE.APPEND) != 0))
                throw new ArgumentException("Modo de apertura invalido");
            if (((_mode & OPENMODE.APPEND) != 0) && ((_mode & OPENMODE.TRUNCATE) != 0))
                throw new ArgumentException("Modo de apertura invalido");
            this.disk = disk;
            mode = _mode;
            if ((mode & OPENMODE.CREATE) == 0)
            {
                fileOffset = 0;
                entry = disk.FindEntryAbsolute(path, out entryAbsoluteOffset);
                if ((mode & OPENMODE.APPEND) != 0)
                    fileOffset = entry.FileSize;
                else if ((mode & OPENMODE.TRUNCATE) != 0)
                    disk.TruncateFile(entry, entryAbsoluteOffset);
            }
            else
            {
                fileOffset = 0;
                bool created;
                entry = disk.CreateEntry(path, ATTRIBUTE.ARCHIVE, 0, out entryAbsoluteOffset, out created);
                if (!created)
                    throw new ArgumentException(String.Format("{0} ya existe", path));
            }
        }

        public VirtualFile(Disk disk, String path, OPENMODE _mode, uint size)
        {
            if (_mode != (OPENMODE.CREATE | OPENMODE.WRITE))
                throw new ArgumentException("Invalid open mode");
            this.disk = disk;
            mode = _mode;
            bool created;
            entry = disk.CreateEntry(path, ATTRIBUTE.ARCHIVE, size, out entryAbsoluteOffset, out created);
            if (!created)
                throw new ArgumentException(String.Format("{0} ya existe", path));
        }

        #endregion

        #region Public Methods

        public int Read(byte[] buffer, int index, int count)
        {
            if (count + index > buffer.Length)
                throw new ArgumentException("El tamaño de buffer es insuficiente");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index es menor que cero");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count es menor que cero");
            if ((mode & OPENMODE.READ) == 0)
                throw new InvalidOperationException("El archivo fue abierto en modo escritura");
            if (fileOffset >= entry.FileSize)
                return -1;
            int read = count;
            if (fileOffset + count > entry.FileSize)
                read = (int)(entry.FileSize - fileOffset);
            disk.ReadDisk(buffer, index, read, entry.FirstCluster, fileOffset);
            fileOffset += (uint)read;
            return read;
        }

        public int Write(byte[] buffer, int index, int count)
        {
            if (count + index > buffer.Length)
                throw new ArgumentException("El tamaño de buffer es insuficiente");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index es menor que cero");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count es menor que cero");
            if ((mode & OPENMODE.WRITE) == 0)
                throw new InvalidOperationException("El archivo fue abierto en modo lectura");
            if (fileOffset + count > entry.FileSize)
            {
                entry.FileSize += (uint)(fileOffset + count - entry.FileSize);
                disk.UpdateDirectoryEntry(entry, entryAbsoluteOffset);
            }
            disk.WriteDisk(buffer, index, count, entry.FirstCluster, fileOffset);
            fileOffset += (uint)count;
            return count;
        }

        #endregion
    }
}
