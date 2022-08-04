using System.Collections.Generic;
using System.Windows.Input;

namespace Files.Uwp.Command
{
    public interface IKeyboardCommand : ICommand
    {
        ICollection<ShortKey> ShortKeys { get; }
    }
}
