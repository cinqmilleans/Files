using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider.Folder
{
    internal class NoFolderRepository : IFolderRepository
    {
        public Task<IFolder?> GetFolderAsync(string path, CancellationToken cancellationToken = default)
            => Task.FromResult<IFolder?>(null);
        public IAsyncEnumerable<IFolder> GetFoldersAsync(string rootPath, ushort levelCount = 0, CancellationToken cancellationToken = default)
            => AsyncEnumerable.Empty<IFolder>();

        public Task PutFolderAsync(IFolder folder, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PutFoldersAsync(IEnumerable<IFolder> folders, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task CleanAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteFolderAsync(string path, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteFoldersAsync(string rootPath, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Dispose() {}
    }
}
