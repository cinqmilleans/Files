using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class PersistentSizeProvider : ISizeProvider
    {
        private const int CacheLevel = 3;
        private const int EventLevel = 2;

        private readonly ISizeRepository repository;
        private readonly IFolderEnumerator enumerator;

        public event EventHandler<SizeChangedEventArgs>? SizeChanged;

        public PersistentSizeProvider(ISizeRepository repository, IFolderEnumerator enumerator)
            => (this.repository, this.enumerator) = (repository, enumerator);

        public Task CleanAsync()
        {
            repository.Clear();
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
                    repository.SetSize(folder.Path, folder.GlobalSize);
                }

                if (folder.Level is 0)
                {
                    RaiseSizeChanged(path, folder.GlobalSize, SizeChangedValueState.Final);
                }
                else if (folder.Level <= EventLevel)
                {
                    size += folder.Level is EventLevel ? folder.GlobalSize : folder.LocalSize;
                    RaiseSizeChanged(path, size, SizeChangedValueState.Intermediate);
                }
            }
        }

        public bool TryGetSize(string path, out ulong size) => repository.TryGetSize(path, out size);

        public void Dispose() {}

        private void RaiseSizeChanged(string path, ulong newSize, SizeChangedValueState valueState)
            => SizeChanged?.Invoke(this, new SizeChangedEventArgs(path, newSize, valueState));
    }
}
