using System.Collections.Generic;
using System.Windows.Input;

namespace Files.Uwp.Command
{
    public interface ICommandManager
    {
        IEnumerable<ICommand> Commands { get; }
    }
}
