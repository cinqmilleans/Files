﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    public class DrivesSizeProvider : ISizeProvider
    {
        private readonly IDictionary<string, ISizeProvider> providers = new Dictionary<string, ISizeProvider>();

        public event EventHandler<SizeChangedEventArgs>? SizeChanged;

        public async Task CleanAsync()
        {
            foreach (var provider in providers.Values)
            {
                await provider.CleanAsync();
            }
            providers.Clear();
        }

        public async Task UpdateAsync(string path, CancellationToken cancellationToken)
        {
            string driveName = GetDriveName(path);
            if (!providers.ContainsKey(driveName))
            {
                await CreateProviderAsync(driveName);
            }
            var provider = providers[driveName];
            await provider.UpdateAsync(path, cancellationToken);
        }

        public bool TryGetSize(string path, out ulong size)
        {
            string driveName = GetDriveName(path);
            if (!providers.ContainsKey(driveName))
            {
                size = 0;
                return false;
            }
            var provider = providers[driveName];
            return provider.TryGetSize(path, out size);
        }

        private static string GetDriveName(string path) => Directory.GetDirectoryRoot(path);

        private async Task CreateProviderAsync(string driveName)
        {
            var repositoryProvider = new SizeRepositoryProvider();
            var repository = await repositoryProvider.GetSizeRepositoryAsync(driveName);

            var sizeProvider = new PersistentSizeProvider(repository);
            sizeProvider.SizeChanged += Provider_SizeChanged;
            providers.Add(driveName, sizeProvider);
        }

        private void Provider_SizeChanged(object sender, SizeChangedEventArgs e)
            => SizeChanged?.Invoke(this, e);

        public void Dispose()
        {
            foreach (var provider in providers.Values)
            {
                provider.SizeChanged -= Provider_SizeChanged;
                provider.Dispose();
            }
        }
    }
}
