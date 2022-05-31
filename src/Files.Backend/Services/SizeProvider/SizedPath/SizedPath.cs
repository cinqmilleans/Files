namespace Files.Backend.Services.SizeProvider
{
    internal class SizedPath
    {
        public string Path { get; }
        public int Level { get; }
        public ulong LocalSize { get; }
        public ulong GlobalSize { get; }

        public SizedPath(string path, int level = 0, ulong localSize = 0, ulong globalSize = 0)
            => (Path, Level, LocalSize, GlobalSize) = (path, level, localSize, globalSize);
    }
}
