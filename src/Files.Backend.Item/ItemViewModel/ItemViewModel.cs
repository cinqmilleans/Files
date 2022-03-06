using System.Collections.Generic;

namespace Files.Backend.Item
{
    public interface IItemViewModel
    {
        string Path { get; }
        string Name { get; }
    }

    public interface IItemViewModelFactory
    {
        IItemViewModel BuildItemViewModel(IItem item);
    }

    public class ItemViewModelFactory : IItemViewModelFactory
    {
        public IItemViewModel BuildItemViewModel(IItem item) => null;
    }

    public static class ItemExtension
    {
        private static IItemViewModelFactory factory = new ItemViewModelFactory();

        public static IItemViewModel ToViewModel(this IItem item) => factory.BuildItemViewModel(item);

        public static async IAsyncEnumerable<IItemViewModel> ToViewModel<TItem>(this IAsyncEnumerable<IItem> items)
        {
            await foreach (var item in items)
            {
                yield return factory.BuildItemViewModel(item);
            }
        }
    }
}
