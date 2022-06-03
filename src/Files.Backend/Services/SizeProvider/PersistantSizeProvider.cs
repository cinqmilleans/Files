using Files.Shared.Extensions;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.Backend.Services.SizeProvider
{
    public class PersistantSizeProvider : ISizeProvider
    {
        private const int CacheLevel = 3;
        private const int EventLevel = 2;

        private const string DataSource = "DatabaseC";

        private readonly SizedPathEnumerator enumerator = new();

        public event EventHandler<SizeChangedEventArgs>? SizeChanged;

        public Task CleanAsync() => Task.CompletedTask;

        public async Task UpdateAsync(string path, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (TryGetSize(path, out ulong cachedSize))
            {
                RaiseSizeChanged(path, cachedSize, SizeChangedValueState.Intermediate);
            }
            else
            {
                RaiseSizeChanged(path, 0, SizeChangedValueState.None);
            }

            ulong size = 0;
            var cache = new List<Folder>();

            var folders = enumerator.EnumerateSizedFolders(path).WithCancellation(cancellationToken);
            await foreach (var folder in folders)
            {
                if (folder.Level <= CacheLevel)
                {
                    await Task.Yield();
                    cache.Add(folder);
                }

                if (folder.Level is 0)
                {
                    RaiseSizeChanged(path, size, SizeChangedValueState.Final);
                }
                else if (folder.Level <= EventLevel)
                {
                    size += folder.Level is EventLevel ? folder.GlobalSize : folder.LocalSize;
                    RaiseSizeChanged(path, size, SizeChangedValueState.Intermediate);
                }
            }
        }

        public bool TryGetSize(string path, out ulong size)
        {
            throw new NotImplementedException();
        }

        public void Dispose() {}

        private void RaiseSizeChanged(string path, ulong newSize, SizeChangedValueState valueState)
            => SizeChanged?.Invoke(this, new SizeChangedEventArgs(path, newSize, valueState));

        public static void CreateDatabase()
        {

            //SqliteEngine.UseWinSqlite3(); //Configuring library to use SDK version of SQLite
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();
                String tableCommand = "CREATE TABLE IF NOT EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY AUTOINCREMENT, Text_Entry NVARCHAR(2048) NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                try
                {
                    createTable.ExecuteReader();
                }
                catch (SqliteException e)
                {
                    //Do nothing
                }
            }

            //var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("sqliteSample.db", CreationCollisionOption.OpenIfExists);
            //string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "sqliteSample.db");
            //using (SqliteConnection db =
            //   new SqliteConnection(@"Filename='C:\USB\test.sqlite'"))
            //{
            //    db.Open();

            //    String tableCommand = "CREATE TABLE IF NOT " +
            //        "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY, " +
            //        "Text_Entry NVARCHAR(2048) NULL)";

            //    SqliteCommand createTable = new SqliteCommand(tableCommand, db);

            //    createTable.ExecuteReader();


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
        }

        private const string CreateCommand
            = "CREATE TABLE IF NOT EXISTS Folder (Path VARCHAR(MAX) NOT NULL UNIQUE, Size UNSIGNED BIGINT NOT NULL)";
    }
}
