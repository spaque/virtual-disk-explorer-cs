using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The Shell uses the IQueryInfo interface to retrieve flags and info tip 
    /// information for an item that resides in an IShellFolder implementation.
    /// </summary>
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00021500-0000-0000-C000-000000000046")]
    public interface IQueryInfo
    {
        /// <summary>
        /// Retrieves the info tip text for an item.
        /// </summary>
        /// <param name="dwFlags">
        /// Flags directing the handling of the item from 
        /// which you're retrieving the info tip text.
        /// </param>
        /// <param name="ppwszTip">
        /// Address of a Unicode string pointer that 
        /// receives the tip string pointer.
        /// </param>
        /// <returns>
        /// Returns S_OK if the function succeeds. 
        /// If no info tip text is available, ppwszTip is set to NULL. 
        /// Otherwise, returns an OLE-defined error value.
        /// </returns>
        [PreserveSig]
        Int32 GetInfoTip(
            ShellAPI.QITIPF dwFlags,
            [MarshalAs(UnmanagedType.LPWStr)]
            out string ppwszTip);

        /// <summary>
        /// Retrieves the information flags for an item.
        /// </summary>
        /// <param name="pdwFlags">
        /// Pointer to a value that receives the flags for the item. 
        /// If no flags are to be returned, this value should be set to zero.
        /// </param>
        /// <returns>
        /// Returns S_OK if pdwFlags returns any flag values, 
        /// or an OLE-defined error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetInfoFlags(
            out IntPtr pdwFlags);
    }
}