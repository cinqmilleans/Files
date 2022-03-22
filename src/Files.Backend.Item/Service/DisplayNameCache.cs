using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Item
{
    internal class DisplayNameCache : IDisplayNameCache
    {
        private readonly ConcurrentDictionary<string, string> cache = new();

        public static DisplayNameCache Instance { get; } = new();

        private DisplayNameCache() {}

        public string? ReadDisplayName(string path)
            => cache.TryGetValue(path, out var displayName) ? displayName : null;

        public void SaveDisplayName(string path, string? displayName)
        {
            if (displayName is null)
            {
                cache.TryRemove(path, out _);
            }
            else
            {
                cache[path] = displayName;
            }
        }
    }
}
