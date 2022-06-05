using Files.Shared.Extensions;
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

        private readonly IFolderRepository repository;
        private readonly IFolderEnumerator enumerator;

        public event EventHandler<SizeChangedEventArgs>? SizeChanged;

        public PersistentSizeProvider(IFolderRepository repository, IFolderEnumerator enumerator)
            => (this.repository, this.enumerator) = (repository, enumerator);

        public Task CleanAsync() => Task.CompletedTask;

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

            var newFolders = enumerator.EnumerateFolders(path).WithCancellation(cancellationToken);

            ulong size = 0;
            var cache = new List<IFolder>();

            await foreach (var newFolder in newFolders)
            {
                if (newFolder.Level <= CacheLevel)
                {
                    await Task.Yield();
                    cache.Add(newFolder);
                }

                if (newFolder.Level is 0)
                {
                    RaiseSizeChanged(path, size, SizeChangedValueState.Final);
                }
                else if (newFolder.Level <= EventLevel)
                {
                    size += newFolder.Level is EventLevel ? newFolder.GlobalSize : newFolder.LocalSize;
                    RaiseSizeChanged(path, size, SizeChangedValueState.Intermediate);
                }
            }
        }

        public bool TryGetSize(string path, out ulong size)
        {
            throw new NotImplementedException();
        }

        public void Dispose() {}

        private void RaiseSizeChanged(string path, ulong newSize, SizeChangedValueState valueState)
            => SizeChanged?.Invoke(this, new SizeChangedEventArgs(path, newSize, valueState));
    }
}
