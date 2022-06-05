using System.Collections.Concurrent;

namespace Files.Backend.Services.SizeProvider
{
    internal class DictionarySizeRepository : ISizeRepository
    {
        private readonly ConcurrentDictionary<string, ulong> cache = new();

        public bool TryGetSize(string path, out ulong size) => cache.TryGetValue(path, out size);
        public void SetSize(string path, ulong size) => cache.[path] = size;

        public void Clear() => cache.Clear();
        public void Delete(string path) => cache.TryRemove(path, out ulong _);
    }
}
