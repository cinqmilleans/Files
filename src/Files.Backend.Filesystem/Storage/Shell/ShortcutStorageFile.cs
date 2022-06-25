using Files.Shared;

namespace Files.Backend.Filesystem.Storage
{
    public class ShortcutStorageFile : ShellStorageFile, IShortcutStorageItem
    {
        public string TargetPath { get; }
        public string Arguments { get; }
        public string WorkingDirectory { get; }
        public bool RunAsAdmin { get; }

        public ShortcutStorageFile(ShellLinkItem item) : base(item)
        {
            TargetPath = item.TargetPath;
            Arguments = item.Arguments;
            WorkingDirectory = item.WorkingDirectory;
            RunAsAdmin = item.RunAsAdmin;
        }
    }
}
