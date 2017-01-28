using System;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell.Interfaces
{
    /// <summary>
    /// The IMalloc interface allocates, frees, and manages memory.
    /// </summary>
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000002-0000-0000-C000-000000000046")]
	public interface IMalloc 
	{
        /// <summary>
        /// Allocates a block of memory.
        /// </summary>
        /// <param name="cb">
        /// Size, in bytes, of the memory block to be allocated.
        /// </param>
        /// <returns>
        /// If successful, Alloc returns a pointer to the allocated memory block.
        /// If insufficient memory is available, Alloc returns NULL.
        /// </returns>
		[PreserveSig]
		IntPtr Alloc(
			UInt32 cb);
			
        /// <summary>
        /// Changes the size of a previously allocated memory block.
        /// </summary>
        /// <param name="pv">
        /// Pointer to the memory block to be reallocated. 
        /// If set to NULL, a new memory block is allocated.
        /// </param>
        /// <param name="cb">
        /// Size of the memory block (in bytes) to be reallocated.
        /// If pv is not NULL and cb is zero, the memory pointed to by pv is freed.
        /// </param>
        /// <returns>
        /// If successful, returns reallocated memory block.
        /// If insufficient memory, or cb is zero and pv is not NULL, returns NULL.
        /// </returns>
		[PreserveSig]
		IntPtr	Realloc(
			IntPtr pv,
			UInt32 cb);

        /// <summary>
        /// Frees a previously allocated block of memory.
        /// </summary>
        /// <param name="pv">
        /// Pointer to the memory block to be freed.
        /// </param>
		[PreserveSig]
		void Free(
			IntPtr pv);

        /// <summary>
        /// Returns the size (in bytes) of a memory block previously 
        /// allocated with IMalloc::Alloc or IMalloc::Realloc.
        /// </summary>
        /// <param name="pv">
        /// Pointer to the memory block for which the size is requested.
        /// </param>
        /// <returns>
        /// The size of the allocated memory block in bytes or, 
        /// if pv is a NULL pointer, -1.
        /// </returns>
		[PreserveSig]
		UInt32 GetSize(
			IntPtr pv);

        /// <summary>
        /// Determines whether this allocator was used to 
        /// allocate the specified block of memory.
        /// </summary>
        /// <param name="pv">
        /// Pointer to the memory block.
        /// Can be a NULL pointer, in which case, -1 is returned.
        /// </param>
        /// <returns>
        ///  1 - The memory block was allocated by this IMalloc instance.
        ///  0 - The memory block was not allocated by this IMalloc instance.
        /// -1 - DidAlloc is unable to determine whether or not it allocated 
        ///      the memory block.
        /// </returns>
		[PreserveSig]
		Int16 DidAlloc(
			IntPtr pv);

        /// <summary>
        /// Minimizes the heap as much as possible by releasing unused 
        /// memory to the operating system, coalescing adjacent free 
        /// blocks and committing free pages.
        /// </summary>
		[PreserveSig]
		void HeapMinimize();
	}
}
	