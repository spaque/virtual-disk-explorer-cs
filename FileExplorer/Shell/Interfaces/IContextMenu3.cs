using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The IContextMenu3 interface is used to create or merge a shortcut 
    /// menu associated with a certain object when the menu implementation 
    /// needs to process the WM_MENUCHAR message.
    /// </summary>
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719")]
    public interface IContextMenu3 : IContextMenu2
    {
        /// <summary>
        /// Allows client objects of the IContextMenu3 interface to 
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
        /// <param name="plResult">
        /// Address of an LRESULT value that the owner of the menu will 
        /// return from the message. This parameter can be NULL.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 HandleMenuMsg2(
            uint uMsg,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr plResult);
    }
}