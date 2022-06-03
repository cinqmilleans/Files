using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    public class CachedSizeProvider : ISizeProvider
    {
        private const int CacheLevel = 3;
        private const int EventLevel = 2;

        private readonly FolderEnumerator enumerator = new();

        private readonly ConcurrentDictionary<string, ulong> sizes = new();

        public event EventHandler<SizeChangedEventArgs>? SizeChanged;

        public Task CleanAsync()
        {
            sizes.Clear();
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(string path, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (TryGetSize(path, out ulong cachedSize))
            {
                RaiseSizeChanged(path, cachedSize, SizeChangedValueState.Intermediate);
            }
            else
            {
                RaiseSizeChanged(path, 0, SizeChangedValueState.None);
            }

            ulong size = 0;

            var folders = enumerator.EnumerateFolders(path).WithCancellation(cancellationToken);
            await foreach (var folder in folders)
            {
                if (folder.Level <= CacheLevel)
                {
                    await Task.Yield();
                    sizes[folder.Path] = folder.GlobalSize;
                }

                if (folder.Level is 0)
                {
                    RaiseSizeChanged(path, size, SizeChangedValueState.Final);
                }
                else if (folder.Level <= EventLevel)
                {
                    size += folder.Level is EventLevel ? folder.GlobalSize : folder.LocalSize;
                    RaiseSizeChanged(path, size, SizeChangedValueState.Intermediate);
                }
            }
        }

        public bool TryGetSize(string path, out ulong size) => sizes.TryGetValue(path, out size);

        public void Dispose() {}

        private void RaiseSizeChanged(string path, ulong newSize, SizeChangedValueState valueState)
            => SizeChanged?.Invoke(this, new SizeChangedEventArgs(path, newSize, valueState));
    }
}
