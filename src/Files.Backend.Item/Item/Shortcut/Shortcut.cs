namespace Files.Backend.Item
{
    public interface IShortcut
    {
        ShortcutTypes ShortcutType { get; }

        string TargetPath { get; }
        string Arguments { get; }
        string WorkingDirectory { get; }
    }

    internal class Shortcut : IShortcut
    {
        public ShortcutTypes ShortcutType { get; init; } = ShortcutTypes.Unknown;

        public string TargetPath { get; init; } = string.Empty;
        public string Arguments { get; init; } = string.Empty;
        public string WorkingDirectory { get; init; } = string.Empty;
    }
}
