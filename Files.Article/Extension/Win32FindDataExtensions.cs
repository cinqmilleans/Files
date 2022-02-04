using static Files.Article.Helper.NativeFindStorageItemHelper;

namespace Files.Article.Extension
{
    public static class Win32FindDataExtensions
    {
        private const long MAXDWORD = 4294967295;

        public static long GetSize(this WIN32_FIND_DATA findData)
        {
            long sizeLow = findData.nFileSizeLow;
            long sizeHigh = findData.nFileSizeHigh;

            return sizeLow + (sizeLow, sizeHigh) switch
            {
                ( < 0, > 0) => (MAXDWORD + 1) * (sizeHigh + 1),
                (_, > 0) => (MAXDWORD + 1) * sizeHigh,
                ( < 0, _) => MAXDWORD + 1,
                _ => 0,
            };
        }
    }
}