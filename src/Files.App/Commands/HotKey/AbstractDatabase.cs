using Files.Shared.Extensions;
using LiteDB;
using System;
using System.Text;
using IO = System.IO;

namespace Files.App.Commands
{
	internal interface IDatabase : IDisposable
	{
		void Import(string json);
		string Export();
	}

	internal abstract class AbstractDatabase<T> : IDatabase
	{
		protected readonly LiteDatabase database;

		private const string Header = "** This is a LiteDB file **";

		[Obsolete]
		public AbstractDatabase(string connection, bool shared = false)
		{
			SafetyExtensions.IgnoreExceptions(() => CheckDatabaseVersion(connection));
			database = new LiteDatabase(new ConnectionString(connection)
			{
				Mode = shared ? FileMode.Shared : FileMode.Exclusive
			});
		}

		~AbstractDatabase() => Dispose();

		public abstract void Import(string json);
		public abstract string Export();

		public void Dispose()
		{
			database.Dispose();

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {}

		private static void CheckDatabaseVersion(string filename)
		{
			var buffer = new byte[8192 * 2];
			using (var stream = new IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite))
			{
				// read first 16k
				stream.Read(buffer, 0, buffer.Length);

				// checks if v7 (plain or encrypted)
				if (buffer[52] is 7 && Encoding.UTF8.GetString(buffer, 25, Header.Length) is Header)
				{
					return; // version 4.1.4
				}
			}
			IO.File.Delete(filename); // recreate DB with correct version
		}
	}
}
