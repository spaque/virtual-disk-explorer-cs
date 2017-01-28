using System;
using System.Runtime.InteropServices;
using FileExplorer.Shell;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The IShellFolder interface is used to manage folders. It is 
    /// exposed by all Shell namespace folder objects.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder
    {
        /// <summary>
        /// Translates the display name of a file object or a folder 
        /// into an item identifier list.
        /// </summary>
        /// <param name="hwnd">
        /// Optional window handle
        /// </param>
        /// <param name="pbc">
        /// A pointer to a bind context object that 
        /// controls the parsing operation. This parameter is typically set to NULL.
        /// </param>
        /// <param name="pszDisplayName">
        /// Null-terminated Unicode string with the display name.
        /// </param>
        /// <param name="pchEaten">
        /// Pointer to a ULONG value that receives the number of characters of
        /// the display name that was parsed. If NULL, no value returned.
        /// </param>
        /// <param name="ppidl">
        /// Pointer to an ITEMIDLIST pointer that receives the item 
        /// identifier list for the object.
        /// </param>
        /// <param name="pdwAttributes">
        /// Value used to query for file attributes. 
        /// If not used, it should be set to NULL.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] 
            string pszDisplayName,
            out UInt32 pchEaten,
            out IntPtr ppidl,
            ref ShellAPI.SFGAO pdwAttributes);

        /// <summary>
        /// Allows a client to determine the contents of a folder by creating an 
        /// item identifier enumeration object and returning its IEnumIDList interface.
        /// </summary>
        /// <param name="hwndOwner">
        /// If user input is required to perform the enumeration, this window handle 
        /// should be used by the enumeration object as the parent window to take 
        /// user input. If hwndOwner is set to NULL, the enumerator should not post 
        /// any messages, and if user input is required, it should silently fail.
        /// </param>
        /// <param name="grfFlags">
        /// Flags indicating which items to include in the enumeration. For 
        /// a list of possible values, see the SHCONTF enumerated type.
        /// </param>
        /// <param name="enumIDList">
        /// Address that receives a pointer to the IEnumIDList interface of the 
        /// enumeration object created by this method. If an error occurs or no 
        /// suitable subobjects are found, ppenumIDList is set to NULL.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 EnumObjects(
            IntPtr hwndOwner,
            ShellAPI.SHCONTF grfFlags,
            out IntPtr penumIDList); 

        /// <summary>
        /// Retrieves an IShellFolder object for a subfolder.
        /// </summary>
        /// <param name="pidl">
        /// Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.
        /// The structure will contain one or more SHITEMID structures, 
        /// followed by a terminating NULL.
        /// </param>
        /// <param name="pbc">
        /// Address of an IBindCtx interface on a bind context object to be used 
        /// during this operation. If this parameter is not used, set it to NULL.
        /// </param>
        /// <param name="riid">
        /// Identifier of the interface to return.
        /// </param>
        /// <param name="ppv">
        /// Address that receives the interface pointer. If an error 
        /// occurs, a NULL pointer is returned in this address.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);
        
        /// <summary>
        /// Requests a pointer to an object's storage interface.
        /// </summary>
        /// <param name="pidl">
        /// Address of an ITEMIDLIST structure that identifies the subfolder 
        /// relative to its parent folder. The structure must contain exactly 
        /// one SHITEMID structure followed by a terminating zero.
        /// </param>
        /// <param name="pbc">
        /// Optional address of an IBindCtx interface on a bind context object 
        /// to be used during this operation. If this parameter is not used, 
        /// set it to NULL.
        /// </param>
        /// <param name="riid">
        /// Interface identifier (IID) of the requested storage interface. To retrieve 
        /// an IStream, IStorage, or IPropertySetStorage interface pointer, set riid 
        /// to IID_IStream, IID_IStorage, or IID_IPropertySetStorage, respectively.
        /// </param>
        /// <param name="ppv">
        /// Address that receives the interface pointer specified by riid. 
        /// If an error occurs, a NULL pointer is returned in this address.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Determines the relative order of two file objects or folders, 
        /// given their item identifier lists.
        /// </summary>
        /// <param name="lParam">
        /// Value that specifies how the comparison should be performed.
        /// The lower 16 bits of lParam define the sorting rule. The default zero
        /// value indicates that the two items should be compared by name.
        /// The upper sixteen bits of lParam are used for flags that modify the 
        /// sorting rule. Values can be from the SHCIDS enum.
        /// </param>
        /// <param name="pidl1">
        /// Pointer to the first item's ITEMIDLIST structure.
        /// </param>
        /// <param name="pidl2">
        /// Pointer to the second item's ITEMIDLIST structure.
        /// </param>
        /// <returns>
        /// If this method is successful, the CODE field of the HRESULT contains 
        /// one of the following values (the code can be retrived using the 
        /// helper function GetHResultCode):
        /// Negative. A negative return value indicates that the first item 
        ///           should precede the second (pidl1 < pidl2).
        /// Positive. A positive return value indicates that the first item 
        ///           should follow the second (pidl1 > pidl2).
        /// Zero.     A return value of zero indicates that the two items 
        ///           are the same (pidl1 = pidl2).
        /// </returns>
        [PreserveSig]
        Int32 CompareIDs(
            IntPtr lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        /// <summary>
        /// Requests an object that can be used to obtain information 
        /// from or interact with a folder object.
        /// </summary>
        /// <param name="hwndOwner">
        /// Handle to the owner window.
        /// </param>
        /// <param name="riid">
        /// Identifier of the requested interface.
        /// </param>
        /// <param name="ppv">
        /// Address of a pointer to the requested interface.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 CreateViewObject(
            IntPtr hwndOwner,
            Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Retrieves the attributes of one or more file or folder objects 
        /// contained in the object represented by IShellFolder.
        /// </summary>
        /// <param name="cidl">
        /// Number of items from which to retrieve attributes.
        /// </param>
        /// <param name="apidl">
        /// Address of an array of pointers to ITEMIDLIST structures, each of 
        /// which uniquely identifies an item relative to the parent folder.
        /// Each ITEMIDLIST structure must contain exactly one SHITEMID 
        /// structure followed by a terminating zero.
        /// </param>
        /// <param name="rgfInOut">
        /// Address of a single ULONG value that, on entry, contains the attributes 
        /// that the caller is requesting. On exit, this value contains the 
        /// requested attributes that are common to all of the specified objects. 
        /// This value can be from the SFGAO enum.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetAttributesOf(
            UInt32 cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref ShellAPI.SFGAO rgfInOut);

        /// <summary>
        /// Retrieves an OLE interface that can be used to carry out 
        /// actions on the specified file objects or folders.
        /// </summary>
        /// <param name="hwndOwner">
        /// Handle to the owner window that the client should 
        /// specify if it displays a dialog box or message box.
        /// </param>
        /// <param name="cidl">
        /// Number of file objects or subfolders specified in the apidl parameter.
        /// </param>
        /// <param name="apidl">
        /// Address of an array of pointers to ITEMIDLIST structures, each of which 
        /// uniquely identifies a file object or subfolder relative to the parent 
        /// folder. Each item identifier list must contain exactly one SHITEMID 
        /// structure followed by a terminating zero.
        /// </param>
        /// <param name="riid">
        /// Identifier of the Component Object Model (COM) interface object to return. 
        /// This can be any valid interface identifier that can be created for an item.
        /// </param>
        /// <param name="rgfReserved">
        /// Reserved.
        /// </param>
        /// <param name="ppv">
        /// Pointer to the requested interface. 
        /// If an error occurs, a NULL pointer is returned in this address.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetUIObjectOf(
            IntPtr hwndOwner,
            UInt32 cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref Guid riid,
            IntPtr rgfReserved,
            out IntPtr ppv);

        /// <summary>
        /// Retrieves the display name for the specified file object or subfolder.
        /// </summary>
        /// <param name="pidl">
        /// Address of an ITEMIDLIST structure (PIDL) that uniquely identifies 
        /// the file object or subfolder relative to the parent folder.
        /// </param>
        /// <param name="uFlags">
        /// Flags used to request the type of display name to return. 
        /// For a list of possible values, see the SHGNO enumerated type.
        /// </param>
        /// <param name="lpName">
        /// Address of a STRRET structure in which to return the display name.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            ShellAPI.SHGNO uFlags, 
            IntPtr lpName);

        /// <summary>
        /// Sets the display name of a file object or subfolder, 
        /// changing the item identifier in the process.
        /// </summary>
        /// <param name="hwndOwner">
        /// Handle to the owner window of any dialog or 
        /// message boxes that the client displays.
        /// </param>
        /// <param name="pidl">
        /// Pointer to an ITEMIDLIST structure that uniquely identifies the file 
        /// object or subfolder relative to the parent folder. The structure must 
        /// contain exactly one SHITEMID structure followed by a terminating zero.
        /// </param>
        /// <param name="pszName">
        /// Pointer to a null-terminated string that specifies the new display name.
        /// </param>
        /// <param name="uFlags">
        /// Flags indicating the type of name specified by the lpszName parameter. 
        /// For a list of possible values, see the description of the SHGNO enum.
        /// </param>
        /// <param name="ppidlOut">
        /// Address of a pointer to an ITEMIDLIST structure that receives the new 
        /// ITEMIDLIST. If you call IShellFolder::SetNameOf with ppidlOut set to NULL, 
        /// it will not return a new ITEMIDLIST for the object. If an error occurs, 
        /// the implementation must set *ppidlOut to NULL (if ppidlOut is non-NULL).
        /// </param>
        /// <returns>
        /// Returns S_OK if successful, or an error value otherwise.
        /// </returns>
        [PreserveSig]
        Int32 SetNameOf(
            IntPtr hwndOwner,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] 
            String pszName,
            ShellAPI.SHGNO uFlags,
            out IntPtr ppidlOut);
    }
}
