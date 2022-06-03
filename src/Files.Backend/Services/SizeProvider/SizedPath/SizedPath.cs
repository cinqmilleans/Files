namespace Files.Backend.Services.SizeProvider
{
        public class Folder
    {
        public string Path { get; init; } = string.Empty;
        public uint Level { get; init; }
        public ulong LocalSize { get; init; }
        public ulong GlobalSize { get; init; }

        public Folder() {}
        public Folder(string path)
            => Path = path;
        public Folder(string path, uint level)
            => (Path, Level) = (path, level);
            public Folder(string path, uint level, ulong size)
            => (Path, Level, LocalSize, GlobalSize) = (path, level, size, size);
        public Folder(string path, uint level, ulong localSize, ulong globalSize)
            => (Path, Level, LocalSize, GlobalSize) = (path, level, localSize, globalSize);

        //private static int GetLevel(string path) => path.Count(c => c is '\\' or '/');
    }
}
