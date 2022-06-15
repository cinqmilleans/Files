using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Shared.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            var list = new List<T>();
            await foreach (var item in items.WithCancellation(cancellationToken))
            {
                list.Add(item);
            }
            return list;
        }
    }
}
