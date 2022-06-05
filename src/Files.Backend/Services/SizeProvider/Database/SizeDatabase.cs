using LiteDB;
using System;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizeDatabase
    {
        private readonly LiteDatabase database;

        public SizeDatabase(string connection, bool shared = false)
        {
            database = new LiteDatabase(new ConnectionString(connection)
            {
                Mode = shared ? FileMode.Shared : FileMode.Exclusive
            });

            CreateDriveCollection();
        }

        public LiteCollection<SizedFolder> GetCollection(Guid volumeGuid)
        {
            UpdateDrive(volumeGuid);

            var collection = database.GetCollection<SizedFolder>($"drive_{volumeGuid}");
            collection.EnsureIndex(nameof(SizedFolder.Path), true);
            return collection;
        }

        public void Clear() => Clear(new DateTime(9999, 12, 31));
        public void Clear(DateTime maxDateTime)
        {
            var driveCollection = GetDriveCollection();

            var oldDrives = driveCollection.Find(d => d.Updated <= maxDateTime);
            foreach (var oldDrive in oldDrives)
            {
                database.DropCollection($"drive_{oldDrive.VolumeGuid}");
            }

            driveCollection.Delete(d => d.Updated <= maxDateTime);
        }

        private LiteCollection<SizedDrive> GetDriveCollection()
            => database.GetCollection<SizedDrive>("drives");

        private LiteCollection<SizedDrive> CreateDriveCollection()
        {
            var collection = GetDriveCollection();
            collection.EnsureIndex(nameof(SizedDrive.VolumeGuid), true);
            collection.EnsureIndex(nameof(SizedDrive.Updated), false);
            return collection;
        }

        private void UpdateDrive(Guid volumeGuid)
        {
            var drives = GetDriveCollection();

            var drive = drives.FindOne(d => d.VolumeGuid == volumeGuid);
            if (drive is null)
            {
                drive = new SizedDrive(volumeGuid);
                drives.Insert(drive);
            }
            else
            {
                drive.Updated = DateTime.Now;
                drives.Update(drive);
            }
        }

        internal class SizedDrive
        {
            [BsonId] public int Id { get; init; }
            public Guid VolumeGuid { get; init; }
            public DateTime Updated { get; set; } = DateTime.Now;

            public SizedDrive() {}
            public SizedDrive(Guid volumeGuid) => VolumeGuid = volumeGuid;
        }
    }
}
