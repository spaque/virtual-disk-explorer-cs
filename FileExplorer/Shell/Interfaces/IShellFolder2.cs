using System;
using System.Runtime.InteropServices;
using FileExplorer.Shell;

namespace FileExplorer.Shell.Interfaces
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("93F2F68C-1D1B-11D3-A30E-00C04F79ABD1")]
    public interface IShellFolder2 : IShellFolder
    {
        /// <summary>
        /// Requests a pointer to an interface that allows a client 
        /// to enumerate the available search objects.
        /// </summary>
        /// <param name="ppEnum">
        /// Address of a pointer to an enumerator object's 
        /// IEnumExtraSearch interface.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or an OLE error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 EnumSearches(
            out IntPtr ppEnum);

        /// <summary>
        /// Retrieves the default sorting and display columns.
        /// </summary>
        /// <param name="dwReserved">
        /// Reserved. Set to zero.
        /// </param>
        /// <param name="sort">
        /// Value that receives the index of the default sorted column.
        /// </param>
        /// <param name="display">
        /// Value that receives the index of the default display column.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or a COM error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDefaultColumn(
            uint dwReserved,
            out uint sort,
            out uint display);

        /// <summary>
        /// Retrieves the default state for a specified column.
        /// </summary>
        /// <param name="iColumn">
        /// Integer that specifies the column number.
        /// </param>
        /// <param name="csFlags">
        /// Value that contains flags that indicate the default column state.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDefaultColumnState(
            uint iColumn,
            out ShellAPI.SHCOLSTATEF csFlags);

        /// <summary>
        /// Returns the globally unique identifier (GUID) of 
        /// the default search object for the folder.
        /// </summary>
        /// <param name="guid">
        /// GUID of the default search object.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or an OLE error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDefaultSearchGUID(
            out Guid guid);

        /// <summary>
        /// Retrieves detailed information, identified by a property set 
        /// identifier (FMTID) and a property identifier (PID), on 
        /// an item in a Shell folder.
        /// </summary>
        /// <param name="pidl">
        /// PIDL of the item, relative to the parent folder. 
        /// This method accepts only single-level PIDLs. 
        /// The structure must contain exactly one SHITEMID 
        /// structure followed by a terminating zero.
        /// </param>
        /// <param name="pscid">
        /// Pointer to an SHCOLUMNID structure that identifies the column.
        /// </param>
        /// <param name="pv">
        /// Pointer to a VARIANT with the requested information. 
        /// The value will be fully typed.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDetailsEx(
            IntPtr pidl,
            IntPtr pscid,
            out object pv);

        /// <summary>
        /// Retrieves detailed information, identified by a column 
        /// index, on an item in a Shell folder.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to an item identifier list (PIDL) of the item for which 
        /// you are requesting information. This method accepts only single-level 
        /// PIDLs. The structure must contain exactly one SHITEMID structure 
        /// followed by a terminating zero. If this parameter is set to NULL,
        /// the title of the information field specified by iColumn is returned.
        /// </param>
        /// <param name="iColumn">
        /// Zero-based index of the desired information field. 
        /// It is identical to the column number of the information as 
        /// it is displayed in a Microsoft Windows Explorer Details view.
        /// </param>
        /// <param name="sd">
        /// Pointer to a SHELLDETAILS structure that contains the information.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDetailsOf(
            IntPtr pidl,
            uint iColumn,
            out ShellAPI.SHELLDETAILS sd);

        /// <summary>
        /// Converts a column to the appropriate property 
        /// set ID (FMTID) and property ID (PID).
        /// </summary>
        /// <param name="iColumn">
        /// Column ID.
        /// </param>
        /// <param name="scid">
        /// Pointer to an SHCOLUMNID structure containing the FMTID and PID.
        /// </param>
        /// <returns>
        /// Returns NOERROR if successful, or a COM error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 MapColumnToSCID(
            uint iColumn,
            out ShellAPI.SHCOLUMNID scid);
    }
}
