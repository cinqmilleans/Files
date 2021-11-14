using System;
using Windows.UI.Xaml.Data;

namespace Files.Converters
{
    public class ToStringConverter : IValueConverter
    {
        public string Format { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = (parameter as string) ?? Format ?? "{0}";
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
