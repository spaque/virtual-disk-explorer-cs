using System;
using System.Text;

using VirtualDrive.FileSystem.FAT32;

namespace VirtualDrive.Interfaces
{
    public interface IData
    {
        int Open(String path);

        int Open(String path, OPENMODE mode);

        int Write(int fd, byte[] buffer, int index, int count);

        int Read(int fd, byte[] buffer, int index, int count);

        void Close(int fd);

        String Contadores();

        void SetRate(float rate);

        float GetRate();
    }
}
