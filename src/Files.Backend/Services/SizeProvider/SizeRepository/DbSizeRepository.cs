using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class DbSizeRepository : IFolderRepository
    {
        private readonly DbConnection connection;

        private readonly int driveID;

        private DbSizeRepository(DbConnection connection, int driveID)
            => (this.connection, this.driveID) = (connection, driveID);

        public static async Task<DbSizeRepository> CreateAsync(Guid driveGuid, CancellationToken cancellationToken = default)
        {
            var connection = await CreateDbConnection(cancellationToken);
            await InitializeTables(connection, cancellationToken);
            int driveID = await InitializeDriveAsync(connection, driveGuid, cancellationToken);
            return new DbSizeRepository(connection, driveID);
        }

        public async Task<ulong?> GetSizeAsync(string path, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT [Size] FROM [Folder] WHERE [DriveID] = '$driveID' [Path] = '$path'";
            AddParameter(command, "driveID", driveID);
            AddParameter(command, "path", path);
            var size = await command.ExecuteScalarAsync(cancellationToken);
            return (ulong?)size;
        }

        public async Task SetSizeAsync(string path, ulong size, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE [Folder] SET [Size] = '$size' WHERE [DriveID] = '$driveID' AND [Path] = '$path'";
            AddParameter(command, "driveID", driveID);
            AddParameter(command, "path", path);
            AddParameter(command, "size", size);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM [Folder] WHERE [DriveID] = '$driveID'";
            AddParameter(command, "driveID", driveID);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM [Folder] WHERE [DriveID] = '$driveID' AND [Path] = '$path'";
            AddParameter(command, "driveID", driveID);
            AddParameter(command, "path", path);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        private static void AddParameter<T>(DbCommand command, string key, T value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = $"${key}";
            parameter.Value = value;
        }

        private static async Task<DbConnection> CreateDbConnection(CancellationToken cancellationToken = default)
            => new SqliteConnection(@"Data Source=:memory:");

        private static async Task InitializeTables(DbConnection connection, CancellationToken cancellationToken = default)
        {
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"CREATE TABLE IF NOT EXISTS [Drive] ("
                + "[ID] INT NOT NULL PRIMARY KEY AUTOINCREMENT, "
                + "[GUID] UNIQUEIDENTIFIER NOT NULL UNIQUE"
            + ") WITHOUT ROWID;";
            await command.ExecuteNonQueryAsync(cancellationToken);

            command.CommandText = @"CREATE TABLE IF NOT EXISTS [Folder] ("
                + "[ID] INT NOT NULL PRIMARY KEY AUTOINCREMENT, "
                + "[DriveID] SMALLINT NOT NULL UNIQUE, "
                + "[Path] TEXT NOT NULL UNIQUE CHECK ([Path] LIKE '%[\\/]'), "
                + "[Size] BIGINT NOT NULL DEFAULT 0 CHECK([LocalSize] >= 0), "
                + "FOREIGN KEY [FK_Folder_DriveID] REFERENCES [Drive]([ID]), "
                + "INDEX [IX_Folder] UNIQUE ([DriveID], [Path])"
            + ") WITHOUT ROWID;";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task<int> InitializeDriveAsync(DbConnection connection, Guid driveGuid, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            AddParameter(command, "guid", driveGuid);

            command.CommandText = @"INSERT OR IGNORE INTO [Drive] ([Guid]) VALUES ('$guid') ";
            await command.ExecuteNonQueryAsync(cancellationToken);

            command.CommandText = @"SELECT ID FROM [Drive] WHERE [Guid] = '$guid'";
            var id = await command.ExecuteScalarAsync(cancellationToken);
            return (int)(id ?? 0);
        }
    }
}
