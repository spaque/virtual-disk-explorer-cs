using System;
using System.Runtime.InteropServices;

namespace FileExplorer.Shell
{
    /// <summary>
    /// Provides all the functinality needed to work with PIDLs
    /// </summary>
    public class PIDL
    {
        private IntPtr pidl = IntPtr.Zero;

        /// <summary>
        /// PIDL constructor.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to the ITEMIDLIST.
        /// </param>
        /// <param name="clone">
        /// Tells whether the given pidl is to be cloned.
        /// </param>
        public PIDL(IntPtr pidl, bool clone)
        {
            if (clone)
                this.pidl = ILClone(pidl);
            else
                this.pidl = pidl;
        }

        public IntPtr Ptr { get { return pidl; } }

        /// <summary>
        /// Appends an ITEMIDLIST to the end of the current PIDL.
        /// </summary>
        /// <param name="aPidl">
        /// PIDL to be appended.
        /// </param>
        public void Append(IntPtr aPidl)
        {
            IntPtr newPIDL = ILCombine(pidl, aPidl);
            Marshal.FreeCoTaskMem(pidl);
            pidl = newPIDL;
        }

        /// <summary>
        /// Inserts an ITEMIDLIST into the beginning of the current PIDL.
        /// </summary>
        /// <param name="iPidl">
        /// PIDL to be inserted.
        /// </param>
        public void Insert(IntPtr iPidl)
        {
            IntPtr newPIDL = ILCombine(iPidl, pidl);
            Marshal.FreeCoTaskMem(pidl);
            pidl = newPIDL;
        }

