using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IO = System.IO;

namespace Files.Backend.Item
{
    [Flags]
    public enum FileAttributes : ushort
    {
        None = 0x0000,
        Archive = 0x0001,
        Compressed = 0x0002,
        Device = 0x0004,
        Directory = 0x0010,
        Encrypted = 0x0020,
        Hidden = 0x0040,
        Offline = 0x0100,
        ReadOnly = 0x0200,
        System = 0x0400,
        Temporary = 0x1000,
    }

    internal static class FileAttributeConverter
    {
        private static readonly IReadOnlyDictionary<IO.FileAttributes, FileAttributes> attributes
            = new ReadOnlyDictionary<IO.FileAttributes, FileAttributes>
            (
                new Dictionary<IO.FileAttributes, FileAttributes>
                {
                    [IO.FileAttributes.Archive] = FileAttributes.Archive,
                    [IO.FileAttributes.Compressed] = FileAttributes.Compressed,
                    [IO.FileAttributes.Device] = FileAttributes.Device,
                    [IO.FileAttributes.Directory] = FileAttributes.Directory,
                    [IO.FileAttributes.Encrypted] = FileAttributes.Encrypted,
                    [IO.FileAttributes.Hidden] = FileAttributes.Hidden,
                    [IO.FileAttributes.Offline] = FileAttributes.Offline,
                    [IO.FileAttributes.ReadOnly] = FileAttributes.ReadOnly,
                    [IO.FileAttributes.System] = FileAttributes.System,
                    [IO.FileAttributes.Temporary] = FileAttributes.Temporary,
                }
            );

        public static FileAttributes ToFileAttribute(this IO.FileAttributes attribute)
            => attributes
                .Where(fileAttribute => attribute.HasFlag(fileAttribute.Key))
                .Select(fileAttribute => fileAttribute.Value)
                .Aggregate((result, attribute) => result | attribute);
    }
}
