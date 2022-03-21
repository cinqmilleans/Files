using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Backend.Item
{
    public interface IFileItem : IItem
    {
        FileAttributes FileAttribute { get; }

        string Extension { get; }

        ByteSize Size { get; }

        DateTime DateCreated { get; }
        DateTime DateModified { get; }
        DateTime DateAccessed { get; }

        BitmapImage? MainImage { get; }
        BitmapImage? OverlayImage { get; }
    }
}
