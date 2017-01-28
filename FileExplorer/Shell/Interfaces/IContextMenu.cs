using System;
using System.Text;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The IContextMenu interface is called by the Shell to either create 
    /// or merge a shortcut menu associated with a Shell object.
    /// </summary>
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    public interface IContextMenu
    {
        /// <summary>
        /// Adds commands to a shortcut menu.
        /// </summary>
        /// <param name="hmenu">
        /// Handle to the menu. 
        /// The handler should specify this handle when adding menu items.
        /// </param>
        /// <param name="iMenu">
        /// Zero-based position at which to insert the first menu item. 
        /// </param>
        /// <param name="idCmdFirst">
        /// Minimum value that the handler can specify for a menu item identifier.
        /// </param>
        /// <param name="idCmdLast">
        /// Maximum value that the handler can specify for a menu item identifier.
        /// </param>
        /// <param name="uFlags">
        /// Optional flags specifying how the shortcut menu can be changed.
        /// </param>
        /// <returns>
        /// If successful, returns an HRESULT value that has its severity 
        /// value set to SEVERITY_SUCCESS and its code value set to the 
        /// offset of the largest command identifier that was assigned, plus one.
        /// </returns>
        [PreserveSig()]
        Int32 QueryContextMenu(
            IntPtr hmenu,
            uint   iMenu,
            uint   idCmdFirst,
            uint   idCmdLast,
            ShellAPI.CMF uFlags);

        /// <summary>
        /// Carries out the command associated with a shortcut menu item.
        /// </summary>
        /// <param name="info">
        /// Pointer to a CMINVOKECOMMANDINFO or CMINVOKECOMMANDINFOEX 
        /// structure containing information about the command.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or an OLE-defined error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 InvokeCommand(
            ref ShellAPI.CMINVOKECOMMANDINFOEX info);

        /// <summary>
        /// Retrieves information about a shortcut menu command, 
        /// including the help string and the language-independent, 
        /// or canonical, name for the command.
        /// </summary>
        /// <param name="idCmd">
        /// Menu command identifier offset.
        /// </param>
        /// <param name="uFlags">
        /// Flags specifying the information to return.
        /// </param>
        /// <param name="pwReserved">
        /// Reserved. Applications must specify NULL when calling this method, 
        /// and handlers must ignore this parameter when called.
        /// </param>
        /// <param name="pszName">
        /// Address of the buffer to receive the null-terminated 
        /// string being retrieved.
        /// </param>
        /// <param name="cchMax">
        /// Size of the buffer, in characters, to 
        /// receive the null-terminated string.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, 
        /// or an OLE-defined error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 GetCommandString(
            uint idCmd,
            ShellAPI.GCS  uFlags,
            uint pwReserved,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder pszName,
            int  cchMax);
    }
}