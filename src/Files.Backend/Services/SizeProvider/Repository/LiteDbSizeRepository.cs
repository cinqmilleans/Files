using LiteDB;

namespace Files.Backend.Services.SizeProvider
{
    internal class LiteDbSizeRepository : ISizeRepository
    {
        private readonly LiteCollection<SizedFolder> collection;

        public LiteDbSizeRepository(LiteCollection<SizedFolder> collection)
            => this.collection = collection;

        public bool TryGetSize(string path, out ulong size)
        {
            var folder = collection.FindOne(f => f.Path == path);
            size = folder?.Size ?? 0;
            return folder is not null;
        }
        public void SetSize(string path, ulong size)
        {
            var folder = collection.FindOne(f => f.Path == path);
            if (folder is null)
            {
                folder = new SizedFolder(path, size);
                collection.Insert(folder);
            }
            else
            {
                folder.Size = size;
                collection.Update(folder);
            }
        }

        public void Clear()
        {
            collection.Delete(f => true);
        }
        public void Delete(string path)
        {
            var folder = collection.FindOne(f => f.Path == path);
            if (folder is not null)
            {
                collection.Delete(folder.Id);
            }
        }
    }
}
