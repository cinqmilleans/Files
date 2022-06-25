using Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public interface IShortcutStorageItem : IStorageItem
    {
        string TargetPath { get; }
        string Arguments { get; }
        string WorkingDirectory { get; }
        bool RunAsAdmin { get; }
    }
}
