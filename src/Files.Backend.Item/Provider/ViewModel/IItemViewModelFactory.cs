namespace Files.Backend.Item
{
    public interface IItemViewModelFactory
    {
        IItemViewModel BuildItemViewModel(IItem item);
    }
}
