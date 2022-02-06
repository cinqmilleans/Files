using Files.Article.Extension;
using System;

namespace Files.Article.ViewModel
{
    public interface IDateViewModel
    {
        DateTimeOffset Value { get; }

        string Label { get; }
    }

    internal struct DateViewModel : IDateViewModel
    {
        private const string format = "G";

        public DateTimeOffset Value { get; }

        public string Label => Value.GetFriendlyDateFromFormat(format);

        public DateViewModel(DateTimeOffset date) => Value = date;
    }
}
