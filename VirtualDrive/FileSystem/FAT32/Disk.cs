using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using VirtualDrive.Interfaces;
using VirtualDrive.Shell;

namespace VirtualDrive.FileSystem.FAT32
{
    public sealed partial class Disk : IControl, IData, ISystem
    {
        #region Fields

        private static readonly object padlock = new object();

        private static uint diskSize = 33554432;
        private static int MAXFD = 20;

        private BootSector bs;
        private FAT fat;
        private FatFileSystemInfo fsi;
        private String path;
        private bool mounted;
        private bool formatted;
        private bool busy;

        private uint rootCluster;

        private VirtualItem currentDir;

        private VirtualFile[] fileTable;

        private long dataStartOffset;

        private System.Windows.Forms.Timer timer;

        private ulong bytesWritten;
        private ulong bytesRead;
        private uint writes;
        private uint reads;
        private ulong lastCount;
        private float transferRate;
        private float maxRateAllowed;
        private uint filesOpened;

        #endregion

        #region Constructor

        public Disk(String _path)
        {
            path = _path;
            if (!CheckDisk())
                throw new InvalidDataException(String.Format("\"{0}\" no contiene un disco valido", path));
            bs = new BootSector();
            fsi = new FatFileSystemInfo();
            fat = new FAT(bs, fsi);
            dataStartOffset = (bs.ReservedSectors + (bs.SectorsPerFAT << 1)) * bs.BytesPerSector;
            fileTable = new VirtualFile[MAXFD];

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 500;
            timer.Tick += new EventHandler(timer_Tick);

            maxRateAllowed = 80.0F;
        }

