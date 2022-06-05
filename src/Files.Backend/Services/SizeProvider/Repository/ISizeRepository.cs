using System;

namespace Files.Backend.Services.SizeProvider
{
    internal interface ISizeRepository
    {
        bool TryGetSize(string path, out ulong size);
        void SetSize(string path, ulong size);

        void Clear();
        void Delete(string path);
    }
}
