using LiteDB;
using System.Collections.Generic;

namespace Files.Backend.Services.SizeProvider
{
    internal class LiteDbSizeRepository : ISizeRepository
    {
        private readonly LiteCollection<SizedFolder> collection;

        public LiteDbSizeRepository(LiteCollection<SizedFolder> collection)
            => this.collection = collection;

        public bool TryGetSize(string path, out ulong size)
        {
            var folder = GetFolder(path);
            size = folder?.Size ?? 0;
            return folder is not null;
        }
        public void SetSize(string path, ulong size)
        {
            var a = new SizedFolder("a", 1);
            var b = new SizedFolder("b", 1);
            var c = new SizedFolder("c", 1);

            c.Parent = b;
            b.Parent = a;

            collection.InsertBulk(new List<SizedFolder> { a, b, c});

            //var d = collection.FindOne(Query.EQ("Path", "c"));

            int n1 = collection.Count();
            //var d = collection.FindOne(Query.EQ("Path", "a"));
            int n2 = collection.Count();



            //var folder = GetFolder(path);
            //if (folder is null)
            //{
            //    folder = new SizedFolder(path, size);
            //    collection.Insert(folder);
            //}
            //else
            //{
            //    folder.Size = size;
            //    collection.Update(folder);
            //}
        }

        public void Clear()
        {
            collection.Delete(f => true);
        }
        public void Delete(string path)
        {
            var folder = GetFolder(path);
            if (folder is not null)
            {
                collection.Delete(folder.Id);
            }
        }

        private SizedFolder GetFolder(string path)
            => collection.FindOne(Query.EQ(nameof(SizedFolder.Path), path));
    }
}
