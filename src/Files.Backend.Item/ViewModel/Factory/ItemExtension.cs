using System.Collections.Generic;
using System.Linq;

namespace Files.Backend.Item
{
    public static class ItemExtension
    {
        private static readonly IItemViewModelFactory factory = new ItemViewModelFactory();

        public static IItemViewModel ToViewModel(this IItem item) => factory.BuildItemViewModel(item);

        public static IEnumerable<IItemViewModel> ToViewModel<TItem>(this IEnumerable<IItem> items)
            => items.Select(item => factory.BuildItemViewModel(item));

        public static async IAsyncEnumerable<IItemViewModel> ToViewModel<TItem>(this IAsyncEnumerable<IItem> items)
        {
            await foreach (var item in items)
            {
                yield return factory.BuildItemViewModel(item);
            }
        }
    }
}
