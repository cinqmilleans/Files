using Files.Enums;
using Files.Filesystem.Search;

namespace Files.ViewModels.Search
{
    public interface ISearchHeaderViewModel
    {
        SearchKeys Key { get; }

        string Glyph { get; }
        string Label { get; }
        string Description { get; }
    }

    internal class SearchHeaderViewModel : ISearchHeaderViewModel
    {
        private readonly ISearchHeader header;

        public SearchKeys Key => header.Key;

        public string Glyph => header.Glyph;
        public string Label => header.Label;
        public string Description => header.Description;

        public SearchHeaderViewModel(ISearchHeader header) => this.header = header;
    }
}
