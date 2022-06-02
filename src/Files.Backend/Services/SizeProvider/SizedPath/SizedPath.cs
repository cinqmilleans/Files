namespace Files.Backend.Services.SizeProvider
{
    public class Folder
    {
        public string Path { get; init; } = string.Empty;
        public int Level { get; init; }
        public ulong LocalSize { get; init; }
        public ulong GlobalSize { get; init; }
    }
}
