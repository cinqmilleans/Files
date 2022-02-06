using Files.Article.Extension;

namespace Files.Article.ViewModel
{
    public interface ISizeViewModel
    {
        long Size { get; }

        public string Label { get; }
        public string LongLabel { get; }
    }

    internal struct NotCalculatedSizeViewModel : ISizeViewModel
    {
        private static readonly string label = "ItemSizeNotCalculated".GetLocalized();

        public long Size => 0;
        public string Label => label;
        public string LongLabel => label;
    }

    internal struct SizeViewModel : ISizeViewModel
    {
        public long Size { get; }

        public string Label => Size.ToSizeString();
        public string LongLabel => Size.ToLongSizeString();

        public SizeViewModel(long size) => Size = size;
    }
}
