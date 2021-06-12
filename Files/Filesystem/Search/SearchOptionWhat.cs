using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class TypeSearchOptionKey : ISearchOptionKey
    {
        public string Text => "type";
        public string Label => "Type of file";

        public ISearchOptionFormat Format { get; } = new TypeSearchOptionFormat();

        public string[] SuggestionValues { get; } = new string[]
        {
            "document", "picture", "audio", "video"
        };

        public string ProvideFilter(ISearchOptionValue value)
        {
            if (value is TypeSearchOptionValue type)
            {
                return ProvideFilter(type);
            }
            throw new ArgumentException();
        }
        public string ProvideFilter(TypeSearchOptionValue value)
        {
            return value.Text switch
            {
                "document" => "System.Kind:=System.Kind#Document",
                "picture" => "System.Kind:=System.Kind#Picture",
                "audio" => "System.Kind:=System.Kind#Music",
                "video" => "System.Kind:=System.Kind#Video",
                _ => throw new ArgumentException()
            };
        }
    }

    public class TypeSearchOptionFormat : ISearchOptionFormat
    {
        private readonly string[] types = new string[]
        {
            "document", "picture", "audio", "video"
        };

        public bool CanParseValue(string value) => types.Contains(value.ToLower());
        public ISearchOptionValue ParseValue(string value) => new TypeSearchOptionValue(value);
    }

    public class TypeSearchOptionValue : ISearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public TypeSearchOptionValue(string value)
        {
            Text = value.ToLower();

            Label = Text switch
            {
                "document" => "Only document files",
                "picture" => "Only picture files",
                "audio" => "Only audio files",
                "video" => "Only video files",
                _ => throw new ArgumentException()
            };
        }
    }
}
