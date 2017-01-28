using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace VirtualDrive.FileSystem.FAT32
{
    internal class BootSector
    {
        #region Fields

        private static uint sectorSize = 512;

        private byte[] contents;

        #endregion

        #region Constructor

        public BootSector()
        {
            contents = new byte[sectorSize];

            Reset();
        }

        #endregion

        #region Properties

        public static uint SectorSize { get { return sectorSize; } }

        public ushort BytesPerSector
        {
            get { return BitConverter.ToUInt16(contents, 11); }
        }

        public ushort ReservedSectors
        {
            get { return BitConverter.ToUInt16(contents, 14); }
        }

        public byte NumberOfFATs { get { return contents[16]; } }

        public uint NumberOfSectors
        {
            get { return BitConverter.ToUInt32(contents, 32); }
        }

        public uint SectorsPerFAT
        {
            get { return BitConverter.ToUInt32(contents, 36); }
        }

        public bool Mirroring
        {
            get { return (contents[40] & 0x80) != 0; }
        }

        public byte ActiveFAT
        {
            get
            {
                if (Mirroring)
                    return (byte)(contents[40] & 0x0F);
                else
                    return 0xFF;
            }
        }

        public uint RootStartCluster
        {
            get { return BitConverter.ToUInt32(contents, 44); }
        }

        public ushort FSISector
        {
            get { return BitConverter.ToUInt16(contents, 48); }
        }

        public ushort BootCopySector
        {
            get { return BitConverter.ToUInt16(contents,50); }
        }

        public byte DriveNumber
        {
            get { return (byte)(contents[64] & 0x7F); }
        }

        public uint SerialNumber
        {
            get { return BitConverter.ToUInt32(contents,67); }
        }

        public string VolumeLabel
        {
            get { return Encoding.ASCII.GetString(contents, 71, 11); }
            set
            {
                if (value.Length > 11)
                    throw new ArgumentException("Etiqueta de volumen invalida");
                if (value.Length < 11)
                    value.PadRight(11, ' ');
                Array.Copy(Encoding.ASCII.GetBytes(value), 0, contents, 71, 11);
            }
        }

        #endregion

        #region Public Methods

        public void ReadBootSector(FileStream stream)
        {
            int bytesRead = stream.Read(contents, 0, (int)sectorSize);
            if (bytesRead < sectorSize)
                throw new InvalidDataException("Sector de arranque invalido");
            // check signature
            if (BitConverter.ToUInt16(contents, 510) != 0xAA55)
                throw new InvalidDataException("Sector de arranque invalido");
            if (contents[66] != 0x29)
                throw new InvalidDataException("Sector de arranque invalido");
        }

        public void WriteBootSector(FileStream stream)
        {
            stream.Write(contents, 0, (int)sectorSize);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the default values for the boot sector.
        /// </summary>
        public void Reset()
        {
            contents[0] = 0xEB; // Machine code for jump over the data. (3 bytes)
            contents[1] = 0x58;
            contents[2] = 0x90;

            contents[3] = (byte)'E'; // OEM name string. (8 bytes)
            contents[4] = (byte)'X';
            contents[5] = (byte)'P';
            contents[6] = (byte)'L';
            contents[7] = (byte)'O';
            contents[8] = (byte)'R';
            contents[9] = (byte)'E';
            contents[10] = (byte)'R';

            // BPB (Bios Parameter Block) start
            contents[11] = 0x00; // Bytes per sector. (2 bytes)
            contents[12] = 0x02; // Nearly always 512 but can be 1024,2048 or 4096.

            contents[13] = 0x01; // Sectors per cluster. (1 byte)

            contents[14] = 0x20; // Reserved sectors. (2 bytes)
            contents[15] = 0x00; // Usually 32 for FAT32 volumes.

            contents[16] = 0x02; // Number of FAT's. (1 byte)

            contents[17] = 0x00; // Maximum number of root directory 
            contents[18] = 0x00; // entries. (2 bytes)

            contents[19] = 0x00; // Old 16-bit total count of sectors
            contents[20] = 0x00; // on the volume. (2 bytes)

            contents[21] = 0xF8; // Media descriptor byte. (Not used)

            contents[22] = 0x00; // FAT12/16: Sectors per FAT. (2 bytes)
            contents[23] = 0x00; // 0 for FAT32

            contents[24] = 0x00; // Sectors per track. (2 bytes)
            contents[25] = 0x00;

            contents[26] = 0x00; // Total number of heads/sides. (2 bytes)
            contents[27] = 0x00;

            contents[28] = 0x00; // Number of hidden sectors. (4 bytes)
            contents[29] = 0x00;
            contents[30] = 0x00;
            contents[31] = 0x00;

            contents[32] = 0x00; // Total number of sectors. (4 bytes)
            contents[33] = 0x00; // Drive Capacity: 32 MB, Bytes per Sector: 512
            contents[34] = 0x01; // 32 MB / 512 B = 65536 sectors
            contents[35] = 0x00;
            // BPB (Bios Parameter Block) end

            contents[36] = 0x00; // FAT32: sectors per FAT. (4 bytes)
            contents[37] = 0x02; // Total Number of sectors = 64 K
            contents[38] = 0x00; // FAT Entry width = 4 B
            contents[39] = 0x00; // FatSz32 = 256 KB / 512 B per sector = 512 sectors

            contents[40] = 0x80; // Mirror flags. (2 bytes)
            contents[41] = 0x00; // bits 0-3: number of active FAT (if bit 7 is 1)
                                 // bit    7: 1 -> single active FAT
                                 //           0 -> all FATs are updated at runtime

            contents[42] = 0x00; // Minor revision
            contents[43] = 0x00; // Major revision

            contents[44] = 0x02; // Root directory starting cluster. (4 bytes)
            contents[45] = 0x00;
            contents[46] = 0x00;
            contents[47] = 0x00;

            contents[48] = 0x01; // File system information sector
            contents[49] = 0x00; // number in FAT32 reserved area.

            contents[50] = 0x06; // If non-zero this gives the sector which
            contents[51] = 0x00; // holds a copy of the boot record.

            contents[64] = 0x80; // Physical drive number. (BIOS system)

            contents[66] = 0x29; // Extended boot signature.

            contents[67] = 0x00; // Serial number. (4 bytes)
            contents[68] = 0x00;
            contents[69] = 0x00;
            contents[70] = 0x00;

            contents[71] = (byte)'V'; // Volume label. (11 bytes)
            contents[72] = (byte)'i';
            contents[73] = (byte)'r';
            contents[74] = (byte)'t';
            contents[75] = (byte)'u';
            contents[76] = (byte)'a';
            contents[77] = (byte)'l';
            contents[78] = (byte)'D';
            contents[79] = (byte)'i';
            contents[80] = (byte)'s';
            contents[81] = (byte)'k';

            contents[82] = (byte)'F'; // File system ID. (8 bytes)
            contents[83] = (byte)'A';
            contents[84] = (byte)'T';
            contents[85] = (byte)'3';
            contents[86] = (byte)'2';
            contents[87] = 0x20;
            contents[88] = 0x20;
            contents[89] = 0x20;

            contents[510] = 0x55; // Boot Signature. (2 bytes)
            contents[511] = 0xAA;
        }

        #endregion
    }
}
