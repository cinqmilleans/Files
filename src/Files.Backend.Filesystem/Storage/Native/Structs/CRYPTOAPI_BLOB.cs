﻿using System;
using System.Runtime.InteropServices;

namespace Files.Backend.Filesystem.Storage
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CRYPTOAPI_BLOB
    {
        public uint cbData;

        public IntPtr pbData;
    }
}
