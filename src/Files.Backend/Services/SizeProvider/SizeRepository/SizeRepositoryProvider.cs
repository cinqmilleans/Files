using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizeRepositoryProvider : ISizeRepositoryProvider
    {
        public async Task<ISizeRepository> CreateSizeRepository(string driveName, CancellationToken cancellationToken = default)
        {
            var volumeInfoFactory = Ioc.Default.GetService<IVolumeInfoFactory>();
            if (volumeInfoFactory is IVolumeInfoFactory factory)
            {
                var info = await factory.BuildVolumeInfo(driveName);
                if (!info.IsEmpty)
                {
                }

            }
            return new DictionarySizeRepository();
        }

    }
}
