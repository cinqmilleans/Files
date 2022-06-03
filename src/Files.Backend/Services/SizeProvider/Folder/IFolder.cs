namespace Files.Backend.Services.SizeProvider
{
    internal interface IFolder
    {
        public string Path { get; }
        public ushort Level { get; }

        public ulong LocalSize { get; }
        public ulong GlobalSize { get; }
    }
}
