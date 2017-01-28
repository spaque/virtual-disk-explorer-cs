using System;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell
{
    public static partial class ShellAPI
    {
        public static Guid CLSID_NewMenu = 
            new Guid("{D969A300-E7FF-11d0-A93B-00A0C90F2719}");

        public static Guid IID_IContextMenu = 
            new Guid("{000214e4-0000-0000-c000-000000000046}");

        public static Guid IID_IContextMenu2 = 
            new Guid("{000214f4-0000-0000-c000-000000000046}");

        public static Guid IID_IContextMenu3 = 
            new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        public static Guid IDD_IEnumIDList =
            new Guid("{000214F2-0000-0000-C000-000000000046}");

        public static Guid IID_IMalloc =
            new Guid("{00000002-0000-0000-C000-000000000046}");

        public static Guid IID_IQueryInfo = 
            new Guid("{00021500-0000-0000-C000-000000000046}");

        public static Guid IID_IShellFolder =
            new Guid("{000214E6-0000-0000-C000-000000000046}");

        public static Guid IID_IShellFolder2 =
            new Guid("{93F2F68C-1D1B-11D3-A30E-00C04F79ABD1}");
    }
}
