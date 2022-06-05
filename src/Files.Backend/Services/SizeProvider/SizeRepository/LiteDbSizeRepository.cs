using LiteDB;

namespace Files.Backend.Services.SizeProvider
{
    internal class LiteDbSizeRepository : ISizeRepository
    {
        private readonly LiteCollection<SizedFolder> collection;

        public LiteDbSizeRepository(LiteDatabase database)
        {
            collection = database.GetCollection<SizedFolder>("sizedFolders");
            collection.EnsureIndex("Path", "$.Path");
        }

        public bool TryGetSize(string path, out ulong size)
        {
            var folder = collection.FindOne(x => x.Path == path);
            size = folder?.Size ?? 0;
            return folder is not null;
        }
        public void SetSize(string path, ulong size)
        {
            var folder = collection.FindOne(x => x.Path == path);
            if (folder is null)
            {
                folder = new SizedFolder { Path = path, Size = size };
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
        }
        public void Delete(string path)
        {
            var folder = collection.FindOne(x => x.Path == path);
            if (folder is not null)
            {
                collection.Delete(folder.Id);
            }
        }

        public class SizedFolder
        {
            [BsonId] public int Id { get; set; }
            public string Path { get; set; } = string.Empty;
            public ulong Size { get; set; }
        }
    }
}
