using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Files.Shared.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IList<T>> ToList<T>(this IAsyncEnumerable<T> source)
        {
            List<T> items = new();

            await foreach (var item in source)
            {
                items.Add(item);
            }

            return items;
        }
        public static async Task<IList<T>> ToList<T>(this IAsyncEnumerable<T> source, int maxCount)
        {
            List<T> items = new();

            if (maxCount <= 0)
            {
                return items;
            }

            await foreach (var item in source)
            {
                items.Add(item);

                if (--maxCount is 0)
                {
                    break;
                }
            }

            return items;
        }

        public static async Task<IList<T>> ToList<T>(this ConfiguredCancelableAsyncEnumerable<T> source)
        {
            List<T> items = new();

            await foreach (var item in source)
            {
                items.Add(item);
            }

            return items;
        }
        public static async Task<IList<T>> ToList<T>(this ConfiguredCancelableAsyncEnumerable<T> source, int maxCount)
        {
            List<T> items = new();

            if (maxCount <= 0)
            {
                return items;
            }

            await foreach (var item in source)
            {
                items.Add(item);

                if (--maxCount is 0)
                {
                    break;
                }
            }

            return items;
        }
    }
}
