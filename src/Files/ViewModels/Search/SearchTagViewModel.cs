using Files.Filesystem.Search;
using Files.UserControls.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchTagViewModel
    {
        ISearchTag Tag { get; }

        ICommand OpenCommand { get; }
        ICommand DeleteCommand { get; }
    }

    public class ParameterTagViewModel : ISearchTagViewModel
    {
        public ISearchTag Tag { get; }

        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }

        public ParameterTagViewModel(ISearchTag tag)
        {
            Tag = tag;

            OpenCommand = new RelayCommand<ISearchFilter>(Open);
            DeleteCommand = new RelayCommand(tag.Delete);
        }

        private void Open(ISearchFilter middleFilter = null)
        {
            var navigator = Ioc.Default.GetService<ISearchNavigator>();

            if (middleFilter is not null)
            {
                navigator.GoPage(middleFilter);
            }

            navigator.GoPage(Tag.Filter);
        }
    }
}