        /// <summary>
        /// Frees the memory previously allocated by PIDL.
        /// </summary>
        public void Free()
        {
            if (pidl != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pidl);
                pidl = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">
        /// The Object to compare with the current Object.
        /// </param>
        /// <returns>
        /// true if the specified Object is equal to the current Object; 
        /// otherwise, false.
        /// </returns>
        public override bool Equals ( object obj )
        {
            try {
                if (obj is IntPtr)
                    return ShellAPI.ILIsEqual(this.Ptr, (IntPtr)obj);
                if (obj is PIDL)
                    return ShellAPI.ILIsEqual(this.Ptr, ((PIDL)obj).Ptr);
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return pidl.GetHashCode();
        }

        /// <summary>
        /// Converts two consecutive bytes into an ushort
        /// </summary>
        /// <param name="b">
        /// Byte array to be converted. Must have length 2.
        /// </param>
        /// <returns>
        /// Returns an ushort with the corresponding value.
        /// </returns>
        public static ushort BtoUS(byte[] b)
        {
            if (b.Length != 2)
                throw new Exception("BtoUS: No se puede hacer la conversion");
            ushort res = b[0] + b[1] * 256;
            return res;
        }

        /// <summary>
        /// Determines whether the specified PIDL is empty.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid identifier list.
        /// </param>
        /// <returns>
        /// Returns true if the list is empty of false otherwise.
        /// </returns>
        public static bool IsEmpty(IntPtr pidl)
        {
            if (pidl == IntPtr.Zero)
                return true;
            byte[] cb = new byte[2];
            Marshal.Copy(pidl, cb, 0, 2);

            return (BtoUS(cb) <= 2);
        }

        /// <summary>
        /// Clones an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to the identifier list to be cloned.
        /// </param>
        /// <returns>
        /// Cloned PIDL.
        /// </returns>
        public static IntPtr ILClone(IntPtr pidl)
        {
            int size = ItemIDListSize(pidl);

            byte[] iidl = new byte[size + 2];
            // get contents of the id list
            Marshal.Copy(pidl, iidl, 0, size + 2);

            // reserve "size" bytes + 2 null terminating bytes
            IntPtr newPIDL = Marshal.AllocCoTaskMem(size + 2);
            Marshal.Copy(iidl, 0, newPIDL, size + 2);

            return newPIDL;
        }

        /// <summary>
        /// Returns the size of the first SHITEMID structure in a PIDL.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid idetifier list.
        /// </param>
        /// <returns>
        /// The size of the first entry if the list has items, or zero otherwise.
        /// </returns>
        public static ushort ItemIDSize(IntPtr pidl)
        {
            if (pidl = IntPtr.Zero)
                return 0;
            byte[] cb = new byte[2];
            // cb field takes 2 bytes
            Marshal.Copy(pidl, cb, 0, 2);
            return BtoUS(cb);
        }

        /// <summary>
        /// Calculates the size of an identifier list.
        /// </summary>
        /// <param name="pidl">
        /// Valid pointer to a PIDL.
        /// </param>
        /// <returns>
        /// Returns the size of the list.
        /// </returns>
        public static ushort ItemIDListSize (IntPtr pidl)
        {
            if (pidl = IntPtr.Zero)
                return 0;

            ushort size, nextSize;
            byte[] b = new byte[2];

            size = ItemIDSize(pidl);
            Marshal.Copy(pidl, b, size, 2);

            nextSize =  BtoUS(b);
            while (nextSize > 0)
            {
                size += nextSize;
                Marshal.Copy(pidl, b, size, 2);
                nextSize = BtoUS(b);
            }
            return size;
        }

        /// <summary>
        /// Combines two ITEMIDLIST structures.
        /// </summary>
        /// <param name="pidl1">
        /// First PIDL.
        /// </param>
        /// <param name="pidl2">
        /// Second PIDL.
        /// </param>
        /// <returns>
        /// Pointer to the new combined ITEMIDLIST structure.
        /// </returns>
        public static IntPtr ILCombine(IntPtr pidl1, IntPtr pidl2)
        {
            ushort size1 = ItemIDListSize(pidl1);
            ushort size2 = ItemIDListSize(pidl2);

            // alloc memory for both PIDLs and the terminating two-byte NULL
            IntPtr newPIDL = Marshal.AllocCoTaskMem(size1 + size2 + 2);
            byte[] b = new byte[size1 + size2 + 2];
            Marshal.Copy(pidl1, b, 0, size1);
            Marshal.Copy(pidl2, b, size1, size2 + 2);

            Marshal.Copy(b, 0, newPIDL, size1 + size2 + 2);

            return newPIDL;
        }

        /// <summary>
        /// Gets the next SHITEMID structure in an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid identifier list.
        /// </param>
        /// <returns>
        /// Returns the SHITEMID that follows the head of the identifier list.
        /// </returns>
        public static IntPtr ILGetNext(IntPtr pidl)
        {
            int size = ItemIDSize(pidl);
            if (size == 0)
                return IntPtr.Zero;
            return new IntPtr((int)pidl + size);
        }

        /// <summary>
        /// Retrieves the last SHITEMID structure from an identifier list.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid identifier list.
        /// </param>
        /// <returns>
        /// Returns a pointer to the last SHITEMID structure.
        /// </returns>
        public static IntPtr ILFindLast(IntPtr pidl)
        {
            IntPtr curPidl = pidl;
            IntPtr nextPidl = ILGetNext(curPidl);

            while (ItemIDSize(nextPidl) > 0)
            {
                curPidl = nextPidl;
                nextPidl = ILGetNext(nextPidl);
            }
            return curPidl;
        }

        /// <summary>
        /// Removes the last SHITEMID structure of an identifier list.
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid identifier list.
        /// </param>
        /// <returns>
        /// Returns false if the last item could not be removed, or true otherwise.
        /// </returns>
        public static bool ILRemoveLast(IntPtr pidl)
        {
            IntPtr lastPidl = ILFindLast(pidl);

            if (lastPidl == pidl)
                return false;

            int newSize = (int)lastPidl - (int)pidl + 2;
            Marshal.ReAllocCoTaskMem(pidl, newSize);

            byte[] z = new byte[] { 0, 0 };
            Marshal.Copy(z, 0, lastPidl, 2);

            return true;
        }

        /// <summary>
        /// Splits an identifier list in two parts:
        /// parent: upper level
        /// child: last level
        /// e.g. c:\MyDocs\MyFiles\MyFile.htm
        ///         parent: c:\MyDocs\MyFiles
        ///         child: MyFile.htm
        /// </summary>
        /// <param name="pidl">
        /// Pointer to a valid identifier list.
        /// </param>
        /// <param name="parent">
        /// PIDL with the upper level.
        /// </param>
        /// <param name="child">
        /// PIDL with the last level.
        /// </param>
        /// <returns>
        /// Returns true if the PIDL could be splitted, of false otherwise.
        /// </returns>
        public static bool ILSplit
            (IntPtr pidl, out IntPtr parent, out IntPtr child)
        {
            IntPtr lastPidl = ILFindLast(pidl);

            if (lastPidl != pidl)
            {
                parent = ILClone(pidl);
                child = ILClone(lastPidl);
                ILRemoveLast(parent);
                return true;
            }
            else
            {
                parent = IntPtr.Zero;
                child = IntPtr.Zero;
                return false;
            }
        }
    }
}
