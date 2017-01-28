using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualDrive.Interfaces
{
    public interface ISystem
    {
        void Format();

        String Identify();

        void Free(out uint used, out uint free);

        void Mount();

        void Dismount();
    }
}
