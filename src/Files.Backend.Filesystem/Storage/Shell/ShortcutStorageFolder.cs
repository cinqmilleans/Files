using Files.Shared;

namespace Files.Backend.Filesystem.Storage
{
    public class ShortcutStorageFolder : ShellStorageFolder, IShortcutStorageItem
    {
        public string TargetPath { get; }
        public string Arguments { get; }
        public string WorkingDirectory { get; }
        public bool RunAsAdmin { get; }

        public ShortcutStorageFolder(ShellLinkItem item) : base(item)
        {
            TargetPath = item.TargetPath;
            Arguments = item.Arguments;
            WorkingDirectory = item.WorkingDirectory;
            RunAsAdmin = item.RunAsAdmin;
        }
    }
}
