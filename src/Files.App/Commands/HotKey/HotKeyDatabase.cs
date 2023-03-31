using Files.Shared.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static Common.FileTagsDb;
using IO = System.IO;
using Files.Shared.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IO = System.IO;

namespace Files.App.Commands
{
	internal interface IHotKeyDatabase : IDatabase
	{
		IEnumerable<CustomHotKey> GetAll();
		IEnumerable<CustomHotKey> GetByCode(CommandCodes code);
	}

	internal class HotKeyDatabase : AbstractDatabase<CustomHotKey>, IHotKeyDatabase
	{
		private const string fileName = "CustomHotKeys";

		[Obsolete]
		public HotKeyDatabase(string connection, bool shared = false) : base(connection, shared) => Update();

		public IEnumerable<CustomHotKey> GetAll()
			=> database.GetCollection<CustomHotKey>(fileName).FindAll();
		public IEnumerable<CustomHotKey> GetByCode(CommandCodes code)
			=> database.GetCollection<CustomHotKey>(fileName).Find(item => item.Code == code);

		public override void Import(string json)
		{
			var dataValues = JsonSerializer.DeserializeArray(json);
			var collection = database.GetCollection(fileName);
			collection.Delete(Query.All());
			collection.InsertBulk(dataValues.Select(value => value.AsDocument));
		}
		public override string Export()
		{
			var items = database.GetCollection(fileName).FindAll();
			return JsonSerializer.Serialize(new BsonArray(items));
		}



		/*public void SetTags(string filePath, ulong? frn, string[]? tags)
		{
			// Get a collection (or create, if doesn't exist)
			var col = db.GetCollection<TaggedFile>(TaggedFiles);

			var tmp = _FindTag(filePath, frn);
			if (tmp is null)
			{
				if (tags is not null && tags.Any())
				{
					// Insert new tagged file (Id will be auto-incremented)
					var newTag = new TaggedFile()
					{
						FilePath = filePath,
						Frn = frn,
						Tags = tags
					};
					col.Insert(newTag);
					col.EnsureIndex(x => x.Frn);
					col.EnsureIndex(x => x.FilePath);
				}
			}
			else
			{
				if (tags is not null && tags.Any())
				{
					// Update file tag
					tmp.Tags = tags;
					col.Update(tmp);
				}
				else
				{
					// Remove file tag
					col.Delete(tmp.Id);
				}
			}
		}

		private TaggedFile? _FindTag(string? filePath = null, ulong? frn = null)
		{
			// Get a collection (or create, if doesn't exist)
			var col = db.GetCollection<TaggedFile>(TaggedFiles);

			if (filePath is not null)
			{
				var tmp = col.FindOne(x => x.FilePath == filePath);
				if (tmp is not null)
				{
					if (frn is not null)
					{
						// Keep entry updated
						tmp.Frn = frn;
						col.Update(tmp);
					}

					return tmp;
				}
			}

			if (frn is not null)
			{
				var tmp = col.FindOne(x => x.Frn == frn);
				if (tmp is not null)
				{
					if (filePath is not null)
					{
						// Keep entry updated
						tmp.FilePath = filePath;
						col.Update(tmp);
					}

					return tmp;
				}
			}

			return null;
		}

		public void UpdateTag(string oldFilePath, ulong? frn = null, string? newFilePath = null)
		{
			// Get a collection (or create, if doesn't exist)
			var col = db.GetCollection<TaggedFile>(TaggedFiles);
			var tmp = col.FindOne(x => x.FilePath == oldFilePath);
			if (tmp is not null)
			{
				if (frn is not null)
				{
					tmp.Frn = frn;
					col.Update(tmp);
				}

				if (newFilePath is not null)
				{
					tmp.FilePath = newFilePath;
					col.Update(tmp);
				}
			}
		}

		public void UpdateTag(ulong oldFrn, ulong? frn = null, string? newFilePath = null)
		{
			// Get a collection (or create, if doesn't exist)
			var col = db.GetCollection<TaggedFile>(TaggedFiles);
			var tmp = col.FindOne(x => x.Frn == oldFrn);
			if (tmp is not null)
			{
				if (frn is not null)
				{
					tmp.Frn = frn;
					col.Update(tmp);
				}

				if (newFilePath is not null)
				{
					tmp.FilePath = newFilePath;
					col.Update(tmp);
				}
			}
		}

		public string[]? GetTags(string? filePath = null, ulong? frn = null)
		{
			return _FindTag(filePath, frn)?.Tags;
		}

		public IEnumerable<TaggedFile> GetAllUnderPath(string folderPath)
		{
			var col = db.GetCollection<TaggedFile>(TaggedFiles);
			return col.Find(x => x.FilePath.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase));
		}

		~FileTagsDb()
		{
			Dispose();
		}

		public void Dispose()
		{
			db.Dispose();
		}

		public void Import(string json)
		{
			var dataValues = JsonSerializer.DeserializeArray(json);
			var col = db.GetCollection(TaggedFiles);
			col.Delete(Query.All());
			col.InsertBulk(dataValues.Select(x => x.AsDocument));
		}

		public string Export()
		{
			return JsonSerializer.Serialize(new BsonArray(db.GetCollection(TaggedFiles).FindAll()));
		}

		private void UpdateDb()
		{
			if (db.Engine.UserVersion == 0)
			{
				var col = db.GetCollection(TaggedFiles);
				foreach (var doc in col.FindAll())
				{
					doc["Tags"] = new BsonValue(new[] { doc["Tag"].AsString });
					doc.Remove("Tags");
					col.Update(doc);
				}
				db.Engine.UserVersion = 1;
			}
		}

		// https://github.com/mbdavid/LiteDB/blob/master/LiteDB/Engine/Engine/Upgrade.cs
		private void CheckDbVersion(string filename)
		{
			var buffer = new byte[8192 * 2];
			using (var stream = new IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite))
			{
				// read first 16k
				stream.Read(buffer, 0, buffer.Length);

				// checks if v7 (plain or encrypted)
				if (Encoding.UTF8.GetString(buffer, 25, "** This is a LiteDB file **".Length) == "** This is a LiteDB file **" &&
					buffer[52] == 7)
				{
					return; // version 4.1.4
				}
			}
			IO.File.Delete(filename); // recreate DB with correct version
		}

		*/
	}
}
