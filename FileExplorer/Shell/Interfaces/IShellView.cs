using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FileExplorer.Shell.Interfaces
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E3-0000-0000-C000-000000000046")]
    internal interface IShellView
    {
        /// <summary>
        /// Get the handle of the shellview implemented
        /// </summary>
        void GetWindow(out IntPtr windowHandle);

        /// <summary>
        /// ContextSensitiveHelp
        /// </summary>
        void ContextSensitiveHelp(bool fEnterMode);

        /// <summary>
        /// Translates accelerator key strokes when a namespace 
        /// extension's view has the focus
        /// </summary>
        [PreserveSig]
        long TranslateAcceleratorA(IntPtr message);

        /// <summary>
        /// Enables or disables modeless dialog boxes. This method is 
        /// not currently implemented
        /// </summary>
        void EnableModeless(bool enable);

        /// <summary>
        /// Called when the activation state of the view window is changed 
        /// by an event that is not caused by the Shell view itself. 
        /// For example, if the TAB key is pressed when the tree has the 
        /// focus, the view should be given the focus
        /// </summary>
        void UIActivate(
            [MarshalAs(UnmanagedType.U4)] 
            ActivationStates activtionState);

        /// <summary>
        /// Refreshes the view's contents in response to user input
        /// Explorer calls this method when the F5 key is pressed on 
        /// an already open view
        /// </summary>
        void Refresh();

        /// <summary>
        /// Creates a view window. This can be either the right pane of  
        /// Explorer or the client window of a folder window.
        /// </summary>
        void CreateViewWindow(
            [In, MarshalAs(UnmanagedType.Interface)] 
            IShellView previousShellView, 
            [In] ref ShellAPI.FOLDERSETTINGS folderSetting, 
            [In] IShellBrowser shellBrowser, 
            [In] ref ShellAPI.RECT bounds, 
            [In, Out] ref IntPtr handleOfCreatedWindow);

        /// <summary>
        /// Destroys the view window
        /// </summary>
        void DestroyViewWindow();

        /// <summary>
        /// Retrieves the current folder settings
        /// </summary>
        void GetCurrentInfo(ref ShellAPI.FOLDERSETTINGS pfs);

        /// <summary>
        /// Allows the view to add pages to the Options property 
        /// sheet from the View menu
        /// </summary>
        void AddPropertySheetPages(
            [In, MarshalAs(UnmanagedType.U4)] 
            uint reserved, 
            [In] ref IntPtr functionPointer, 
            [In] IntPtr lparam);

        /// <summary>
        /// Saves the Shell's view settings so the current state can be 
        /// restored during a subsequent browsing session
        /// </summary>
        void SaveViewState();

        /// <summary>
        /// Changes the selection state of one or more items 
        /// within the Shell view window
        /// </summary>
        void SelectItem(
            IntPtr pidlItem, 
            [MarshalAs(UnmanagedType.U4)] 
            uint flags);

        /// <summary>
        /// Retrieves an interface that refers to data presented in the view
        /// </summary>
        [PreserveSig]
        long GetItemObject(
            [MarshalAs(UnmanagedType.U4)] 
            ShellAPI.SVGIO AspectOfView, 
            ref Guid riid, 
            ref IntPtr ppv);
    }
}
