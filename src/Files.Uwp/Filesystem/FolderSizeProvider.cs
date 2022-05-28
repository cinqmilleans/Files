using Files.Backend.Services.SizeProvider;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Uwp.Filesystem
{
    public interface IFolderSizeProvider
    {
        public event EventHandler<FolderSizeChangedEventArgs> FolderSizeChanged;

        Task CleanCacheAsync();
        Task UpdateFolderAsync(string folderPath, CancellationToken cancellationToken);
        bool GetCachedSize(string folderPath, out long cachedSize);
    }

    public class FolderSizeChangedEventArgs : EventArgs
    {
        public string Folder { get; }
        public long Size { get; }
        public bool Intermediate { get; }

        public FolderSizeChangedEventArgs(string folderPath, long newSize, bool intermediate)
            => (Folder, Size, Intermediate) = (folderPath, newSize, intermediate);
    }

    public class FolderSizeProvider : IFolderSizeProvider, IDisposable
    {
        private readonly ISizeProvider provider = new UserSizeProvider();

        public event EventHandler<FolderSizeChangedEventArgs> FolderSizeChanged;

        public FolderSizeProvider() => provider.SizeChanged += Provider_SizeChanged;

        public async Task CleanCacheAsync()
            => await provider.CleanAsync();

        public async Task UpdateFolderAsync(string folderPath, CancellationToken cancellationToken)
            => await provider.UpdateAsync(folderPath, cancellationToken);

        public bool GetCachedSize(string folderPath, out long cachedSize)
        {
            bool cache = provider.TryGetSize(folderPath, out ulong size);
            cachedSize = (long)size;
            return cache;
        }

        private void Provider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var eventArgs = e.ValueState switch
            {
                SizeChangedValueState.None => new FolderSizeChangedEventArgs(e.Path, -1, false),
                SizeChangedValueState.Final => new FolderSizeChangedEventArgs(e.Path, (long)e.NewSize, false),
                SizeChangedValueState.Intermediate => new FolderSizeChangedEventArgs(e.Path, (long)e.NewSize, true),
                _ => throw new ArgumentException(),
            };
            FolderSizeChanged?.Invoke(this, eventArgs);
        }

        public void Dispose() => provider.SizeChanged -= Provider_SizeChanged;
    }
}