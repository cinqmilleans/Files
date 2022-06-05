﻿using System.Collections.Generic;
using System.Threading;

namespace Files.Backend.Services.SizeProvider
{
    internal interface IFolderEnumerator
    {
        IAsyncEnumerable<IFolder> EnumerateFolders(string path, CancellationToken cancellationToken = default);
    }
}
