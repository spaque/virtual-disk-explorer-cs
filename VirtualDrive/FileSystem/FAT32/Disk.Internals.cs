using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

using VirtualDrive.Shell;

namespace VirtualDrive.FileSystem.FAT32
{
    public sealed partial class Disk
    {
        #region Public Methods

        public void ReadDisk(byte[] buffer, int index, int count, uint firstCluster, uint offset)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            if (count == 0) return;
            Sector sector = new Sector();
            uint cluster = firstCluster;
            uint actualOffset = offset;
            long readOffset;
            int lenght;

            while (actualOffset >= BootSector.SectorSize && cluster < FAT.Size)
            {
                actualOffset -= BootSector.SectorSize;
                cluster = fat.FindNextCluster(cluster);
            }

            if (cluster < FAT.Size)
            {
                using (FileStream fstream =
                    new FileStream(path,
                                   FileMode.Open,
                                   FileAccess.Read,
                                   FileShare.ReadWrite,
                                   8192,
                                   FileOptions.RandomAccess))
                {
                    while (count > 0 && cluster < FAT.Size)
                    {
                        readOffset = dataStartOffset + (cluster * BootSector.SectorSize);
                        fstream.Seek(readOffset, SeekOrigin.Begin);
                        sector.ReadSector(fstream);
                        lenght = Math.Min(count, (int)BootSector.SectorSize);
                        sector.CopyFromSector(0, buffer, index, lenght);
                        cluster = fat.FindNextCluster(cluster);
                        index += lenght;
                        count -= lenght;
                        lock (padlock)
                        {
                            bytesRead += (ulong)lenght;
                        }
                    }
                }
            }
        }

        public void WriteDisk(byte[] buffer, int index, int count, uint firstCluster, uint offset)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            if (count == 0) return;
            Sector sector = new Sector();
            uint cluster = firstCluster;
            uint actualOffset = offset;
            long writeOffset;
            int lenght;

