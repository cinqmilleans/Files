using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace Files.Uwp.Command
{
    public interface IAction
    {
        string Label { get; }
        ICommand Command { get; }

        ICollection<ShortKey> ActiveShortKeys { get; }
        ICollection<ShortKey> DefaultShortKeys { get; }
    }

    public class ViewDetailAction : IAction
    {
        public string Label => "Details".GetLocalized();

        public ICommand Command { get; }

        public ICollection<ShortKey> ActiveShortKeys { get; }
        public ICollection<ShortKey> DefaultShortKeys { get; }
    }

    //public class ViewCommandManager : ICommandManager
    //{
    //    private class DetailViewCommand : IKeyboardCommand
    //    {
    //        public event EventHandler CanExecuteChanged;

    //        public string Label => "Details".GetLocalized();

    //        public ShortKey ShortKeys { get; } = new ShortKey(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

    //        public bool CanExecute(object parameter) => true;

    //        public void Execute(object parameter)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    private class DetailViewShortKeyCollection : Collection<ShortKey>, ICustomShortKeyCollection
    //    {
    //        public string Label => "Details".GetLocalized();

    //        public ImmutableArray<ShortKey> DefaultShortKeys { get; } = new List<ShortKey>
    //        {
    //            new ShortKey(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift),
    //        }.ToImmutableArray();
    //    }
    //}
}
