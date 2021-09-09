using System;
using Windows.UI.Xaml.Data;

namespace Files.Converters
{
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter as string;
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString();
            }
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
