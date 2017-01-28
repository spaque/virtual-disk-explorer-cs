using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace VirtualDrive.FileSystem.FAT32
{
    public enum ENTRYTYPE
    {
        SFN,    // Short file name
        LFN,    // Long file name
        DOT,    // Either '.' or '..'
        UNUSED, // Deleted entry
        EOD,    // End of directory
        ROOT
    }

    [Flags]
    public enum ATTRIBUTE : byte
    {
        NONE        = 0x00,
        READ_ONLY   = 0x01,
        HIDDEN      = 0x02,
        SYSTEM      = 0x04,
        VOLUME_ID   = 0x08,
        DIRECTORY   = 0x10,
        ARCHIVE     = 0x20,
        LONG_NAME   = READ_ONLY | HIDDEN | SYSTEM | VOLUME_ID
    }

    public class DirectoryEntry : IEquatable<DirectoryEntry>
    {
        #region Fields

        private static uint entrySize = 32;
        // " * / : < > ? \ |
        private static string invalidCharacters = "\"*/:<>?\\|";
        private static byte MAXENTRIES = 0xFF;

        private String name;
        private ATTRIBUTE attributes;
        private uint firstCluster;
        private uint fileSize;
        private ENTRYTYPE type;
        private byte createdTimeMs;
        private ushort createdTime;
        private ushort createdDate;
        private ushort modifiedTime;
        private ushort modifiedDate;
        private ushort lastAccessDate;

        private byte lfnOrd;
        private byte lfnChecksum;
        private bool isLastLongEntry;

        // entry offset within the cluster, saved in reserved byte 12
        private byte offset;

        #endregion

        #region Constructors

        public DirectoryEntry()
        {
            type = ENTRYTYPE.EOD;
        }

        public DirectoryEntry(uint cluster)
        {
            firstCluster = cluster;
            type = ENTRYTYPE.ROOT;
            attributes = ATTRIBUTE.DIRECTORY;
            name = "V:";
        }

        public DirectoryEntry(String _name, ATTRIBUTE attr)
        {
            if (_name.Length > 11)
                throw new ArgumentException(String.Format("\"{0}\" no es un nombre corto valido", _name));
            type = ENTRYTYPE.SFN;
            DateTime dt = DateTime.Now;
            name = _name;
            attributes = attr;
            createdTimeMs = (byte)(dt.Millisecond / 5);
            createdTime = ToFATTime(dt);
            createdDate = ToFATDate(dt);
            modifiedTime = createdTime;
            modifiedDate = createdDate;
            lastAccessDate = createdDate;
        }

        public DirectoryEntry(String _name, uint cluster)
        {
            if (String.Compare(_name, ".", true) != 0 && 
                String.Compare(_name, "..", true) != 0)
                throw new ArgumentException("Invalid name");
            DateTime dt = DateTime.Now;
            name = _name;
            attributes = ATTRIBUTE.DIRECTORY;
            createdTimeMs = (byte)(dt.Millisecond / 5);
            createdTime = ToFATTime(dt);
            createdDate = ToFATDate(dt);
            modifiedTime = createdTime;
            modifiedDate = createdDate;
            lastAccessDate = createdDate;
            firstCluster = cluster;
        }

        public DirectoryEntry(String _name, byte chksum, byte ord)
        {
            if (_name.Length > 13)
                throw new ArgumentException(String.Format("\"{0}\" es demasiado largo", _name));
            type = ENTRYTYPE.LFN;
            name = _name;
            lfnChecksum = chksum;
            lfnOrd = (byte)(ord & 0xBF);
            if ((ord & 0x40) != 0) isLastLongEntry = true;
        }

        #endregion

        #region Properties

        public static string InvalidCharacters { get { return invalidCharacters; } }

        public static uint EntrySize { get { return entrySize; } }

        public String Name
        {
            get
            {
                if (type == ENTRYTYPE.SFN && !IsFolder)
                {
                    String fileName, extension;
                    fileName = name.Substring(0, 8).Trim(new char[] { ' ' });
                    extension = name.Substring(8, 3).Trim(new char[] { ' ' });
                    if (extension.Length == 0)
                        return fileName;
                    else
                        return fileName + "." + extension;
                }
                else if (type == ENTRYTYPE.SFN && IsFolder)
                    return name.Trim(new char[] { ' ' });
                else
                    return name;
            }
        }

        public String ShortName
        {
            get { return name; }
            set { name = value; }
        }

        public uint FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        public ATTRIBUTE Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public bool IsReadOnly
        {
            get { return (attributes & ATTRIBUTE.READ_ONLY ) != 0; }
        }

        public bool IsHidden
        {
            get { return (attributes & ATTRIBUTE.HIDDEN) != 0; }
        }

        public bool IsSystem
        {
            get { return (attributes & ATTRIBUTE.SYSTEM) != 0; }
        }

        public bool IsVolume
        {
            get { return (attributes & ATTRIBUTE.VOLUME_ID) != 0; }
        }

        public bool IsFolder
        {
            get { return (attributes & ATTRIBUTE.DIRECTORY) != 0; }
        }

        public bool IsArchive
        {
            get { return (attributes & ATTRIBUTE.ARCHIVE) != 0; }
        }

        public DateTime ModifiedDate
        {
            get
            {
                if (type != ENTRYTYPE.SFN)
                    throw new InvalidOperationException("Solo se puede obtener la fecha de modificacion para una entrada corta");
                int seconds, minutes, hours;
                seconds = (modifiedTime & 0x1F) << 1;
                minutes = (modifiedTime >> 5) & 0x3F;
                hours = (modifiedTime >> 11) & 0x1F;
                int day, month, year;
                day = modifiedDate & 0x1F;
                month = (modifiedDate >> 5) & 0x0F;
                year = (modifiedDate >> 9) & 0x7F;
                return new DateTime(year + 1980, month, day, hours, minutes, seconds);
            }
            set
            {
                if (type != ENTRYTYPE.SFN)
                    throw new InvalidOperationException("Solo se puede modificar la fecha de modificacion para una entrada corta");
                modifiedDate = ToFATDate(value);
                modifiedTime = ToFATTime(value);
            }
        }

        public DateTime LastAccessDate
        {
            get
            {
                if (type != ENTRYTYPE.SFN)
                    throw new InvalidOperationException("Solo se puede obtener la fecha de acceso para una entrada corta");
                return ToSystemDate(lastAccessDate);
            }
            set
            {
                if (type != ENTRYTYPE.SFN)
                    throw new InvalidOperationException("Solo se puede modificar la fecha de acceso para una entrada corta");
                lastAccessDate = ToFATDate(value);
            }
        }

        public ENTRYTYPE Type { get { return type; } }

        public uint FirstCluster
        {
            get { return firstCluster; }
            set { firstCluster = value; }
        }

        public bool IsLastLongEntry
        {
            get
            {
                if (type != ENTRYTYPE.LFN)
                    throw new InvalidOperationException("Operacion solo valida para entradas largas");
                return isLastLongEntry;
            }
            set { isLastLongEntry = value; }
        }

        public byte Checksum { get { return lfnChecksum; } }

        public byte Order
        {
            get { return lfnOrd; }
            set { lfnOrd = value; }
        }

        public byte Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        #endregion

        #region Public Methods

        public void ReadDirectoryEntry(FileStream stream)
        {
            byte[] contents = new byte[entrySize];
            int bytesRead = stream.Read(contents, 0, (int)entrySize);
            if (bytesRead < entrySize)
                throw new InvalidDataException("Entrada de directorio invalida");
            SetDirectoryEntry(contents);
        }

        public void WriteDirectoryEntry(FileStream stream)
        {
            if (type == ENTRYTYPE.SFN && firstCluster == 0)
                throw new InvalidOperationException("No se puede escribir la entrada, aun no se ha asignado cluster");
            byte[] contents;
            if (type == ENTRYTYPE.LFN || attributes == ATTRIBUTE.LONG_NAME)
                contents = LongNameDirectoryTable();
            else
                contents = DirectoryTable();
            stream.Write(contents, 0, (int)entrySize);
        }

        public void SetDirectoryEntry(byte[] entry)
        {
            ushort fClusterLo, fClusterHi;
            if (entry.Length != entrySize)
                throw new ArgumentException("Entrada de directorio invalida");
            if (entry[0] == 0x00)
            {
                type = ENTRYTYPE.EOD;
                return;
            }
            attributes = (ATTRIBUTE)entry[11];
            offset = entry[12];
            if (attributes != ATTRIBUTE.LONG_NAME)
            {
                type = ENTRYTYPE.SFN;
                name = Encoding.ASCII.GetString(entry, 0, 11);
                createdTime = BitConverter.ToUInt16(entry, 14);
                createdDate = BitConverter.ToUInt16(entry, 16);
                lastAccessDate = BitConverter.ToUInt16(entry, 18);
                fClusterHi = BitConverter.ToUInt16(entry, 20);
                modifiedTime = BitConverter.ToUInt16(entry, 22);
                modifiedDate = BitConverter.ToUInt16(entry, 24);
                fClusterLo = BitConverter.ToUInt16(entry, 26);
                fileSize = BitConverter.ToUInt32(entry, 28);
                firstCluster = (((uint)fClusterHi) << 16) | (uint)fClusterLo;
            }
            else
            {
                // 13 = max number of characters in a single lfn entry
                StringBuilder lfnName = new StringBuilder(13);
                type = ENTRYTYPE.LFN;
                isLastLongEntry = (entry[0] & 0x40) != 0;
                // remove last long entry mark
                lfnOrd = (byte)(entry[0] & 0xBF);
                lfnName.Append(Encoding.Unicode.GetString(entry, 1, 10));
                lfnChecksum = entry[13];
                lfnName.Append(Encoding.Unicode.GetString(entry, 14, 12));
                lfnName.Append(Encoding.Unicode.GetString(entry, 28, 4));
                // Remove trailing 0xFFFF
                int i = 0;
                while (i < lfnName.Length && lfnName[i] != '\0')
                    i++;
                if (i < lfnName.Length)
                    lfnName.Remove(i, lfnName.Length - i);
                name = lfnName.ToString();
            }
            if (entry[0] == 0xE5 || entry[0] == 0x05)
                type = ENTRYTYPE.UNUSED;
        }

        public void DeleteEntry()
        {
            type = ENTRYTYPE.UNUSED;
            if (IsArchive)
                fileSize = 0;
        }

        #region Static

        public static bool IsValidName(String name)
        {
            for (int i = 0; i < invalidCharacters.Length; i++)
            {
                for (int j = 0; j < name.Length; j++)
                {
                    if (invalidCharacters[i] == name[j])
                        return false;
                }
            }
            return true;
        }

        public static String ExtractLongName(List<DirectoryEntry> dirs, ref int i)
        {
            DirectoryEntry entry = dirs[i++];
            byte cheksum = DirectoryEntry.ShortNameChecksum(entry.ShortName);
            if (i < dirs.Count && dirs[i].Type == ENTRYTYPE.LFN)
            {
                StringBuilder name = new StringBuilder();
                do
                {
                    entry = dirs[i];
                    name.Append(entry.Name);
                    i++;
                } while (i < dirs.Count &&
                       entry.Type == ENTRYTYPE.LFN &&
                       !entry.IsLastLongEntry &&
                       entry.Checksum == cheksum);
                // there's orphans long entries
                while (i < dirs.Count && dirs[i].Type == ENTRYTYPE.LFN)
                    i++;
                return name.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        public static DateTime ToSystemTime(ushort time)
        {
            int seconds, minutes, hours;
            seconds = time & 0x1F;
            minutes = (time >> 5) & 0x3F;
            hours = (time >> 11) & 0x1F;
            return new DateTime(2007, 5, 5, hours, minutes, seconds);
        }

        public static DateTime ToSystemDate(ushort date)
        {
            int day, month, year;
            day = date & 0x1F;
            month = (date >> 5) & 0x0F;
            year = (date >> 9) & 0x7F;
            return new DateTime(year + 1980, month, day);
        }

        public static ushort ToFATTime(DateTime dt)
        {
            int seconds, minutes, hours;
            seconds = dt.Second >> 1;
            minutes = dt.Minute;
            hours = dt.Hour;
            return (ushort)(seconds | (minutes << 5) | (hours << 11));
        }

        public static ushort ToFATDate(DateTime dt){
            int day, month, year;
            day = dt.Day;
            month = dt.Month;
            year = dt.Year - 1980;
            return (ushort)(day | (month << 5) | (year << 9));
        }

        /// <summary>
        /// Calculates the checksum for a given MS-DOS formatted name.
        /// </summary>
        /// <param name="name">
        /// String containing a name of lenght 11.
        /// </param>
        /// <returns>
        /// Checksum of the name.
        /// </returns>
        public static byte ShortNameChecksum(String name)
        {
            byte sum = 0;
            byte partial = 0;
            if (name.Length != 11)
                throw new ArgumentException(String.Format("\"{0}\" no es un nombre corto", name));
            // check for lowercase
            if (String.Compare(name, name.ToUpper(), false) != 0)
                throw new ArgumentException(String.Format("\"{0}\" no es un nombre corto", name));
            for (int i = 0; i < 11; i++)
            {
                partial = (byte)((((sum & 0x01) != 0) ? 0x80 : 0x00));
                partial += (byte)(sum >> 1);
                sum = (byte)(partial + (byte)name[i]);
                partial = 0;
            }
            return sum;
        }

        public static String Extension(String name)
        {
            StringBuilder extension = new StringBuilder(name);
            int i = name.Length - 1;
            while (i > 0 && name[i] != '.')
            {
                extension.Insert(0, name[i]);
                i--;
            }
            extension.Insert(0, '.');
            return extension.ToString();
        }

        /// <summary>
        /// Generates a short name from the name of an entry.
        /// </summary>
        /// <param name="longName">
        /// The name of the entry.
        /// </param>
        /// <param name="dir">
        /// True if the entry is a directory
        /// </param>
        /// <param name="n">
        /// Number of entries in the directory with the same name.
        /// </param>
        /// <returns>
        /// The short name string.
        /// </returns>
        public static String GenerateShortName(String longName, bool dir, uint n)
        {
            StringBuilder extension = new StringBuilder();
            StringBuilder name = new StringBuilder(longName.ToUpper());
            int i = name.Length - 1;
            if (!dir)
            {
                if (longName.Contains("."))
                {
                    // split name and extension
                    while (i > 0 && name[i] != '.')
                    {
                        extension.Insert(0, name[i]);
                        i--;
                    }
                    // trim to size 3
                    while (extension.Length > 3)
                        extension.Remove(extension.Length - 1, 1);
                    //extension.Insert(0, '.');
                    while (extension.Length < 3)
                        extension.Append(' ');
                    name.Remove(i, name.Length - i);
                }
                else
                    extension.Append("   ");
            }
            else
            {
                extension.Append("   ");
            }

            // remove spaces and dots
            for (i = name.Length - 1; i >= 0; i--)
                if (name[i] == ' ' || name[i] == '.')
                    name.Remove(i, 1);
            if (n > 0)
            {
                uint length = 7 - ((uint)Math.Log10((double)n) + 1);
                // trim to correct size
                while (name.Length > length)
                    name.Remove(name.Length - 1, 1);
                // add numeric tail
                name.Append('~');
                name.Append(n);
            }
            else
                while (name.Length > 8)
                    name.Remove(name.Length - 1, 1);
            while (name.Length < 8)
                name.Append(' ');

            return name.ToString() + extension.ToString();
        }

        public static bool IsShortName(String name, bool dir)
        {
            if (dir)
            {
                if (name.Length > 8)
                    return false;
                if (String.Compare(name, name.ToUpper(), false) != 0)
                    return false;
                return true;
            }
            else
            {
                if (name.Length > 12)
                    return false;
                if (String.Compare(name, name.ToUpper(), false) != 0)
                    return false;
                if (Extension(name).Length > 3)
                    return false;
                if (Path.GetFileNameWithoutExtension(name).Length > 8)
                    return false;
                return true;
            }
        }

        public static uint GenerateShortNameTail(String name, bool dir, List<DirectoryEntry> entries)
        {
            uint result = 0;
            String itemName;
            String extension = "$";
            if (dir)
                itemName = name;
            else
            {
                itemName = Path.GetFileNameWithoutExtension(name);
                String ext = Path.GetExtension(name).ToUpper();
                if (ext.Length > 4)
                    extension = "[.]" + ext.Substring(1, 3) + "$";
                else if (ext.Length > 0)
                    extension = "[.]" + ext.Remove(0,1) + "$";
            }
            List<String> names = new List<String>();
            StringBuilder subStrings = new StringBuilder();
            int length = Math.Min(name.Length, 6);
            String prefix = name.Replace(" ","").Substring(0, length).ToUpper();
            int i;
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < length; i++)
                sb.Append(Disk.RegexString(prefix[i]));
            String regexPref = sb.ToString();
            i = 0;
            while (i < length)
            {
                subStrings.Append("^");
                if (regexPref[i] == '\\')
                {
                    i++;
                    length++;
                }
                subStrings.Append(regexPref.Substring(0, i + 1));
                subStrings.Append(@"[~]\d+");
                subStrings.Append(extension);
                subStrings.Append("|");
                i++;
            }
            subStrings.Remove(subStrings.Length - 1, 1);

            Regex regex = new Regex(subStrings.ToString());

            foreach (DirectoryEntry entry in entries)
                if (regex.IsMatch(entry.Name) && !names.Contains(entry.Name))
                    names.Add(entry.Name);

            extension = Path.GetExtension(name).ToUpper();
            if (extension.Length > 4)
                extension = extension.Substring(0, 4);
            if (names.Count > 0)
            {
                result = 1;
                int count;
                prefix += "~" + result;
                while (names.Contains(prefix + extension))
                {
                    result++;
                    count = (int)Math.Log10((double)result) + 1;
                    prefix = prefix.Remove(prefix.Length - count, count);
                    prefix += result;
                }
            }
            else if (itemName.Length > 8) result = 1;
            return result;
        }

        public static List<DirectoryEntry> GetEntries(String name, ATTRIBUTE attributes, List<DirectoryEntry> dirEntries)
        {
            List<DirectoryEntry> result = new List<DirectoryEntry>();
            bool dir = (attributes & ATTRIBUTE.DIRECTORY) != 0;

            uint n;
            if (dirEntries != null)
                n = GenerateShortNameTail(name, dir, dirEntries);
            else
                n = 0;

            String shortName = GenerateShortName(name, dir, n);

            if (IsShortName(name, dir))
                result.Add(new DirectoryEntry(shortName, attributes));
            else
            {
                result.Add(new DirectoryEntry(shortName, attributes));
                byte checksum = ShortNameChecksum(shortName);
                byte ord = 1;
                int index = 0;
                bool end = false;
                while (!end)
                {
                    if (index + 13 >= name.Length)
                    {
                        result.Add(new DirectoryEntry(name.Substring(index), checksum, (byte)(ord | 0x40)));
                        end = true;
                    }
                    else
                    {
                        result.Add(new DirectoryEntry(name.Substring(index, 13), checksum, ord));
                        index += 13;
                        ord++;
                    }
                }
            }
            return result;
        }

        #endregion

        #endregion

        #region Private Methods

        private byte[] DirectoryTable()
        {
            byte[] result = new byte[entrySize];
            if (type != ENTRYTYPE.EOD && type != ENTRYTYPE.ROOT)
            {
                if (type == ENTRYTYPE.UNUSED)
                {
                    Array.Copy(Encoding.ASCII.GetBytes(name), 1, result, 1, 10);
                    result[0] = 0xE5;
                }
                else
                    Array.Copy(Encoding.ASCII.GetBytes(name), result, 11);
                result[11] = (byte)attributes;
                result[12] = offset;
                result[13] = createdTimeMs;
                Array.Copy(BitConverter.GetBytes(createdTime), 0, result, 14, 2);
                Array.Copy(BitConverter.GetBytes(createdDate), 0, result, 16, 2);
                Array.Copy(BitConverter.GetBytes(lastAccessDate), 0, result, 18, 2);
                Array.Copy(BitConverter.GetBytes(firstCluster), 2, result, 20, 2);
                Array.Copy(BitConverter.GetBytes(modifiedTime), 0, result, 22, 2);
                Array.Copy(BitConverter.GetBytes(modifiedDate), 0, result, 24, 2);
                Array.Copy(BitConverter.GetBytes(firstCluster), 0, result, 26, 2);
                Array.Copy(BitConverter.GetBytes(fileSize), 0, result, 28, 4);
            }
            return result;
        }

        private byte[] LongNameDirectoryTable()
        {
            byte[] result = new byte[entrySize];
            int length = name.Length;
            if (isLastLongEntry) result[0] = (byte)(lfnOrd | 0x40);
            else result[0] = lfnOrd;
            result[11] = 0x0F;
            result[12] = this.offset;
            result[13] = lfnChecksum;
            if (length < 5)
            {
                int offset = 1 + ((1 + length) << 1);
                Array.Copy(Encoding.Unicode.GetBytes(name), 0, result, 1, length << 1);
                while (offset < 11)
                    result[offset++] = 0xFF;
                for (offset = 14; offset < 26; offset++)
                    result[offset] = 0xFF;
                for (offset = 28; offset < 32; offset++)
                    result[offset] = 0xFF;
            }
            else if (length < 11)
            {
                Array.Copy(Encoding.Unicode.GetBytes(name), 0, result, 1, 10);
                length -= 5;
                int offset = 14 + ((1 + length) << 1);
                Array.Copy(Encoding.Unicode.GetBytes(name), 10, result, 14, length << 1);
                while (offset < 26)
                    result[offset++] = 0xFF;
                for (offset = 28; offset < 32; offset++)
                    result[offset] = 0xFF;
            }
            else if (length < 13)
            {
                Array.Copy(Encoding.Unicode.GetBytes(name), 0, result, 1, 10);
                Array.Copy(Encoding.Unicode.GetBytes(name), 10, result, 14, 12);
                length -= 11;
                int offset = 28 + ((1 + length) << 1);
                Array.Copy(Encoding.Unicode.GetBytes(name), 22, result, 28, length << 1);
                while (offset < 32)
                    result[offset++] = 0xFF;
            }
            else
            {
                Array.Copy(Encoding.Unicode.GetBytes(name), 0, result, 1, 10);
                Array.Copy(Encoding.Unicode.GetBytes(name), 10, result, 14, 12);
                Array.Copy(Encoding.Unicode.GetBytes(name), 22, result, 28, 4);
            }
            if (type == ENTRYTYPE.UNUSED)
                result[0] = 0xE5;
            return result;
        }

        #endregion

        #region IEquatable<DirectoryEntry>

        public bool Equals(DirectoryEntry other)
        {
            return (String.Compare(name, other.name, true) == 0) &&
                           (attributes == other.attributes) &&
                           (type == other.type);
        }

        #endregion

        public override string ToString()
        {
            if (type == ENTRYTYPE.EOD)
                return "End of Directory";
            if (type == ENTRYTYPE.UNUSED)
                return "Deleted Entry";
            return Name;
        }
    }
}
