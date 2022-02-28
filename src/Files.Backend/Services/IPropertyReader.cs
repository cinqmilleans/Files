using Files.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;

namespace Files.Backend.Services
{
    internal interface IPropertyReader
    {
        Task<T> GetPropertyAsync<T>(string key);
        Task<IDictionary<string, T>> GetPropertiesAsync<T>(params string[] keys);
    }

    internal class PropertyReader : IPropertyReader
    {
        private readonly IStorageItemExtraProperties properties;

        private TimeSpan timeout = TimeSpan.FromSeconds(5);
        public TimeSpan Timeout
        {
            get => timeout;
            init => timeout = value > TimeSpan.Zero ? value : TimeSpan.Zero;
        }

        public PropertyReader(IStorageItemExtraProperties properties) => this.properties = properties;

        public async Task<T> GetPropertyAsync<T>(string key)
        {
            var properties = await GetPropertiesAsync<T>(key);
            return properties[key];
        }
        public async Task<IDictionary<string, T>> GetPropertiesAsync<T>(params string[] keys)
        {
            //await Task.Yield();
            //return new Dictionary<string, T>();
            //var results = await properties.RetrievePropertiesAsync(keys).AsTask().WithTimeoutAsync(Timeout);
            //return default; // keys.ToDictionary(key => key, key => results?[key] is T ? (T)results[key] : default);
            return new Dictionary<string, T>();
        }
    }
}
