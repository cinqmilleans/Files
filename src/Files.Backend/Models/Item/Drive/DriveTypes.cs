using System;
using System.IO;

namespace Files.Backend.Models.Item
{
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

    internal interface IDriveTypeConverter
    {
        DriveTypes ToDriveType(string path);
    }

    internal class DriveTypeConverter : IDriveTypeConverter
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

        private static string ToNormalizedPath(string path)
            => path; // Path.nor Helpers.PathNormalization.NormalizePath(path);
    }
}
