using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// * Reserved sectors include boot sector
//
//                     #sectors     start      size       cummulative size
// Boot Sector      ->  1 sector     0      -> 512 B
// Reserved Sectors -> 32 sectors    0      -> 16384 B    -> 16384 B
// FAT #1           -> 512 sectors   32     -> 262144 B   -> 278528 B
// FAT #2           -> 512 sectors   544    -> 262144 B   -> 540672 B
// Data Area        -> 64479 sectors 1056   -> 33013248 B -> 33553920 B
//
// Count of Clusters -> 64479, according to microsoft this should be FAT16 :)
// Cluster #0 maps to sector 1056
// max value in FAT entry: 64479 = 0xFBDFh

namespace VirtualDrive.FileSystem.FAT32
{
    internal class FAT
    {
        #region Fields

        private static uint FATSize = 64480;

        private uint[] table;

        private FatFileSystemInfo fsi;
        private BootSector bs;

        private bool modified;

        #endregion

        #region Constructor

        public FAT(BootSector bs, FatFileSystemInfo fsi)
        {
            this.bs = bs;
            this.fsi = fsi;
            Reset();
        }

        #endregion

        #region Properties

        public static uint Size { get { return FATSize; } }

        internal bool Modified
        {
            get { return modified; }
            set { modified = value; }
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            table = new uint[FATSize];

            table[0] = 0x0FFFFFF8; // Media descriptor byte
            table[1] = 0x0CFFFFFF; // Dirty volume flags
            table[2] = 0x0FFFFFFF; // Root directory end

            fsi.FreeClusters = FATSize - 3;
            fsi.NextFreeCluster = bs.RootStartCluster + 1;

            modified = true;
        }

        public void ReadFAT(FileStream stream)
        {
            byte[] data = new byte[4];
            for (int i = 0; i < FATSize; i++)
            {
                stream.Read(data, 0, 4);
                table[i] = BitConverter.ToUInt32(data, 0);
            }
        }

        public void WriteFAT(FileStream stream, bool forceWrite)
        {
            if (modified || forceWrite)
            {
                byte[] data = new byte[4];
                for (int i = 0; i < FATSize; i++)
                {
                    data = BitConverter.GetBytes(table[i]);
                    stream.Write(data, 0, 4);
                }
            }
        }

        public uint FindNextCluster(uint cluster)
        {
            if (cluster >= FATSize || cluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint result = table[cluster];
            if (result < bs.RootStartCluster)
                throw new ArgumentException("Cadena de clusters rota, deberia formatear el disco");
            return result;
        }

        public uint AllocNewCluster(uint start)
        {
            if (fsi.FreeClusters == 0)
                throw new InvalidOperationException("No queda espacio libre en el disco");
            if (start >= FATSize || start < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint next;
            uint current = start;

            while (current < FATSize && 
                   !IsEndOfChain(table[current]) &&
                   !IsFreeCluster(table[current]))
            {
                current = table[current];
            }
            next = FindFreeCluster();
            if (next == 0xFFFFFFFF)
                throw new InvalidOperationException("No queda espacio libre en el disco");
            else
            {
                table[current] = next;
                table[next] = 0x0FFFFFFF;
                fsi.FreeClusters--;
                if (next == FATSize - 1)
                    fsi.NextFreeCluster = bs.RootStartCluster;
                else
                    fsi.NextFreeCluster = next + 1;
            }
            modified = true;
            return next;
        }

        public void AllocCluster(uint cluster)
        {
            if (fsi.FreeClusters == 0)
                throw new InvalidOperationException("No queda espacio libre en el disco");
            if (cluster >= FATSize || cluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            if (!IsFreeCluster(table[cluster]))
                throw new ArgumentException("Numero de cluster invalido");
            table[cluster] = 0x0FFFFFFF;
            modified = true;
            fsi.FreeClusters--;
            if (cluster == FATSize - 1)
                fsi.NextFreeCluster = bs.RootStartCluster;
            else
                fsi.NextFreeCluster = cluster + 1;
        }

        public uint FindFreeCluster()
        {
            uint current = fsi.NextFreeCluster;
            uint count = 0;

            if (current == 0xFFFFFFFF)
                current = bs.RootStartCluster;

            while (count < FATSize - 3 && !IsFreeCluster(table[current]))
            {
                count++;
                current++;
                if (current >= FATSize) current = bs.RootStartCluster + 1;
            }

            if (current == FATSize)
                return 0xFFFFFFFF;
            else
                return current & 0x0FFFFFFF; // mask out high 4 bits
        }

        public void ReleaseClusterChain(uint start)
        {
            if (start >= FATSize || start < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint next;
            uint current = start;

            while (current < FATSize && 
                   !IsEndOfChain(table[current]) && 
                   !IsFreeCluster(table[current]))
            {
                next = table[current];
                table[current] = 0x00;
                current = next;
                fsi.FreeClusters++;
            }
            if (IsEndOfChain(table[current]))
            {
                table[current] = 0x00;
                fsi.FreeClusters++;
            }
            modified = true;
        }

        public void ReleaseClusterChainTail(uint start)
        {
            if (start >= FATSize || start < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            uint cluster = table[start];
            if (cluster < FATSize && !IsFreeCluster(table[cluster]))
            {
                ReleaseClusterChain(cluster);
                table[start] = 0x0FFFFFFF;
                modified = true;
            }
        }

        public void ReleaseClusterChainPrev(uint start, uint cluster)
        {
            if (start >= FATSize || start < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            if (cluster >= FATSize || cluster < bs.RootStartCluster)
                throw new ArgumentException("Numero de cluster invalido");
            if (start == cluster) return;

            uint current = start;
            uint next = table[start];
            while (next < FATSize && next != cluster)
            {
                current = next;
                next = table[next];
            }
            if (next == cluster)
                ReleaseClusterChainTail(current);
        }

        #endregion

        #region Private Methods

        private bool IsFreeCluster(uint contents)
        {
            return (contents & 0x0FFFFFFF) == 0;
        }

        private bool IsReservedCluster(uint contents)
        {
            uint cleared = contents & 0x0FFFFFFF;
            return (cleared == 0x01) ||
                   (cleared >= 0x0FFFFFF0 && cleared <= 0x0FFFFFF6);
        }

        private bool IsBadCluster(uint contents)
        {
            return (contents & 0x0FFFFFFF) == 0x0FFFFFF7;
        }

        private bool IsEndOfChain(uint contents)
        {
            return (contents & 0x0FFFFFFF) == 0x0FFFFFFF;
        }

        #endregion
    }
}
