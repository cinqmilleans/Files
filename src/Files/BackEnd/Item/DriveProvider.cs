using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace Files.BackEnd
{
    internal interface IDriveProvider
    {
        bool LoadSpaces { get; set; }
        bool LoadIcons { get; set; }
    }

    internal class DriveProvider : IDriveProvider
    {
        public bool LoadSpaces { get; set; } = false;
        public bool LoadIcons { get; set; } = false;

        public static async Task<Drive> Build(DeviceInformation information)
        {
            string id = information.Id;


            var drive = new DriveItem
            {
                Path = ToPath(root),
                Name = root.DisplayName,
                //DriveType = driveTypeConverter.ToDriveType(root.Path),
            };

            if (LoadSpaces)
            {
                await drive.UpdateSpaces(root);
            }

            return drive;
        }

        private static StorageFolder ToStorageFolder(DeviceInformation info)
        {
            try
            {
                return StorageDevice.FromId(info.Id);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The DeviceInformation can't be converted to StorageFolder", e);
            }
        }
    }
}
