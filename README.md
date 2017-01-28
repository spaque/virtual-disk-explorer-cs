# virtual-disk-explorer-cs
Virtual disk explorer

![Alt text](/screenshot.jpg?raw=true)

The application was originally developed in Visual Studio 2005 using C# and .NET 2.0. The aim was:
 * develop a reusable component that, using a file in the host OS for storage, emulates a virtual disk following the FAT32 file system standard,
 * create a component able to visually explore the contents of both the host and the virtual disk, either as tree or list view, and
 * use both components in a GUI to be able to interact with the virtual disk in a non-blocking manner:
   * drag&drop content between the host and virtual disk,
   * perform actions from any item's context menu,
   * peform file operations on either the host or virtual file system, e.g. open files, copy/cut/paste files,
   * issue commands in a custom command line,
   * monitor performance.
