namespace Files.Backend.Item
{
    public interface IShortcutItem : IFileItem
    {
        IShortcut Shortcut { get; }
    }

    internal class ShortcutItem : FileItem, IShortcutItem
    {
        public IShortcut Shortcut { get; init; } = new Shortcut();
    }
}
