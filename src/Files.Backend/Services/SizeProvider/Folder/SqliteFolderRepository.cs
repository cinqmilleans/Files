﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class SqliteFolderRepository : IFolderRepository
    {
        private readonly SqliteConnection connection = new(@"Data Source=:memory:");

        private readonly int driveID = 1;

        private SqliteFolderRepository(SqliteConnection connection, int driveID) => this.driveID = driveID;

        public static async Task<SqliteFolderRepository> CreateAsync(Guid driveGuid, CancellationToken cancellationToken = default)
        {
            var connection = new SqliteConnection(@"Data Source=:memory:");
            await CreateTablesAsync(cancellationToken);
            return new SqliteFolderRepository(connection, driveID);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await CreateTablesAsync(cancellationToken);
        }


        public async Task<IFolder?> GetFolder(string path, CancellationToken cancellationToken = default)
        {
            const string query = @"SELECT [Level], [LocalSize], [GlobalSize] FROM [Folder] WHERE [Path] = '$path'";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$path", path);
            var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!reader.Read())
            {
                return null;
            }

            return new Folder
            {
                Path = path,
                Level = (ushort)reader.GetInt16(0),
                LocalSize = (ulong)reader.GetInt64(1),
                GlobalSize = (ulong)reader.GetInt64(2),
            };
        }
        public async IAsyncEnumerable<IFolder> GetFolders
            (string rootPath, ushort levelCount = 0, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            const string query = @"SELECT [Level], [LocalSize], [GlobalSize] FROM [Folder]"
                + " WHERE Path LIKE '$path%' AND [Level] BETWEEN {minLevel} AND {maxLevel}";

            int rootLevel = GetLevel(rootPath);
            int maxLevel = levelCount is 0 || levelCount > ushort.MaxValue - rootLevel ? ushort.MaxValue : rootLevel + levelCount;

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$path", rootPath);
            command.Parameters.AddWithValue("$minLevel", rootLevel);
            command.Parameters.AddWithValue("$maxLevel", maxLevel);

            var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync())
            {
                yield return new Folder
                {
                    Path = reader.GetString(0),
                    Level = (ushort)reader.GetInt32(1),
                    LocalSize = (ulong)reader.GetInt64(2),
                    GlobalSize = (ulong)reader.GetInt64(3),
                };
            }
        }

        public async Task PutFolder(IFolder folder, CancellationToken cancellationToken = default)
        {
            const string query = @"INSERT INTO [Folder] ([Path], [Level], [LocalSize], [GlobalSize]) VALUES ('$path', $level, $localSize, $globalSize)";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$path", folder.Path);
            command.Parameters.AddWithValue("$level", folder.Level);
            command.Parameters.AddWithValue("$localSize", folder.LocalSize);
            command.Parameters.AddWithValue("$globalSize", folder.GlobalSize);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        public async Task PutFolders(IEnumerable<IFolder> folders, CancellationToken cancellationToken = default)
        {
            const string query = @"INSERT INTO [Folder] ([Path], [Level], [LocalSize], [GlobalSize]) VALUE ('$path', $level, $localSize, '$globalSize)";

            using var transaction = connection.BeginTransaction();

            var command = new SqliteCommand(query, connection, transaction);

            var pathParameter = command.Parameters.AddWithValue("$path", null);
            var LevelParameter = command.Parameters.AddWithValue("$level", null);
            var localSizeParameter = command.Parameters.AddWithValue("$localSize", null);
            var globalSizeParameter = command.Parameters.AddWithValue("$globalSize", null);

            foreach (Folder folder in folders)
            {
                pathParameter.Value = folder.Path;
                LevelParameter.Value = folder.Level;
                localSizeParameter.Value = folder.LocalSize;
                globalSizeParameter.Value = folder.GlobalSize;

                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            transaction.Commit();
        }

        public async Task DeleteFolder(string path, CancellationToken cancellationToken = default)
        {
            const string query = @"DELETE FROM [Folder] WHERE [Path] = '$path'";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$path", path);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        public async Task DeleteFolders(string rootPath, CancellationToken cancellationToken = default)
        {
            const string query = @"DELETE FROM [Folder] WHERE [Path] LIKE '$path%'";

            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("$path", rootPath);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }


        public async Task<ulong?> GetSizeAsync(string path, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT [Size] FROM [Folder] WHERE [DriveID] = '$driveID' [Path] = '$path'";
            command.Parameters.AddWithValue("$driveID", driveID);
            command.Parameters.AddWithValue("$path", path);
            var size = await command.ExecuteScalarAsync(cancellationToken);
            return (ulong?)size;
        }

        public async Task SetSizeAsync(string path, ulong size, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE [Folder] SET [Size] = '$size' WHERE [DriveID] = '$driveID' [Path] = '$path'";
            command.Parameters.AddWithValue("$driveID", driveID);
            command.Parameters.AddWithValue("$path", path);
            var size = await command.ExecuteScalarAsync(cancellationToken);
            return (ulong?)size;

        }

        Task ClearAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);


        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        private async Task CreateTablesAsync(CancellationToken cancellationToken = default)
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

        private static ushort GetLevel(string path) => (ushort)path.Count(c => c is '\\' or '/');

        public class Folder : IFolder
        {
            public string Path { get; init; } = string.Empty;
            public ushort Level { get; init; }

            public ulong LocalSize { get; init; }
            public ulong GlobalSize { get; init; }
        }
    }
}
