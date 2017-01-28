using System;
using System.Collections.Generic;
using System.IO;

namespace VirtualDrive.FileSystem.FAT32
{
    internal class Sector
    {
        #region Fields

        private byte[] data;
        private uint sectorNumber;
        private uint writes;
        private uint reads;
        bool modified;

        #endregion

        #region Constructors

        public Sector()
        {
            data = new byte[BootSector.SectorSize];
            sectorNumber = 0xFFFFFFFF;
        }

        public Sector(uint sector)
        {
            data = new byte[BootSector.SectorSize];
            sectorNumber = sector;
        }

        #endregion

        #region Properties

        public uint SectorNumber
        {
            get { return sectorNumber; }
            set { sectorNumber = value; }
        }

        public uint Writes { get { return writes; } }

        public uint Reads { get { return reads; } }

        public bool Modified { get { return modified; } }

        #endregion

        #region Public Methods

        public void ReadSector(FileStream stream)
        {
            stream.Read(data, 0, (int)BootSector.SectorSize);
        }

        public void WriteSector(FileStream stream)
        {
            stream.Write(data, 0, (int)BootSector.SectorSize);
        }

        public void CopyToSector(int srcIndex, byte[] src, uint index, int length)
        {
            Array.Copy(src, srcIndex, data, index, length);
            writes++;
            modified = true;
        }

        public void CopyFromSector(uint index, byte[] dest, int destIndex, int length)
        {
            Array.Copy(data, index, dest, destIndex, length);
            reads++;
        }

        public byte[] SubArray(uint start, uint count)
        {
            if (start + count > BootSector.SectorSize)
                throw new ArgumentException("Indices invalidos");
            byte[] result = new byte[count];
            Array.Copy(data, start, result, 0, count);
            return result;
        }

        public byte PeekByte(uint index)
        {
            if (index > BootSector.SectorSize)
                throw new IndexOutOfRangeException("Indice fuera de rango");
            return data[index];
        }

        #endregion
    }
}
