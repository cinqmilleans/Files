using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizeProviderFactory : ISizeProviderFactory
    {
        public async Task<ISizeProvider> CreateSizeProvider(string driveName)
        {
            return await CreateSqliteSizeProvider(driveName);
        }

        private async Task<ISizeProvider> CreateSqliteSizeProvider(string driveName)
        {
            var repository = new SqliteFolderRepository();
            var enumerator = new FolderEnumerator();

            await repository.InitializeAsync();
            return new PersistentSizeProvider(repository, enumerator);
        }
    }
}
