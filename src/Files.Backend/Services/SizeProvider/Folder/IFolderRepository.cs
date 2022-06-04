﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal interface IFolderRepository : IDisposable
    {
        Task<ulong?> GetSizeAsync(string path, CancellationToken cancellationToken = default);
        Task SetSizeAsync(string path, ulong size, CancellationToken cancellationToken = default);

        Task ClearAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
    }
}
