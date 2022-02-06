using Windows.UI.Xaml.Media.Imaging;

namespace Files.Article.ViewModel
{
    public interface IIconViewModel
    {
        BitmapImage Value { get; }

        bool CanLoad { get; }
    }

    internal struct IconViewModel : IIconViewModel
    {
        public BitmapImage Value { get; }

        public bool CanLoad => Value is not null;

        public IconViewModel(BitmapImage image) => Value = image;
    }
}
