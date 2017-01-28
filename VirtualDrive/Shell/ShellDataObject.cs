using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

using VirtualDrive.FileSystem.FAT32;
using FileExplorer.Shell;
using FileExplorer.Shell.Interfaces;

namespace VirtualDrive.Shell
{
    public class ShellDataObject : DataObject//, IAsyncOperation
    {
        private bool downloaded;
        private bool clipboardOp;
        private ManualResetEvent mre;

        public ShellDataObject(ManualResetEvent mre, bool clipboardOp)
            : base()
        {
            this.mre = mre;
            this.clipboardOp = clipboardOp;
        }

        public override object GetData(String format)
        {
            Object obj = base.GetData(format);
            if (format == DataFormats.FileDrop &&
                (clipboardOp || !InDragLoop()) && 
                !downloaded)
            {
                mre.WaitOne(40000, false);
                downloaded = true;
            }
            return obj;
        }

        private bool InDragLoop()
        {
            return ((int)GetData(ShellAPI.CFSTR_INDRAGLOOP) != 0);
        }

/*        private void WriteData(String path, String virtualPath)
        {
            using (FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Write))
            {
                int fd = disk.Open(virtualPath, OPENMODE.READ);
                byte[] buffer = new byte[4096];
                int bytes = disk.Read(fd, buffer, 0, 4096);
                while (bytes > 0)
                {
                    fstream.Write(buffer, 0, bytes);
                    bytes = disk.Read(fd, buffer, 0, 4096);
                }
                disk.Close(fd);
            }
        }

        private void AsyncTransfer()
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                    WriteData(files[i], selected[i]);
                else
                {
                    WriteDirectory(files[i]);
                }
            }
            downloaded = true;
            disk.Busy = false;
        }

        private void WriteDirectory(String path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            FileSystemInfo[] infos = info.GetFileSystemInfos();
            String relativePath = path.Substring(tempPath.Length + 1);
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Attributes == FileAttributes.Directory)
                    WriteDirectory(infos[i].FullName);
                else
                    WriteData(infos[i].FullName, "V:\\" + relativePath + "\\" + infos[i].Name);
            }
        }

        #region IAsyncOperation

        public int SetAsyncMode(bool fDoOpAsync)
        {
            asyncExtraction = fDoOpAsync;
            return ShellAPI.S_OK;
        }

        public int GetAsyncMode(out bool pfIsOpAsync)
        {
            pfIsOpAsync = asyncExtraction;
            return ShellAPI.S_OK;
        }

        public int StartOperation(IntPtr pbcReserved)
        {
            inAsyncOp = true;
            disk.Busy = true;
            //Thread.Sleep(500);
            Thread thread = new Thread(new ThreadStart(AsyncTransfer));
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            //AsyncTransfer();
            thread.Join();
            return ShellAPI.S_OK;
        }

        public int InOperation(out bool pfInAsyncOp)
        {
            pfInAsyncOp = inAsyncOp;
            return ShellAPI.S_OK;
        }

        public int EndOperation(int hResult, IntPtr pbcReserved, ShellAPI.DROPEFFECT dwEffects)
        {
            inAsyncOp = false;
            return ShellAPI.S_OK;
        }

        #endregion*/
    }
}
