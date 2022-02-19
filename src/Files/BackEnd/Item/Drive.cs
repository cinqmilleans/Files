using Files.Common;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace Files.BackEnd
{
    public interface IDriveItem : IItem
    {
        DriveTypes DriveType { get; }

        ByteSize UsedSpace { get; }
        ByteSize FreeSpace { get; }
        ByteSize TotalSpace { get; }

        Uri IconSource { get; }
        byte[] IconImage { get; }
    }

    [Flags]
    public enum DriveTypes : ushort
    {
        Unknown,
        Fixed,
        Removable,
        Network,
        Ram,
        CDRom,
        FloppyDisk,
        NoRootDirectory,
        Virtual,
        Cloud,
    }

    /*public class DriveItemProvider
    {
        public IEnumerable<IDriveItem> GetDrives()
        {
            var factory = new DriveItemFactory
            {
                LoadSpaces = true;
                LoadIcons = true;
            };
        }
    }*/

    internal class DriveItemFactory
    {
        private readonly Logger logger = App.Logger;

        private readonly DriveTypeConverter driveTypeConverter = new();

        public bool LoadSpaces { get; set; } = false;
        public bool LoadIcons { get; set; } = false;

        public async Task<IDriveItem> Build(DeviceInformation information)
        {
            string id = information.Id;

            StorageFolder root;
            try
            {
                root = StorageDevice.FromId(id);
            }
            catch (Exception e)
            {
                logger.Warn($"{e.GetType()}: Attempting to add the device, {information.Name}, "
                    + $"failed at the StorageFolder initialization step. This device will be ignored. Device ID: {id}");
                return null;
            }

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

        private static string ToNormalizedPath(string path)
            => Helpers.PathNormalization.NormalizePath(path);

        private static string ToPath(StorageFolder root)
            => string.IsNullOrEmpty(root.Path) ? $"\\\\?\\{root.Name}\\" : root.Path;

        private class DriveTypeConverter
        {
            private readonly string driveAPath = ToNormalizedPath("A:");
            private readonly string driveBPath = ToNormalizedPath("B:");

            public DriveTypes ToDriveType(string path)
            {
                try
                {
                    var info = new DriveInfo(path);
                    return ToDriveType(info);
                }
                catch (ArgumentException)
                {
                    return DriveTypes.Removable;
                }
                catch
                {
                    return DriveTypes.Unknown;
                }
            }

            private DriveTypes ToDriveType(DriveInfo info)
            {
                if (info.DriveType is DriveType.Unknown)
                {
                    string drivePath = ToNormalizedPath(info.Name);
                    if (drivePath == driveAPath || drivePath == driveBPath)
                    {
                        return DriveTypes.FloppyDisk;
                    }
                }

                return info.DriveType switch
                {
                    DriveType.Fixed => DriveTypes.Fixed,
                    DriveType.Removable => DriveTypes.Removable,
                    DriveType.Network => DriveTypes.Network,
                    DriveType.Ram => DriveTypes.Ram,
                    DriveType.CDRom => DriveTypes.CDRom,
                    DriveType.NoRootDirectory => DriveTypes.NoRootDirectory,
                    _ => DriveTypes.Unknown,
                };
            }
        }

        private class DriveItem : ObservableObject, IDriveItem
        {
            public string Path { get; init; }
            public string Name { get; init; }

            public DriveTypes DriveType { get; init; } = DriveTypes.Unknown;

            private ByteSize usedSpace = 0L;
            public ByteSize UsedSpace
            {
                get => usedSpace;
                set => SetProperty(ref usedSpace, value);
            }

            private ByteSize freeSpace = 0L;
            public ByteSize FreeSpace
            {
                get => freeSpace;
                set => SetProperty(ref freeSpace, value);
            }

            private ByteSize totalSpace = 0L;
            public ByteSize TotalSpace
            {
                get => totalSpace;
                set => SetProperty(ref totalSpace, value);
            }

            public Uri IconSource { get; set; }
            public byte[] IconImage { get; set; }

            public DriveItem()
            {
            }

            public async Task UpdateSpaces(StorageFolder root)
            {
                if (root is null)
                {
                    return;
                }
                try
                {
                    var properties = await root.Properties
                        .RetrievePropertiesAsync(new[] { "System.Capacity", "System.Capacity" })
                        .AsTask()
                        .WithTimeoutAsync(TimeSpan.FromSeconds(5));

                    if (properties is null)
                    {
                        UsedSpace = FreeSpace = TotalSpace = 0L;
                    }
                    else
                    {
                        TotalSpace = properties["System.Capacity"] is ulong totalSpace ? totalSpace : 0L;
                        FreeSpace = properties["System.FreeSpace"] is ulong freeSpace ? freeSpace : 0L;
                        UsedSpace = FreeSpace <= TotalSpace ? TotalSpace - FreeSpace : ByteSize.MinValue;
                    }
                }
                catch
                {
                    UsedSpace = FreeSpace = TotalSpace = 0L;
                }
            }
        }
    }
}
