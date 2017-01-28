using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The IContextMenu2 interface is used to either create or 
    /// merge a shortcut menu associated with a certain object 
    /// when the menu involves owner-drawn menu items.
    /// </summary>
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214f4-0000-0000-c000-000000000046")]
    public interface IContextMenu2 : IContextMenu
    {
        /// <summary>
        /// Allows client objects of the IContextMenu interface to 
        /// handle messages associated with owner-drawn menu items.
        /// </summary>
        /// <param name="uMsg">
        /// Message to be processed.
        /// </param>
        /// <param name="wParam">
        /// Additional message information.
        /// </param>
        /// <param name="lParam">
        /// Additional message information.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 HandleMenuMsg(
            uint    uMsg,
            IntPtr  wParam,
            IntPtr  lParam);
    }
}