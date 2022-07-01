using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Files.Backend.Filesystem.Storage
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TOKEN_USER
    {
        public SID_AND_ATTRIBUTES User;
    }
}
