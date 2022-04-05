using System.Collections.Immutable;

namespace Files.Backend
{
    public static class FileNameHelpers
    {
        private static readonly ImmutableArray<char> RestrictedCharacters = new()
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '|'
        };

        private static readonly ImmutableArray<string> RestrictedFileNames = new()
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };
    }
}
