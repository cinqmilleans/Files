using System;
using System.Runtime.InteropServices;

namespace Files.Backend.Filesystem.Storage
{
    public unsafe struct OVERLAPPED
    {
        public IntPtr Internal;
        public IntPtr InternalHigh;
        public Union PointerAndOffset;
        public IntPtr hEvent;

        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            [FieldOffset(0)] public void* IntPtr;
            [FieldOffset(0)] public OffsetPair Offset;

            public struct OffsetPair
            {
                public uint Offset;
                public uint OffsetHigh;
            }
        }
    }
}
