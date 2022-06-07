using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.Backend.Services.SizeProvider
{
    public class SizeProviderFactory
    {
        public static ISizeProvider GetSizeProvider()
        {
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "foldersizes.sqlite");
            var database = new SizeDatabase(path, true);

            var collection = database.GetCollection(Guid.Empty);
            var repository = new LiteDbSizeRepository(collection);

            repository.SetSize(path, 100);
            repository.TryGetSize(path, out ulong size);

            return new PersistentSizeProvider(repository);
        }
    }

    internal class SizeRepositoryProvider : ISizeRepositoryProvider
    {
        private SizeDatabase? database;

        public async Task<ISizeRepository> GetSizeRepositoryAsync(string driveName, CancellationToken cancellationToken = default)
        {
            var volumeInfoFactory = Ioc.Default.GetService<IVolumeInfoFactory>();
            if (volumeInfoFactory is IVolumeInfoFactory factory)
            {
                var info = await factory.BuildVolumeInfo(driveName);
                if (!info.IsEmpty)
                {
                    var database = GetDatabase();
                    var collection = database.GetCollection(Guid.Empty);
                    return new LiteDbSizeRepository(collection);
                }
            }
            //await Task.Yield();
            return new DictionarySizeRepository();
        }

        private SizeDatabase GetDatabase()
        {
            if (database is null)
            {
                string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "foldersizes.db");
                database = new SizeDatabase(path);
            }
            return database;
        }
    }
}
