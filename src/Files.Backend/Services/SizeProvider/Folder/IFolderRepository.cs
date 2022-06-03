using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal interface IFolderRepository : IDisposable
    {
        Task<IFolder?> GetFolderAsync(string path, CancellationToken cancellationToken = default);
        IAsyncEnumerable<IFolder> GetFoldersAsync(string rootPath, ushort levelCount = 0, CancellationToken cancellationToken = default);

        Task PutFolderAsync(IFolder folder, CancellationToken cancellationToken = default);
        Task PutFoldersAsync(IEnumerable<IFolder> folders, CancellationToken cancellationToken = default);

        Task CleanAsync(CancellationToken cancellationToken = default);

        Task DeleteFolderAsync(string path, CancellationToken cancellationToken = default);
        Task DeleteFoldersAsync(string rootPath, CancellationToken cancellationToken = default);
    }
}
