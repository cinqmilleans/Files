using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal interface IFolderRepository : IDisposable
    {
        Task<IFolder?> GetFolder(string path, CancellationToken cancellationToken = default);
        IAsyncEnumerable<IFolder> GetFolders(string rootPath, ushort levelCount = 0, CancellationToken cancellationToken = default);

        Task PutFolder(IFolder folder, CancellationToken cancellationToken = default);
        Task PutFolders(IEnumerable<IFolder> folders, CancellationToken cancellationToken = default);

        Task DeleteFolder(string path, CancellationToken cancellationToken = default);
        Task DeleteFolders(string rootPath, CancellationToken cancellationToken = default);
    }
}
