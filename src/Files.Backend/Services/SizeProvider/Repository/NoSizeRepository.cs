namespace Files.Backend.Services.SizeProvider
{
    internal class NoSizeRepository : ISizeRepository
    {
        public bool TryGetSize(string path, out ulong size)
        {
            size = 0;
            return false;
        }
        public void SetSize(string path, ulong size) {}

        public void Clear() {}
        public void Delete(string path) {}
    }
}
