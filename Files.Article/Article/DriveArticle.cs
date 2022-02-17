namespace Files.Article.Article
{
    public enum DriveTypes : ushort
    {
        Fixed,
        Removable,
        Network,
        Ram,
        CDRom,
        FloppyDisk,
        Unknown,
        NoRootDirectory,
        VirtualDrive,
        CloudDrive,
    }

    public interface IDriveArticle : IArticle
    {
        DriveTypes DriveType { get; }

        long UsedSpace { get; }
        long TotalSpace { get; }
    }
}
