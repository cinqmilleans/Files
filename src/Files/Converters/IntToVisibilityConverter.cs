using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Files.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public int MinValue { get; set; } = 1;
        public int MaxValue { get; set; } = int.MaxValue;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int n = System.Convert.ToInt32(value);

            return n >= MinValue && n <= MaxValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