            while (actualOffset >= BootSector.SectorSize)
            {
                actualOffset -= BootSector.SectorSize;
                cluster = fat.FindNextCluster(cluster);
                if (cluster >= FAT.Size)
                    cluster = fat.AllocNewCluster(firstCluster);
            }

            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Write,
                               FileShare.ReadWrite,
                               8192,
                               FileOptions.RandomAccess))
            {
                while (count > 0)
                {
                    lenght = Math.Min(count, (int)BootSector.SectorSize);
                    writeOffset = dataStartOffset + (cluster * BootSector.SectorSize);
                    sector.CopyToSector(index, buffer, 0, lenght);
                    fstream.Seek(writeOffset, SeekOrigin.Begin);
                    sector.WriteSector(fstream);
                    index += lenght;
                    count -= lenght;
                    cluster = fat.FindNextCluster(cluster);
                    if (cluster >= FAT.Size && count > 0)
                        cluster = fat.AllocNewCluster(firstCluster);
                    lock (padlock)
                    {
                        bytesWritten += (ulong)lenght;
                    }
                }
            }
        }

        public List<DirectoryEntry> GetDirectoryContents(uint firstCluster)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            List<DirectoryEntry> contents = new List<DirectoryEntry>();
            Sector sector = new Sector(firstCluster);

            // set the current position the stream
            uint currentCluster = firstCluster;
            long offset = dataStartOffset + (currentCluster * BootSector.SectorSize);

            DirectoryEntry entry;
            bool eodReached = false;
            uint entryOffset;

            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               4096, // default buffer size
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
                while (!eodReached)
                {
                    // walk the sector and extract directory entries
                    entryOffset = 0;
                    do
                    {
                        entry = new DirectoryEntry();
                        entry.SetDirectoryEntry(
                            sector.SubArray(entryOffset, DirectoryEntry.EntrySize));
                        if (entry.Type != ENTRYTYPE.EOD && entry.Type != ENTRYTYPE.UNUSED)
                            contents.Add(entry);
                        entryOffset += DirectoryEntry.EntrySize;
                    } while (entryOffset < BootSector.SectorSize && entry.Type != ENTRYTYPE.EOD);
                    // follow the cluster chain
                    if (entry.Type != ENTRYTYPE.EOD)
                    {
                        currentCluster = fat.FindNextCluster(currentCluster);
                        if (currentCluster < FAT.Size)
                        {
                            offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                            fstream.Seek(offset, SeekOrigin.Begin);
                            sector.SectorNumber = currentCluster;
                            sector.ReadSector(fstream);
                        }
                        else
                            eodReached = true;
                    }
                    else
                        eodReached = true;
                }
            }
            return contents;
        }

        public int GetDirectoryEntryCount(uint firstCluster)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            int result = 0;
            Sector sector = new Sector(firstCluster);

            // set the current position the stream
            uint currentCluster = firstCluster;
            long offset = dataStartOffset + (currentCluster * BootSector.SectorSize);

            bool eodReached = false;
            uint entryOffset;
            byte head;

            using (FileStream fstream =
                new FileStream(path, 
                               FileMode.Open, 
                               FileAccess.Read, 
                               FileShare.ReadWrite,
                               4096, 
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
                while (!eodReached)
                {
                    // walk the sector and extract directory entries
                    entryOffset = 0;
                    do
                    {
                        head = sector.PeekByte(entryOffset);
                        if (head != 0x00 && head != 0xE5)
                            result++;
                        entryOffset += DirectoryEntry.EntrySize;
                    } while (entryOffset < BootSector.SectorSize && head != 0x00);
                    // follow the cluster chain
                    if (head != 0x00)
                    {
                        currentCluster = fat.FindNextCluster(currentCluster);
                        if (currentCluster < FAT.Size)
                        {
                            offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                            fstream.Seek(offset, SeekOrigin.Begin);
                            sector.SectorNumber = currentCluster;
                            sector.ReadSector(fstream);
                        }
                        else
                            eodReached = true;
                    }
                    else
                        eodReached = true;
                }
            }
            return result;
        }

        public int GetDirectoryEntryLength(uint firstCluster)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            int result = 0;
            Sector sector = new Sector(firstCluster);

            // set the current position the stream
            uint currentCluster = firstCluster;
            long offset = dataStartOffset + (currentCluster * BootSector.SectorSize);

            bool eodReached = false;
            uint entryOffset;
            byte head;

            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               4096,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
                while (!eodReached)
                {
                    // walk the sector and extract directory entries
                    entryOffset = 0;
                    do
                    {
                        head = sector.PeekByte(entryOffset);
                        if (head != 0x00)
                            result++;
                        entryOffset += DirectoryEntry.EntrySize;
                    } while (entryOffset < BootSector.SectorSize && head != 0x00);
                    // follow the cluster chain
                    if (head != 0x00)
                    {
                        currentCluster = fat.FindNextCluster(currentCluster);
                        if (currentCluster < FAT.Size)
                        {
                            offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                            fstream.Seek(offset, SeekOrigin.Begin);
                            sector.SectorNumber = currentCluster;
                            sector.ReadSector(fstream);
                        }
                        else
                            eodReached = true;
                    }
                    else
                        eodReached = true;
                }
            }
            return result;
        }

        public DirectoryEntry FindEntryAbsolute(String path, out long offset)
        {
            if (String.Compare(path, "V:\\", true) == 0)
            {
                offset = -1;
                return new DirectoryEntry(bs.RootStartCluster);
            }
            path = path.Remove(0, 3);
            String[] route = path.Split(new char[] { '\\' });

            uint cluster = bs.RootStartCluster;
            DirectoryEntry entry = null;
            Dictionary<String, DirectoryEntry> table;

            for (int i = 0; i < route.Length - 1; i++)
            {
                table = DirectoryLookupTable(cluster, true);
                if (table.ContainsKey(route[i]))
                    table.TryGetValue(route[i], out entry);
                else
                    throw new FileNotFoundException(String.Format("No se encontro la entrada \"{0}\"", route[i]));
                cluster = entry.FirstCluster;
            }
            // parent cluster offset
            offset = dataStartOffset + (cluster * BootSector.SectorSize);

            table = DirectoryLookupTable(cluster, true);
            if (table.ContainsKey(route[route.Length - 1]))
                table.TryGetValue(route[route.Length - 1], out entry);
            else
                throw new FileNotFoundException(String.Format("No se encontro la entrada \"{0}\"", route[route.Length - 1]));
            offset += (long)(entry.Offset * DirectoryEntry.EntrySize);

            return entry;
        }

        public DirectoryEntry FindEntryRelative(String path, uint startCluster, out long offset)
        {
            if (startCluster >= FAT.Size || startCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            String[] route = path.Split(new char[] { '\\' });

            uint cluster = startCluster;
            DirectoryEntry entry = null;
            Dictionary<String, DirectoryEntry> table;

            for (int i = 0; i < route.Length - 1; i++)
            {
                table = DirectoryLookupTable(cluster, true);
                if (table.ContainsKey(route[i]))
                    table.TryGetValue(route[i], out entry);
                else
                    throw new FileNotFoundException(String.Format("No se encontro la entrada \"{0}\"", route[i]));
                cluster = entry.FirstCluster;
            }
            // parent cluster offset
            offset = dataStartOffset + (cluster * BootSector.SectorSize);

            table = DirectoryLookupTable(cluster, true);
            if (table.ContainsKey(route[route.Length - 1]))
                table.TryGetValue(route[route.Length - 1], out entry);
            else
                throw new FileNotFoundException(String.Format("No se encontro la entrada \"{0}\"", route[route.Length - 1]));
            offset += (long)(entry.Offset * DirectoryEntry.EntrySize);

            return entry;
        }

        public void TruncateFile(DirectoryEntry entry, long entryOffset)
        {
            fat.ReleaseClusterChainTail(entry.FirstCluster);
            entry.FileSize = 0;
            UpdateDirectoryEntry(entry, entryOffset);
        }

        public void UpdateDirectoryEntry(DirectoryEntry entry, long entryOffset)
        {
            if (entryOffset >= Disk.DiskSize || entryOffset < 0)
                throw new ArgumentException("entryOffset esta fuera de rango");
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Write,
                               FileShare.ReadWrite,
                               (int)DirectoryEntry.EntrySize,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(entryOffset, SeekOrigin.Begin);
                entry.WriteDirectoryEntry(fstream);
            }
        }

        public Dictionary<String, DirectoryEntry> DirectoryLookupTable(uint firstCluster, bool all)
        {
            List<DirectoryEntry> entries = GetDirectoryContents(firstCluster);
            Dictionary<String, DirectoryEntry> result = new Dictionary<String, DirectoryEntry>();
            int i = 0;
            while (i < entries.Count)
            {
                DirectoryEntry entry = entries[i];
                String name = DirectoryEntry.ExtractLongName(entries, ref i);
                if (name == String.Empty)
                {
                    if (!result.ContainsKey(entry.Name))
                        result.Add(entry.Name, entry);
                }
                else
                {
                    result.Add(name, entry);
                    if (all && !result.ContainsKey(entry.Name))
                        result.Add(entry.Name, entry);
                }
            }
            return result;
        }

        private bool CloneEntry(String path, DirectoryEntry entry)
        {
            path = path.Remove(0, 3);
            if (path.Length == 0)
                throw new InvalidOperationException("El directorio raiz ya esta creado");
            DirectoryEntry parent = null;
            Dictionary<String, DirectoryEntry> table = null;
            String[] route = path.Split(new char[] { '\\' });
            int i = 0;
            uint cluster = bs.RootStartCluster;

            while (i < route.Length)
            {
                table = DirectoryLookupTable(cluster, true);
                if (table.ContainsKey(route[i]))
                {
                    table.TryGetValue(route[i], out parent);
                    if (i < route.Length && !parent.IsFolder)
                        throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
                    cluster = parent.FirstCluster;
                    i++;
                }
                else break;
            }
            if (i == route.Length) return false;
            else
            {
                List<DirectoryEntry> dirEntries = new List<DirectoryEntry>(table.Values);
                List<DirectoryEntry> newEntries;
                while (i < route.Length)
                {
                    if (i == route.Length - 1)
                    {
                        newEntries = DirectoryEntry.GetEntries(route[i], entry.Attributes, dirEntries);
                        String shortName = entry.ShortName;
                        entry.ShortName = newEntries[0].ShortName;
                        newEntries[0] = entry;
                        WriteEntries(newEntries, cluster, false);
                        entry.ShortName = shortName;
                    }
                    else
                    {
                        newEntries = DirectoryEntry.GetEntries(route[i], ATTRIBUTE.DIRECTORY, dirEntries);
                        cluster = WriteEntries(newEntries, cluster, true);
                    }
                    dirEntries = null;
                    i++;
                }
            }
            return true;
        }

        public DirectoryEntry CreateEntry(String path, ATTRIBUTE attributes, uint size, out long offset, out bool created)
        {
            if (attributes == ATTRIBUTE.DIRECTORY && size > 0)
                throw new ArgumentException("El tamaño de un directorio debe ser cero");
            path = path.Remove(0, 3);
            if (path.Length == 0)
                throw new InvalidOperationException("El directorio raiz ya esta creado");

            DirectoryEntry entry = null;
            Dictionary<String, DirectoryEntry> table = null;
            String[] route = path.Split(new char[] { '\\' });
            int i = 0;
            uint cluster = bs.RootStartCluster;

            while (i < route.Length)
            {
                table = DirectoryLookupTable(cluster, true);
                if (table.ContainsKey(route[i]))
                {
                    table.TryGetValue(route[i], out entry);
                    if (i < route.Length && !entry.IsFolder)
                    {
                        if (i == route.Length - 1)
                            throw new ArgumentException(String.Format("{0} ya existe", route[i]));
                        throw new ArgumentException(String.Format("\"{0}\" no es una ruta valida", path));
                    }
                    cluster = entry.FirstCluster;
                    i++;
                }
                else break;
            }

            if (i == route.Length)
            {
                created = false;
            }
            else
            {
                List<DirectoryEntry> dirEntries = new List<DirectoryEntry>(table.Values);
                List<DirectoryEntry> newEntries = null;
                while (i < route.Length)
                {
                    if (i == route.Length - 1)
                    {
                        newEntries = DirectoryEntry.GetEntries(route[i], attributes, dirEntries);
                        newEntries[0].FileSize = size;
                        WriteEntries(newEntries, cluster, true);
                    }
                    else
                    {
                        newEntries = DirectoryEntry.GetEntries(route[i], ATTRIBUTE.DIRECTORY, dirEntries);
                        cluster = WriteEntries(newEntries, cluster, true);
                    }
                    dirEntries = null;
                    i++;
                }
                entry = newEntries[0];
                created = true;
            }
            offset = dataStartOffset +
                    (cluster * BootSector.SectorSize) +
                    (entry.Offset * DirectoryEntry.EntrySize);
            return entry;
        }

        public void MoveEntry(DirectoryEntry parent, DirectoryEntry entry, String dest)
        {
            if (CloneEntry(dest, entry))
                DeleteFromDirectory(parent, entry);
        }

        public void RenameEntry(String name, DirectoryEntry parent, DirectoryEntry entry, String dest)
        {
            String path = dest;
            if (path.EndsWith("\\"))
                path += name;
            else
                path += "\\" + name;
            DeleteFromDirectory(parent, entry);
            CloneEntry(path, entry);
        }

        public bool DeleteEntry(DirectoryEntry parent, DirectoryEntry entry, bool forceDelete)
        {
            if (entry.IsFolder)
            {
                int count = GetDirectoryEntryCount(entry.FirstCluster);
                if (!forceDelete && count > 0) return false;
                if (forceDelete && count > 0)
                    DeleteAllSubEntries(entry.FirstCluster);
            }
            else
                DeleteFile(entry);
            DeleteFromDirectory(parent, entry);
            fat.ReleaseClusterChain(entry.FirstCluster);
            return true;
        }

        public long EntrySize(uint cluster)
        {
            long result = 0;
            List<DirectoryEntry> entries = GetDirectoryContents(cluster);
            for (int i = 0; i < entries.Count; i++)
            {
                DirectoryEntry current = entries[i];
                if (current.Type == ENTRYTYPE.SFN && current.IsFolder)
                    result += EntrySize(current.FirstCluster);
                else if (current.Type == ENTRYTYPE.SFN && current.IsArchive)
                    result += current.FileSize;
            }
            double count = (entries.Count > 0 ? entries.Count : 1.0d);
            return result + (long)(Math.Ceiling(count / 16.0d)) * BootSector.SectorSize;
        }

        public void CopyArchive(DirectoryEntry entry, String dest)
        {
            if (entry.IsArchive)
            {
                long offset;
                bool created;
                DirectoryEntry newEntry;
                newEntry = CreateEntry(dest, entry.Attributes, entry.FileSize, out offset, out created);
                if (created)
                {
                    float timeToWait;
                    Stopwatch watch = new Stopwatch();
                    watch.Reset();
                    watch.Start();
                    CopyData(entry.FirstCluster, newEntry.FirstCluster);
                    watch.Stop();
                    lock (padlock)
                    {
                        transferRate = (float)entry.FileSize / (float)(watch.Elapsed.TotalSeconds * 1048576.0F);
                    }
                    timeToWait = (float)entry.FileSize / (maxRateAllowed * 1048576.0F);
                    timeToWait -= (float)watch.Elapsed.TotalSeconds;
                    if (timeToWait > 0)
                        Thread.Sleep((int)timeToWait * 1000);
                }
            }
        }

        public void CopyFolder(DirectoryEntry entry, String dest)
        {
            if (entry.IsFolder)
            {
                long offset;
                bool created;
                DirectoryEntry newEntry;
                DirectoryEntry subEntry;
                newEntry = CreateEntry(dest, entry.Attributes, 0, out offset, out created);
                if (created)
                {
                    Dictionary<String, DirectoryEntry> table = DirectoryLookupTable(entry.FirstCluster, false);
                    foreach (String str in table.Keys)
                    {
                        table.TryGetValue(str, out subEntry);
                        if (subEntry.IsArchive)
                            CopyArchive(subEntry, dest + "\\" + str);
                        else if (subEntry.IsFolder)
                            CopyFolder(subEntry, dest + "\\" + str);
                    }
                }
            }
        }

        public Dictionary<String, DirectoryEntry> GetDirectoryTable(String path)
        {
            long offset;
            DirectoryEntry entry = FindEntryAbsolute(path, out offset);
            if (entry.IsFolder)
                return DirectoryLookupTable(entry.FirstCluster, false);
            else
                return new Dictionary<string, DirectoryEntry>();
        }

        #endregion

        #region Private Methods

        private void DeleteFromDirectory(DirectoryEntry parent, DirectoryEntry entry)
        {
            uint currentCluster = parent.FirstCluster;
            DirectoryEntry currentEntry;
            Sector sector = new Sector(currentCluster);
            bool end = false;
            bool found = false;
            uint entryOffset;
            uint entryCluster = 0;

            long offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               (int)BootSector.SectorSize,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
            }

            while (!end)
            {
                // walk the sector and extract directory entries
                entryOffset = 0;
                do
                {
                    currentEntry = new DirectoryEntry();
                    currentEntry.SetDirectoryEntry(
                        sector.SubArray(entryOffset, DirectoryEntry.EntrySize));
                    if (!found && currentEntry.Equals(entry))
                    {
                        found = true;
                        entryCluster = currentCluster;
                        currentEntry.DeleteEntry();
                        UpdateDirectoryEntry(currentEntry, offset + entryOffset);
                    }
                    else if (found && currentEntry.Type == ENTRYTYPE.LFN)
                    {
                        currentEntry.DeleteEntry();
                        UpdateDirectoryEntry(currentEntry, offset + entryOffset);
                    }
                    else if (found && currentEntry.Type != ENTRYTYPE.LFN) break;
                    entryOffset += DirectoryEntry.EntrySize;
                } while (entryOffset < BootSector.SectorSize && currentEntry.Type != ENTRYTYPE.EOD);
                // follow the cluster chain
                if (entryOffset == BootSector.SectorSize)
                {
                    currentCluster = fat.FindNextCluster(currentCluster);
                    if (currentCluster < FAT.Size)
                    {
                        offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                        using (FileStream fstream =
                            new FileStream(path,
                                           FileMode.Open,
                                           FileAccess.Read,
                                           FileShare.ReadWrite,
                                           (int)BootSector.SectorSize,
                                           FileOptions.RandomAccess))
                        {
                            fstream.Seek(offset, SeekOrigin.Begin);
                            sector.SectorNumber = currentCluster;
                            sector.ReadSector(fstream);
                        }
                    }
                    else
                        end = true;
                }
                else
                    end = true;
            }
            if (found && entryCluster != currentCluster)
            {
                int count = GetDirectoryEntryCount(currentCluster);
                if (count == 0)
                    fat.ReleaseClusterChainTail(entryCluster);
            }
            else if (found && currentCluster != parent.FirstCluster)
            {
                int count = GetDirectoryEntryCount(currentCluster);
                if (count == 0)
                    fat.ReleaseClusterChainPrev(parent.FirstCluster, currentCluster);
            }
        }

        private bool CheckDisk()
        {
            FileStream stream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               8, // min buffer size
                               FileOptions.RandomAccess);
            if (stream.Length != diskSize)
            {
                stream.Close();
                return false;
            }
            BinaryReader reader = new BinaryReader(stream);
            // check BootSector signature
            reader.BaseStream.Seek(510, SeekOrigin.Begin);
            ushort signature = reader.ReadUInt16();
            if (signature == 0xAA55)
                formatted = true;
            reader.Close();
            return true;
        }

        private void ResetDisk()
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                byte[] zero = new byte[1024];
                int count = (int)diskSize >> 10;
                for (int i = 0; i < count; i++)
                    stream.Write(zero, 0, 1024);
                bs.Reset();
                fsi.Reset();
                fat.Reset();
                stream.Seek(0, SeekOrigin.Begin);
                bs.WriteBootSector(stream);
                stream.Seek(bs.FSISector * bs.BytesPerSector, SeekOrigin.Begin);
                fsi.WriteFileSystemInfo(stream);
                stream.Seek(bs.ReservedSectors * bs.BytesPerSector, SeekOrigin.Begin);
                fat.WriteFAT(stream, true);
                fat.WriteFAT(stream, true);
            }
        }

        private void CopyData(uint srcCluster, uint destCluster)
        {
            if (srcCluster >= FAT.Size || destCluster >= FAT.Size)
                throw new ArgumentException("Numero de cluster invalido");
            if (srcCluster < bs.RootStartCluster || destCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invaido");
            Sector sector = new Sector();
            long offset;
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.ReadWrite,
                               FileShare.ReadWrite,
                               4096, // default buffer size
                               FileOptions.RandomAccess))
            {
                do
                {
                    offset = dataStartOffset + (srcCluster * BootSector.SectorSize);
                    fstream.Seek(offset, SeekOrigin.Begin);
                    sector.ReadSector(fstream);
                    offset = dataStartOffset + (destCluster * BootSector.SectorSize);
                    fstream.Seek(offset, SeekOrigin.Begin);
                    sector.WriteSector(fstream);
                    srcCluster = fat.FindNextCluster(srcCluster);
                    if (srcCluster < FAT.Size)
                        destCluster = fat.AllocNewCluster(destCluster);
                } while (srcCluster < FAT.Size);
            }
        }

        private void DeleteAllSubEntries(uint firstCluster)
        {
            if (firstCluster >= FAT.Size || firstCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint currentCluster = firstCluster;
            DirectoryEntry entry;
            Sector sector = new Sector(firstCluster);
            bool eodReached = false;
            uint entryOffset;

            long offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               (int)BootSector.SectorSize,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
            }

            while (!eodReached)
            {
                // walk the sector and extract directory entries
                entryOffset = 0;
                do
                {
                    entry = new DirectoryEntry();
                    entry.SetDirectoryEntry(
                        sector.SubArray(entryOffset, DirectoryEntry.EntrySize));
                    if (entry.Type == ENTRYTYPE.SFN && entry.IsFolder)
                        DeleteAllSubEntries(entry.FirstCluster);
                    else if (entry.Type == ENTRYTYPE.SFN && !entry.IsFolder)
                        DeleteFile(entry);
                    if (entry.Type != ENTRYTYPE.EOD)
                        UpdateDirectoryEntry(new DirectoryEntry(), offset + entryOffset);
                    entryOffset += DirectoryEntry.EntrySize;
                } while (entryOffset < BootSector.SectorSize && entry.Type != ENTRYTYPE.EOD);
                // follow the cluster chain
                if (entry.Type != ENTRYTYPE.EOD)
                {
                    currentCluster = fat.FindNextCluster(currentCluster);
                    if (currentCluster < FAT.Size)
                    {
                        offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                        using (FileStream fstream =
                            new FileStream(path,
                                           FileMode.Open,
                                           FileAccess.Read,
                                           FileShare.ReadWrite,
                                           (int)BootSector.SectorSize,
                                           FileOptions.RandomAccess))
                        {
                            fstream.Seek(offset, SeekOrigin.Begin);
                            sector.SectorNumber = currentCluster;
                            sector.ReadSector(fstream);
                        }
                    }
                    else
                        eodReached = true;
                }
                else
                    eodReached = true;
            }
            fat.ReleaseClusterChain(firstCluster);
        }

        private void DeleteFile(DirectoryEntry entry)
        {
            uint currentCluster = entry.FirstCluster;
            Sector sector = new Sector();
            long offset;
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Write,
                               FileShare.ReadWrite,
                               (int)BootSector.SectorSize,
                               FileOptions.RandomAccess))
            {
                while (currentCluster < FAT.Size)
                {
                    offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                    fstream.Seek(offset, SeekOrigin.Begin);
                    sector.WriteSector(fstream);
                    currentCluster = fat.FindNextCluster(currentCluster);
                }
            }
            fat.ReleaseClusterChain(entry.FirstCluster);
        }

        private uint GetEntrySlot(uint startCluster, out uint start, int size)
        {
            if (size < 1)
                throw new ArgumentException("size debe ser mayor que cero");
            if (startCluster >= FAT.Size || startCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint cluster = startCluster;
            uint slotCluster = startCluster;
            long offset;
            byte head = 0xFF, prevHead;
            int count = 0;
            Sector sector = new Sector(startCluster);

            offset = dataStartOffset + (cluster * BootSector.SectorSize);
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Read,
                               FileShare.ReadWrite,
                               (int)BootSector.SectorSize,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                sector.ReadSector(fstream);
            }

            bool found = false;
            uint entryNumber = 0;
            uint entryOffset;
            start = 0;

            while (!found)
            {
                // walk the sector and extract directory entries
                entryNumber = 0;
                entryOffset = 0;
                do
                {
                    prevHead = head;
                    head = sector.PeekByte(entryOffset);
                    if (head == 0xE5 && prevHead != 0xE5)
                    {
                        count = 1;
                        start = entryNumber;
                        slotCluster = cluster;
                    }
                    else if (head == 0x00 && prevHead != 0xE5 && prevHead != 0x00)
                    {
                        count = 1;
                        start = entryNumber;
                        slotCluster = cluster;
                    }
                    else if (head == 0xE5 && prevHead == 0xE5)
                    {
                        count++;
                    }
                    else if (head == 0x00 && (prevHead == 0xE5 || prevHead == 0x00))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        start = 0;
                    }
                    entryNumber++;
                    entryOffset += DirectoryEntry.EntrySize;
                } while (entryOffset < BootSector.SectorSize && count != size);
                // follow the cluster chain
                if (count != size)
                {
                    cluster = fat.FindNextCluster(cluster);
                    if (cluster >= FAT.Size)
                    {
                        if (count > 0)
                        {
                            fat.AllocNewCluster(startCluster);
                            return slotCluster;
                        }
                        return fat.AllocNewCluster(startCluster);
                    }
                    offset = dataStartOffset + (cluster * BootSector.SectorSize);
                    using (FileStream fstream =
                        new FileStream(path,
                                       FileMode.Open,
                                       FileAccess.Read,
                                       FileShare.ReadWrite,
                                       (int)BootSector.SectorSize,
                                       FileOptions.RandomAccess))
                    {
                        fstream.Seek(offset, SeekOrigin.Begin);
                        sector.SectorNumber = cluster;
                        sector.ReadSector(fstream);
                    }
                }
                else
                    found = true;
            }
            return slotCluster;
        }

        private uint WriteEntries(List<DirectoryEntry> newEntries, uint dirCluster, bool alloc)
        {
            if (dirCluster >= FAT.Size || dirCluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint cluster = dirCluster;
            uint currentCluster;
            uint entryOffset;
            uint entryCluster;

            cluster = GetEntrySlot(cluster, out entryOffset, newEntries.Count);

            long offset = dataStartOffset +
                          (cluster * BootSector.SectorSize) +
                          (entryOffset * DirectoryEntry.EntrySize);

            newEntries[0].Offset = (byte)entryOffset;
            if (alloc)
            {
                entryCluster = fat.FindFreeCluster();
                newEntries[0].FirstCluster = entryCluster;
                fat.AllocCluster(entryCluster);
            }
            using (FileStream fstream =
                new FileStream(path,
                               FileMode.Open,
                               FileAccess.Write,
                               FileShare.ReadWrite,
                               (int)BootSector.SectorSize,
                               FileOptions.RandomAccess))
            {
                fstream.Seek(offset, SeekOrigin.Begin);
                newEntries[0].WriteDirectoryEntry(fstream);

                currentCluster = cluster;
                for (int i = 1; i < newEntries.Count; i++)
                {
                    if (entryOffset + i == 16)
                    {
                        entryOffset = 0;
                        currentCluster = fat.FindNextCluster(currentCluster);
                        if (currentCluster >= FAT.Size)
                            currentCluster = fat.AllocNewCluster(dirCluster);
                        offset = dataStartOffset + (currentCluster * BootSector.SectorSize);
                        fstream.Seek(offset, SeekOrigin.Begin);
                    }
                    newEntries[i].Offset = (byte)(entryOffset + i);
                    newEntries[i].WriteDirectoryEntry(fstream);
                }
            }
            return newEntries[0].FirstCluster;
        }

        #endregion
    }
}
