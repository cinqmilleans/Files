using Files.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.BackEnd
{
    internal interface IStorageFolderReader
    {
        Task<T> GetPropertyAsync<T>(string key);
        Task<IDictionary<string, T>> GetPropertiesAsync<T>(params string[] keys);
    }

    internal class StorageFolderReader : IStorageFolderReader
    {
        private readonly StorageFolder folder;

        private TimeSpan timeout = TimeSpan.FromSeconds(5);
        public TimeSpan Timeout
        {
            get => timeout;
            init => timeout = value > TimeSpan.Zero ? value : TimeSpan.Zero;
        }

        public StorageFolderReader(StorageFolder folder) => this.folder = folder;

        public async Task<T> GetPropertyAsync<T>(string key)
        {
            var properties = await GetPropertiesAsync<T>(key);
            return properties[key];
        }
        public async Task<IDictionary<string, T>> GetPropertiesAsync<T>(params string[] keys)
        {
            var properties = await folder.Properties.RetrievePropertiesAsync(keys).AsTask().WithTimeoutAsync(Timeout);
            return keys.ToDictionary(key => key, key => properties?[key] is T ? (T)properties[key] : default);
        }
    }
}
