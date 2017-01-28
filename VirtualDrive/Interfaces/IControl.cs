using System;
using System.Collections.Generic;
using VirtualDrive.Shell;

namespace VirtualDrive.Interfaces
{
    public interface IControl
    {
        String Dir(String pattern);

        bool CreateDir(String path);

        bool DeleteDir(String path, bool forceDelete);

        bool Delete(String path);

        String Copy(String src, String dest, bool folders);

        String Move(String src, String dest, bool folders);
    }
}
