using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3D8B0590-F691-11d2-8EA9-006097DF5BD4")]
    public interface IAsyncOperation
    {
        /// <summary>
        /// Called by a drop source to specify whether the data 
        /// object supports asynchronous data extraction.
        /// </summary>
        /// <param name="fDoOpAsync">
        /// True to indicate that an asynchronous 
        /// operation is supported, false otherwise.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful or an OLE error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 SetAsyncMode(
            [MarshalAs(UnmanagedType.Bool)]
            [In] bool fDoOpAsync);

        /// <summary>
        /// Called by a drop target to determine whether the data 
        /// object supports asynchronous data extraction.
        /// </summary>
        /// <param name="pfIsOpAsync">
        /// True to indicate that an asynchronous 
        /// operation is supported, false otherwise.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful or an OLE error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 GetAsyncMode(
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool pfIsOpAsync);

        /// <summary>
        /// Called by a drop target to indicate that 
        /// asynchronous data extraction is starting.
        /// </summary>
        /// <param name="pbcReserved">
        /// Reserved. Set this value to NULL.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful or an OLE error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 StartOperation(
            [In] IntPtr pbcReserved);

        /// <summary>
        /// Called by the drop source to determine whether 
        /// the target is extracting data asynchronously.
        /// </summary>
        /// <param name="pfInAsyncOp">
        /// True if data extraction is being handled 
        /// asynchronously, or false otherwise.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful or an OLE error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 InOperation(
            [MarshalAs(UnmanagedType.Bool)]
            [Out] out bool pfInAsyncOp);

        /// <summary>
        /// Notifies the data object that that asynchronous data extraction has ended.
        /// </summary>
        /// <param name="hResult">
        /// An HRESULT value that indicates the outcome of the data extraction.
        /// </param>
        /// <param name="pbcReserved">
        /// Reserved. Set to NULL.
        /// </param>
        /// <param name="dwEffects">
        /// Indicates the result of an optimized move. 
        /// This should be the same value that would be passed to the data 
        /// object as a CFSTR_PERFORMEDDROPEFFECT format with a normal 
        /// data extraction operation.
        /// </param>
        /// <returns>
        /// Returns S_OK if successful or an OLE error value otherwise.
        /// </returns>
        [PreserveSig()]
        Int32 EndOperation(
            [In] Int32 hResult,
            [In] IntPtr pbcReserved,
            [In] ShellAPI.DROPEFFECT dwEffects);
    }
}