        ~Disk()
        {
            if (fat != null && fat.Modified)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    stream.Seek(bs.FSISector * bs.BytesPerSector, SeekOrigin.Begin);
                    fsi.WriteFileSystemInfo(stream);
                    stream.Seek(bs.ReservedSectors * bs.BytesPerSector, SeekOrigin.Begin);
                    fat.WriteFAT(stream, false);
                    fat.WriteFAT(stream, false);
                }
            }
        }

        #endregion

        #region Properties

        public static uint DiskSize { get { return diskSize; } }

        public bool Mounted { get { return mounted; } }

        public bool Formatted { get { return formatted; } }

        public uint RootCluster { get { return rootCluster; } }

        internal VirtualItem CurrentDirectory
        {
            get { return currentDir; }
            set { currentDir = value; }
        }

        public bool Busy
        {
            get {
                lock (padlock)
                {
                    return busy;
                }
            }
            set {
                lock (padlock)
                {
                    busy = value;
                }
            }
        }

        internal BootSector BootSector { get { return bs; } }

        internal String DiskPath { get { return path; } }

        #endregion

        #region Events

        void timer_Tick(object sender, EventArgs e)
        {
            lock (padlock)
            {
                ulong bytes = bytesRead + bytesWritten;
                if (bytes == lastCount)
                    transferRate = 0;
                lastCount = bytes;
            }
        }

        #endregion

        #region Public Methods

        public String RunCommand(String cmd)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            try
            {
                String[] args = ParseCommand(cmd);
                if (String.Compare(args[0], "dir", true) == 0)
                {
                    if (args.Length == 1) return Dir(String.Empty);
                    else if (args.Length == 2)
                        return Dir(args[1]);
                    else
                        return "La sintaxis del comando no es correcta";
                }
                else if (String.Compare(args[0], "mkdir", true) == 0)
                {
                    SetPath(args);
                    if (args.Length != 2)
                        return "La sintaxis del comando no es correcta";
                    else
                    {
                        bool created = CreateDir(args[1]);
                        if (created)
                            return String.Format("{0} creado correctamente", args[1]);
                        else
                            return String.Format("{0} ya existe", args[1]);
                    }
                }
                else if (String.Compare(args[0], "rmdir", true) == 0)
                {
                    SetPath(args);
                    if (args.Length < 2 || args.Length > 3)
                        return "La sintaxis del comando no es correcta";
                    else if (args.Length >= 2)
                    {
                        bool recursive = false;
                        if (args.Length > 2)
                            recursive = String.Compare(args[2], "-f", true) == 0;
                        if (DeleteDir(args[1], recursive))
                            return String.Format("{0} borrado correctamente", args[1]);
                        else if (!recursive)
                            return String.Format("{0} no esta vacio", args[1]);
                    }
                    return String.Format("No se pudo borrar {0}", args[1]);
                }
                else if (String.Compare(args[0], "copy", true) == 0)
                {
                    SetPath(args);
                    if (args.Length != 3)
                        return "La sintaxis del comando no es correcta";
                    return Copy(args[1], args[2], false);
                }
                else if (String.Compare(args[0], "move", true) == 0)
                {
                    SetPath(args);
                    if (args.Length != 3)
                        return "La sintaxis del comando no es correcta";
                    return Move(args[1], args[2], false);
                }
                else if (String.Compare(args[0], "del", true) == 0)
                {
                    SetPath(args);
                    if (args.Length != 2)
                        return "La sintaxis del comando no es correcta";
                    if (Delete(args[1]))
                        return "Borrado completado correctamente";
                    else
                        return "Borrado no completado";
                }
                else if (String.Compare(args[0], "rename", true) == 0)
                {
                    SetPath(args);
                    if (args.Length != 3)
                        return "La sintaxis del comando no es correcta";
                    if (Rename(args[1], Path.GetFileName(args[2])))
                        return "Renombrado completado correctamente";
                    else
                        return "No se pudo renombrar";
                }
                else return String.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static String RegexString(char character)
        {
            if (character == '*') return ".*";
            else if (character == '.') return @"\.";
            else if (character == ' ') return @"\s";
            else if (character == '?') return @"\w";
            else if (character == '\\') return @"\\";
            else if (character == '-') return @"\-";
            else if (character == '(') return @"\(";
            else if (character == ')') return @"\)";
            else if (character == '[') return @"\[";
            else if (character == ']') return @"\]";
            else if (character == '+') return @"\+";
            else if (character == '$') return @"\$";
            else if (character == '^') return @"\^";
            else if (character == '{') return @"\{";
            else if (character == '}') return @"\}";
            else return character.ToString();
        }

        public static String RegexString(String str)
        {
            StringBuilder sb = new StringBuilder("^");
            char character;
            for (int i = 0; i < str.Length; i++)
            {
                character = str[i];
                if (character == '*') sb.Append(".*");
                else if (character == '.') sb.Append(@"\.");
                else if (character == ' ') sb.Append(@"\s");
                else if (character == '?') sb.Append(@"\w");
                else if (character == '\\') sb.Append(@"\\");
                else if (character == '-') sb.Append(@"\-");
                else if (character == '(') sb.Append(@"\(");
                else if (character == ')') sb.Append(@"\)");
                else if (character == '[') sb.Append(@"\[");
                else if (character == ']') sb.Append(@"\]");
                else if (character == '+') sb.Append(@"\+");
                else if (character == '$') sb.Append(@"\$");
                else if (character == '^') sb.Append(@"\^");
                else if (character == '{') sb.Append(@"\{");
                else if (character == '}') sb.Append(@"\}");
                else sb.Append(character);
            }
            sb.Append("$");
            return sb.ToString();
        }

        #endregion

        #region Private Methods

        private void SetPath(String[] args)
        {
            if (args.Length > 1)
            {
                if (!args[1].StartsWith("V:\\"))
                {
                    String path = currentDir.Path;
                    if (String.Compare(path, "V:\\", true) == 0)
                        args[1] = args[1].Insert(0, path);
                    else
                        args[1] = args[1].Insert(0, path + "\\");
                }
            }
            if (args.Length > 2 && !args[2].StartsWith("-"))
            {
                if (args[2].StartsWith("\\"))
                    args[2] = args[2].TrimStart(new char[] { '\\' });
                if (!args[2].StartsWith("V:\\"))
                {
                    String path = currentDir.Path;
                    if (String.Compare(path, "V:\\", true) == 0)
                        args[2] = args[2].Insert(0, path);
                    else
                        args[2] = args[2].Insert(0, path + "\\");
                }
            }
        }

        private String[] ParseCommand(String cmd)
        {
            String[] split = Regex.Split(cmd, "\\s|\"([^\"]*)[^\\s]*");
            List<String> args = new List<String>();
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Length > 0)
                    args.Add(split[i]);
            }
            return args.ToArray();
        }

        private String GetDestination(String src, String dest)
        {
            if (!dest.Contains("*"))
                return dest;
            return Regex.Replace(dest, @"\*", src);
        }

        #endregion

        #region IControl

        public String Dir(String pattern)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            List<DirectoryEntry> entries = GetDirectoryContents(currentDir.FirstCluster);
            StringBuilder listing = new StringBuilder();
            String name;
            DirectoryEntry entry;
            int i;
            int dirs = 0;
            int files = 0;
            uint totalSize = 0;
            Regex regex = new Regex(RegexString(pattern), RegexOptions.IgnoreCase);
            listing.AppendFormat("Directorio actual: {0}", currentDir.Path);
            listing.AppendLine();
            listing.AppendLine();
            i = 0;
            while (i < entries.Count)
            {
                entry = entries[i];
                name = DirectoryEntry.ExtractLongName(entries, ref i);
                if (pattern != String.Empty)
                {
                    // if the current entry doesn't match don't list it
                    if (name != String.Empty && !regex.IsMatch(name))
                        continue;
                    if (name == String.Empty && !regex.IsMatch(entry.Name))
                        continue;
                }
                listing.AppendFormat("{0}  ", entry.ModifiedDate.ToShortDateString());
                listing.Append(String.Format("{0}", entry.ModifiedDate.ToShortTimeString()).PadRight(9));
                if (entry.IsFolder)
                {
                    listing.Append("<DIR>              ");
                    dirs++;
                }
                else
                {
                    listing.Append(String.Format("{0:#,###} ", entry.FileSize).PadLeft(19));
                    files++;
                    totalSize += entry.FileSize;
                }
                if (name != String.Empty)
                    listing.AppendLine(name);
                else
                    listing.AppendLine(entry.Name);
            }
            listing.AppendLine();
            listing.AppendFormat(
                "\t{0} archivos {1} bytes", 
                files, String.Format("{0:#,###}", totalSize).PadLeft(16));
            listing.AppendLine();
            listing.AppendFormat("\t{0} directorios", dirs);
            uint used, free;
            Free(out used, out free);
            listing.AppendFormat("\t {0:#,###} bytes libres", free);
            return listing.ToString();
        }

        public bool CreateDir(String path)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (path == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!path.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            long offset;
            bool created;
            CreateEntry(path, ATTRIBUTE.DIRECTORY, 0, out offset, out created);
            return created;
        }

        public bool DeleteDir(String path, bool forceDelete)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (path == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!path.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (String.Compare(path, "V:\\", true) == 0)
                throw new ArgumentException("No se puede borrar el directorio raiz");
            if (path.EndsWith("\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            bool result = false;
            long offset;
            String parentPath = path.Substring(0, path.LastIndexOf('\\'));
            if (parentPath.Length == 2) parentPath = parentPath + "\\";
            String childPath = path.Substring(path.LastIndexOf('\\') + 1);
            DirectoryEntry parent = FindEntryAbsolute(parentPath, out offset);
            DirectoryEntry entry = FindEntryRelative(childPath, parent.FirstCluster, out offset);
            result = DeleteEntry(parent, entry, forceDelete);
            return result;
        }

        public bool Delete(String path)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (path == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!path.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            String directory = Path.GetDirectoryName(path);
            String filename = Path.GetFileName(path);
            long offset;
            bool deleted = false;
            DirectoryEntry parent = FindEntryAbsolute(directory, out offset);
            DirectoryEntry entry;
            Dictionary<String, DirectoryEntry> table = DirectoryLookupTable(parent.FirstCluster, false);
            Regex regex = new Regex(RegexString(filename));
            foreach (String str in table.Keys)
            {
                if (regex.IsMatch(str))
                {
                    table.TryGetValue(str, out entry);
                    if (entry.IsArchive)
                    {
                        if (DeleteEntry(parent, entry, false))
                            deleted = true;
                        else
                            return false;
                    }
                }
            }
            return deleted;
        }

        public String Copy(String src, String dest, bool folders)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (src == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (dest == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!src.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!dest.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (String.Compare(src, dest, true) == 0)
                throw new ArgumentException("Origen y destino no pueden ser iguales");
            if (String.Compare(src, "V:\\") == 0)
                throw new ArgumentException("No se puede copiar el directorio raiz");
            if (dest.Contains("?"))
                throw new ArgumentException("No se permite el comodin ? en el destino");

            String directory = Path.GetDirectoryName(src);
            Dictionary<String, DirectoryEntry> table = GetDirectoryTable(directory);
            DirectoryEntry entry;
            Regex regex = new Regex(RegexString(src));
            List<String> fileList = new List<String>();
            int count = 0;
            StringBuilder sb = new StringBuilder();

            if (!directory.EndsWith("\\")) directory += "\\";
            foreach (String str in table.Keys)
            {
                if (regex.IsMatch(directory + str))
                {
                    table.TryGetValue(str, out entry);
                    if (entry.IsArchive)
                        fileList.Add(str);
                    else if (folders && entry.IsFolder)
                        fileList.Add(str);
                }
            }
            if (fileList.Count > 0)
            {
                String destination;
                if (fileList.Count > 1 && !Path.GetFileName(dest).StartsWith("*"))
                    throw new ArgumentException("Sintaxis incorrecta");
                for (int i = 0; i < fileList.Count; i++)
                {
                    table.TryGetValue(fileList[i], out entry);
                    try
                    {
                        destination = GetDestination(fileList[i], dest);
                        if (entry.IsArchive)
                            CopyArchive(entry, destination);
                        else if (entry.IsFolder)
                            CopyFolder(entry, destination);
                        sb.Append(fileList[i]);
                        sb.Append(" --> ");
                        sb.AppendLine(destination.Replace(directory, ""));
                        count++;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(ex.Message);
                    }
                }
                sb.AppendFormat("\t{0} archivos copiados", count);
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("No se encontro el archivo especificado.");
                sb.AppendLine("\t0 archivos copiados.");
            }
            return sb.ToString();
        }

        public String Move(String src, String dest, bool folders)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (src == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (dest == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!src.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!dest.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (String.Compare(src, dest, true) == 0)
                throw new ArgumentException("Origen y destino no pueden ser iguales");
            if (String.Compare(src, "V:\\") == 0)
                throw new ArgumentException("No se puede mover el directorio raiz");

            String directory = Path.GetDirectoryName(src);
            long offset;
            DirectoryEntry parent = FindEntryAbsolute(directory, out offset);
            DirectoryEntry entry;
            Dictionary<String, DirectoryEntry> table = DirectoryLookupTable(parent.FirstCluster, false);
            Regex regex = new Regex(RegexString(src));
            List<String> fileList = new List<String>();
            int count = 0;
            StringBuilder sb = new StringBuilder();

            if (!directory.EndsWith("\\")) directory += "\\";
            foreach (String str in table.Keys)
            {
                if (regex.IsMatch(directory + str))
                {
                    table.TryGetValue(str, out entry);
                    if (entry.IsArchive)
                        fileList.Add(str);
                    else if (folders && entry.IsFolder)
                        fileList.Add(str);
                }
            }
            if (fileList.Count > 0)
            {
                String destination;
                if (fileList.Count > 1 && !Path.GetFileName(dest).StartsWith("*"))
                    throw new ArgumentException("Sintaxis incorrecta");
                for (int i = 0; i < fileList.Count; i++)
                {
                    table.TryGetValue(fileList[i], out entry);
                    try
                    {
                        destination = GetDestination(fileList[i], dest);
                        MoveEntry(parent, entry, destination);
                        sb.Append(fileList[i]);
                        sb.Append(" --> ");
                        sb.AppendLine(destination.Replace(directory, ""));
                        count++;
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(ex.Message);
                    }
                }
                sb.AppendFormat("\t{0} archivos movidos", count);
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("No se encontro el archivo especificado.");
                sb.AppendLine("\t0 archivos movidos.");
            }
            return sb.ToString();
        }

        public bool Rename(String oldPath, String newName)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (oldPath == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (newName == String.Empty)
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (!oldPath.StartsWith("V:\\"))
                throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
            if (String.Compare(oldPath, "V:\\") == 0)
                throw new ArgumentException("No se puede renombrar el directorio raiz");
            if (String.Compare(newName, "V:\\") == 0)
                throw new ArgumentException("Nombre invalido");
            if (newName == null)
                return false;
            String oldName = Path.GetFileName(oldPath);
            String parentPath = Path.GetDirectoryName(oldPath);
            if (String.Compare(Path.GetFileName(oldName), newName, true) == 0)
                return false;
            if (!DirectoryEntry.IsValidName(Path.GetFileName(newName)))
                return false;

            long offset;
            DirectoryEntry parent = FindEntryAbsolute(parentPath, out offset);
            Dictionary<String, DirectoryEntry> table = DirectoryLookupTable(parent.FirstCluster, true);
            if (table.ContainsKey(oldName))
            {
                DirectoryEntry entry;
                table.TryGetValue(oldName, out entry);
                RenameEntry(newName, parent, entry, parentPath);
            }
            else return false;
            return true;
        }

        #endregion

        #region IData

        public int Open(String path)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            int fd = 0;
            while (fd < MAXFD && fileTable[fd] != null)
                fd++;
            if (fd == MAXFD)
                return -1;
            fileTable[fd] = new VirtualFile(this, path);
            filesOpened++;
            return fd;
        }

        public int Open(String path, OPENMODE mode)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            int fd = 0;
            while (fd < MAXFD && fileTable[fd] != null)
                fd++;
            if (fd == MAXFD)
                return -1;
            fileTable[fd] = new VirtualFile(this, path, mode);
            filesOpened++;
            return fd;
        }

        public int Open(String path, OPENMODE mode, uint size)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            int fd = 0;
            while (fd < MAXFD && fileTable[fd] != null)
                fd++;
            if (fd == MAXFD)
                return -1;
            fileTable[fd] = new VirtualFile(this, path, mode, size);
            filesOpened++;
            return fd;
        }

        public int Write(int fd, byte[] buffer, int index, int count)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (fd >= MAXFD)
                throw new IndexOutOfRangeException("Descriptor de fichero invalido");
            if (fileTable[fd] == null)
                throw new ArgumentException("Descriptor de fichero invalido");
            float timeToWait;
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            int result = fileTable[fd].Write(buffer, index, count);
            watch.Stop();
            lock (padlock)
            {
                transferRate = (float)result / (float)(watch.Elapsed.TotalSeconds * 1048576.0F);
            }
            timeToWait = (float)result / (maxRateAllowed * 1048576.0F);
            timeToWait -= (float)watch.Elapsed.TotalSeconds;
            if (timeToWait > 0)
            {
                transferRate = maxRateAllowed;
                Thread.Sleep((int)timeToWait * 1000);
            }
            writes++;
            return result;
        }

        public int Read(int fd, byte[] buffer, int index, int count)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (fd >= MAXFD)
                throw new IndexOutOfRangeException("Descriptor de fichero invalido");
            if (fileTable[fd] == null)
                throw new ArgumentException("Descriptor de fichero invalido");
            float timeToWait;
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            int result = fileTable[fd].Read(buffer, index, count);
            watch.Stop();
            lock (padlock)
            {
                transferRate = (float)result / (float)(watch.Elapsed.TotalSeconds * 1048576.0);
            }
            timeToWait = (float)result / (maxRateAllowed * 1048576.0F);
            timeToWait -= (float)watch.Elapsed.TotalSeconds;
            if (timeToWait > 0)
            {
                transferRate = maxRateAllowed;
                Thread.Sleep((int)timeToWait * 1000);
            }
            reads++;
            return result;
        }

        public void Close(int fd)
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (fd >= MAXFD)
                throw new IndexOutOfRangeException("Descriptor de fichero invalido");
            if (fileTable[fd] == null)
                throw new ArgumentException("Descriptor de fichero invalido");
            fileTable[fd] = null;
        }

        public String Contadores()
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Bytes leidos:          {0:0,0;Zero}", bytesRead);
            sb.AppendLine();
            sb.AppendFormat("Bytes escritos:       {0:0,0}", bytesWritten);
            sb.AppendLine();
            sb.AppendFormat("Total bytes:           {0:0,0}", bytesRead + bytesWritten);
            sb.AppendLine();
            sb.AppendFormat("Lecturas:               {0}", reads);
            sb.AppendLine();
            sb.AppendFormat("Escrituras:             {0}", writes);
            sb.AppendLine();
            sb.AppendFormat("Ficheros abiertos:  {0}", filesOpened);

            return sb.ToString();
        }

        public void SetRate(float rate)
        {
            lock (padlock)
            {
                maxRateAllowed = rate;
            }
        }

        public float GetRate()
        {
            lock (padlock)
            {
                return transferRate;
            }
        }

        #endregion

        #region ISystem

        public void Format()
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            ResetDisk();
            formatted = true;
            timer.Start();
        }

        public String Identify()
        {
            if (!formatted)
                throw new InvalidOperationException("El disco no tiene formato");
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            uint free = fsi.FreeClusters * bs.BytesPerSector;
            String strFree;
            if (free > 1048576)
                strFree = String.Format("{0:#,###} MB", free >> 20);
            else if (free > 1024)
                strFree = String.Format("{0:#,###} KB", free >> 10);
            else
                strFree = String.Format("{0} B", free);
            return String.Format("Etiqueta: {0}\nEspacio libre: {1}\nTamaño total: 32 MB", bs.VolumeLabel, strFree); ;
        }

        public void Free(out uint used, out uint free)
        {
            if (!formatted)
                throw new InvalidOperationException("El disco no tiene formato");
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            free = fsi.FreeClusters * bs.BytesPerSector;
            used = diskSize - free;
        }

        public void Mount()
        {
            if (mounted)
                throw new InvalidOperationException("El disco ya esta montado");
            if (formatted)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bs.ReadBootSector(stream);
                    stream.Seek(bs.FSISector * bs.BytesPerSector, SeekOrigin.Begin);
                    fsi.ReadFileSystemInfo(stream);
                    stream.Seek(bs.ReservedSectors * bs.BytesPerSector, SeekOrigin.Begin);
                    fat.ReadFAT(stream);
                }
                timer.Start();
            }
            rootCluster = bs.RootStartCluster;
            mounted = true;
        }

        public void Dismount()
        {
            if (!mounted)
                throw new InvalidOperationException("El disco no esta montado");
            if (busy)
                throw new InvalidOperationException("El disco esta ocupado");
            if (fat.Modified)
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    stream.Seek(bs.FSISector * bs.BytesPerSector, SeekOrigin.Begin);
                    fsi.WriteFileSystemInfo(stream);
                    stream.Seek(bs.ReservedSectors * bs.BytesPerSector, SeekOrigin.Begin);
                    fat.WriteFAT(stream, false);
                    fat.WriteFAT(stream, false);
                }
                timer.Stop();
                fat.Modified = false;
            }
            mounted = false;
        }

        #endregion
    }
}
