using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class NoSizeRepository : ISizeRepository
    {
        public Task<ulong?> GetSizeAsync(string path, CancellationToken cancellationToken = default) => Task.FromResult<ulong?>(null);
        public Task SetSizeAsync(string path, ulong size, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ClearAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(string path, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Dispose() {}
    }
}
