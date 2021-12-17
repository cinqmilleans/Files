using Files.Filesystem.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchFilterViewModel
    {
        ISearchFilter Filter { get; }

        string Key { get; }
        string Glyph { get; }
        string Label { get; }
        string Description { get; }

        ISearchFilterParameter
    }

    public interface ISearchFilterParameter
    {
        string Glyph { get; }
        string Label { get; }
        string Description { get; }

        ICommand OpenCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
