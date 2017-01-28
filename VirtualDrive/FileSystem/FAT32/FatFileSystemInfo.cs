using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VirtualDrive.FileSystem.FAT32
{
    internal class FatFileSystemInfo
    {
        #region Fields

        private byte[] contents;

        #endregion

        #region Constructor

        public FatFileSystemInfo()
        {
            Reset();
        }

        #endregion

        #region Properties

        public uint FreeClusters
        {
            get { return BitConverter.ToUInt32(contents, 488); }
            set { Array.ConstrainedCopy(BitConverter.GetBytes(value), 0, contents, 488, 4); }
        }

        public uint NextFreeCluster
        {
            get { return BitConverter.ToUInt32(contents, 492); }
            set { Array.ConstrainedCopy(BitConverter.GetBytes(value), 0, contents, 492, 4); }
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            contents = new byte[BootSector.SectorSize];

            contents[0] = 0x52; // Leading signature
            contents[1] = 0x52;
            contents[2] = 0x61;
            contents[3] = 0x41;

            contents[484] = 0x72; // FSI structure signature
            contents[485] = 0x72;
            contents[486] = 0x41;
            contents[487] = 0x61;

            contents[488] = 0xFF; // Last known count of free clusters,
            contents[489] = 0xFF; // or 0xFFFFFFFFh if it's unknown
            contents[490] = 0xFF;
            contents[491] = 0xFF;

            contents[492] = 0xFF; // Next free cluster (hint), or
            contents[493] = 0xFF; // 0xFFFFFFFFh if the field has not been set.
            contents[494] = 0xFF;
            contents[495] = 0xFF;

            contents[508] = 0x00; // Trailing signature
            contents[509] = 0x00;
            contents[510] = 0x55;
            contents[511] = 0xAA;
        }

        public void ReadFileSystemInfo(FileStream stream)
        {
            int bytesRead = stream.Read(contents, 0, (int)BootSector.SectorSize);
            if (bytesRead < BootSector.SectorSize)
                throw new InvalidDataException("Estructura FSI invalida");
            // check signatures
            if (BitConverter.ToUInt32(contents, 0) != 0x41615252)
                throw new InvalidDataException("Estructura FSI invalida");
            if (BitConverter.ToUInt32(contents, 484) != 0x61417272)
                throw new InvalidDataException("Estructura FSI invalida");
            if (BitConverter.ToUInt32(contents, 508) != 0xAA550000)
                throw new InvalidDataException("Estructura FSI invalida");
        }

        public void WriteFileSystemInfo(FileStream stream)
        {
            stream.Write(contents, 0, (int)BootSector.SectorSize);
        }

        #endregion
    }
}
