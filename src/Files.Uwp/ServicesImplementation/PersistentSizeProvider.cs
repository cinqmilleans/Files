using Files.Backend.Services.SizeProvider;
using Files.Uwp.Filesystem.StorageItems;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.Uwp.ServicesImplementation
{
    public class PersistentSizeProvider : ISizeProvider
    {
        public event EventHandler<SizeChangedEventArgs> SizeChanged;

        //public static void CreateDatabase()
        //{
            private readonly SqliteConnection connection;

            public Task CleanAsync()
            {
                throw new NotImplementedException();
            }



            public Task UpdateAsync(string path, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

        public bool TryGetSize(string path, out ulong size)
        {
//            String tableCommand = "SELECT [GlobalSize] FROM Folder WHERE

//                command.CommandText =
//@"
//    INSERT INTO user (name)
//    VALUES ($name)
//";
//            command.Parameters.AddWithValue("$name", name);

            //            "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
            //            "Text_Entry NVARCHAR(2048) NULL)";

            //        SqliteCommand createTable = new SqliteCommand(tableCommand, connection);

            //        createTable.ExecuteReader();

        }



        public void Dispose() => connection.Dispose();





            //SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            //using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            //{
            //    db.Open();
            //    String tableCommand = "CREATE TABLE IF NOT EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY AUTOINCREMENT, Text_Entry NVARCHAR(2048) NULL)";
            //    SqliteCommand createTable = new SqliteCommand(tableCommand, db);
            //    try
            //    {
            //        createTable.ExecuteReader();
            //    }
            //    catch (SqliteException e)
            //    {
            //        //Do nothing
            //    }
            //}

            //var file = StorageFile.cr ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            //string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            //using (SqliteConnection connection = new(@"Data Source=:memory:;Version=3"))
            //{
            //    connection.Open();

            //        String tableCommand = "CREATE TABLE IF NOT " +
            //            "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
            //            "Text_Entry NVARCHAR(2048) NULL)";

            //        SqliteCommand createTable = new SqliteCommand(tableCommand, connection);

            //        createTable.ExecuteReader();
            //}

            /*Batteries_V2.Init();

            //string temp = ApplicationData.Current.LocalSettings.Values.Get("TEMP", "") ?? string.Empty;
            string source = @"C:\USB\test.sqlite";


            using var connection = new SqliteConnection($"Data Source={source}");
            connection.Open();
            using (var c1 = new SqliteCommand(CreateCommand, connection))
                c1.ExecuteNonQuery();
            using (var c2 = new SqliteCommand("INSERT INTO Folder (Path, Size) VALUES ('Alic', '7045551212') ON CONFLICT (Path) DO UPDATE SET Size = Size", connection))
                c2.ExecuteNonQuery();

            using var results = new SqliteCommand("SELECT Size FROM Folder WHERE Path = 'Alic'", connection);
            ulong size = (ulong)(results.ExecuteScalar() ?? 0);
            connection.Close();*/

        /*private async Task CreateDatabase()
        {
            //using (SqliteConnection connection = new(@"Data Source=:memory:;Version=3"))
            //{
            //    connection.Open();

            //        String tableCommand = "CREATE TABLE IF NOT " +
            //            "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
            //            "Text_Entry NVARCHAR(2048) NULL)";

            //        SqliteCommand createTable = new SqliteCommand(tableCommand, connection);

            //        createTable.ExecuteReader();
            //}

        }*/

        private interface IFolderRepository : IDisposable
        {
            Task<Folder> GetFolder(string path, CancellationToken cancellationToken = default);
            IAsyncEnumerable<Folder> GetFolders(string rootPath, int maxLevel = 0, CancellationToken cancellationToken = default);

            Task PutFolder(Folder folder, CancellationToken cancellationToken = default);
            Task PutFolders(IEnumerable<Folder> folders, CancellationToken cancellationToken = default);

            Task DeleteFolder(string path, CancellationToken cancellationToken = default);
            Task DeleteFolders(string rootPath, CancellationToken cancellationToken = default);
        }

        private class DatabaseFolderRepository : IFolderRepository
        {
            private readonly SqliteConnection connection = new(@"Data Source=:memory:");

            public async Task Initialize(CancellationToken cancellationToken = default)
            {
                const string query = @"CREATE TABLE Folder ("
                    + "[Path] VARCHAR(MAX) NOT NULL UNIQUE CHECK ([Path] NOT LIKE '%[\\/]'), "
                    + "[Level] SMALLINT NOT NULL UNIQUE, "
                    + "[LocalSize] BIGINT NOT NULL DEFAULT 0 CHECK(LocalSize >= 0), "
                    + "[GlobalSize] BIGINT NOT NULL DEFAULT 0 CHECK(GlobalSizeSize >= 0), "
                    + "CHECK(GlobalSize >= LocalSize)"
                + ");";

                using var command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            public async Task<Folder> GetFolder(string path, CancellationToken cancellationToken = default)
            {
                const string query = @"SELECT Level, LocalSize, GlobalSize FROM Folder WHERE Path = '$path'";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("$path", path);
                var reader = await command.ExecuteReaderAsync(cancellationToken);

                if (reader.Read())
                {
                    return new Folder
                    {
                        Path = path,
                        Level = (int)reader["Level"],
                        LocalSize = (ulong)reader["LocalSize"],
                        GlobalSize = (ulong)reader["GlobalSize"],
                    };
                }

                return null;
            }
            public async IAsyncEnumerable<Folder> GetFolders
                (string rootPath, int maxLevel = 0, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                const string query = @"SELECT Path, Level, LocalSize, GlobalSize FROM Folder "
                    + "WHERE (Path = '$path' OR Path LIKE '$path[\\/]%') AND Level <= {level}";

                int rootLevel = GetLevel(rootPath);
                int level = maxLevel <= 0 ? int.MaxValue : rootLevel + maxLevel;

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("$path", rootPath);
                command.Parameters.AddWithValue("$level", level);

                var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (reader.Read())
                {
                    yield return new Folder
                    {
                        Path = (string)reader["Path"],
                        Level = (int)reader["Level"],
                        LocalSize = (ulong)reader["LocalSize"],
                        GlobalSize = (ulong)reader["GlobalSize"],
                    };
                }
            }

            public async Task PutFolder(Folder folder, CancellationToken cancellationToken = default)
            {
                const string query = @"INSERT INTO Folder ([Path], [Level], [LocalSize], [GlobalSize]) VALUE ('$path', $level, $localSize, '$globalSize)";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("$path", folder.Path);
                command.Parameters.AddWithValue("$level", folder.Level);
                command.Parameters.AddWithValue("$localSize", folder.LocalSize);
                command.Parameters.AddWithValue("$globalSize", folder.GlobalSize);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            public async Task PutFolders(IEnumerable<Folder> folders, CancellationToken cancellationToken = default)
            {
                const string query = @"INSERT INTO Folder ([Path], [Level], [LocalSize], [GlobalSize]) VALUE ('$path', $level, $localSize, '$globalSize)";

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
                const string query = @"DELETE Folder WHERE Path = '$path'";
                await ExecuteQuery(query, path, cancellationToken);
            }
            public async Task DeleteFoldersAndDescendants(IEnumerable<string> paths, CancellationToken cancellationToken = default)
            {
                const string query = @"DELETE Folder WHERE Path = '$path' OR Path LIKE '$path[\\/]%'";
                await ExecuteQuery(query, paths, cancellationToken);
            }



            public void Dispose() => connection.Dispose();

            private static int GetLevel(string path) => path.Count(c => c is '\\' or '/');

            private async Task ExecuteQuery(string query, string path, CancellationToken cancellationToken = default)
            {
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("$path", path);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            public async Task ExecuteQuery(string query, IEnumerable<string> paths, CancellationToken cancellationToken = default)
            {
                using var transaction = connection.BeginTransaction();

                var command = new SqliteCommand(query, connection, transaction);
                var parameter = command.Parameters.AddWithValue("$path", null);

                foreach (string path in paths)
                {
                    parameter.Value = path;
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                transaction.Commit();
            }
            private async Task ExecuteQuery(string query, Folder folder, CancellationToken cancellationToken = default)
            {
            }
            private async Task ExecuteQuery(string query, IEnumerable<Folder> folders, CancellationToken cancellationToken = default)
            {
            }
        }
    }
}
