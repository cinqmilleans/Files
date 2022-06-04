using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class DictionaryFolderRepository : IFolderRepository
    {
        private readonly ConcurrentDictionary<string, ulong> cache = new();

        public Task<ulong?> GetSizeAsync(string path, CancellationToken _ = default)
            => cache.TryGetValue(path, out ulong size)
                ? Task.FromResult<ulong?>(size)
                : Task.FromResult<ulong?>(null);

        public Task SetSizeAsync(string path, ulong size, CancellationToken _ = default)
        {
            cache[path] = size;
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken _ = default)
        {
            cache?.Clear();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            cache.TryRemove(path, out ulong _);
            return Task.CompletedTask;
        }

        public void Dispose() {}
    }
}
