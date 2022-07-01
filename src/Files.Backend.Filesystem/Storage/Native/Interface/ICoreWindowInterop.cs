using System;
using System.Runtime.InteropServices;

namespace Files.Backend.Filesystem.Storage
{
    // https://www.travelneil.com/wndproc-in-uwp.html
    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { get; }
    }
}
