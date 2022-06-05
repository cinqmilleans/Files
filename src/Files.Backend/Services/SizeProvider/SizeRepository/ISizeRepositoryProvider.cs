﻿using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal interface ISizeRepositoryProvider
    {
        Task<ISizeRepository> GetSizeRepository(string driveName, CancellationToken cancellationToken = default);
    }
}
