using Files.App.CommandActions;
using System.Collections.Generic;

namespace Files.App.ViewModels.Commands
{
    public interface ICommandManager
	{
		IRichCommand this[CommandCodes actionCode] { get; }

		IRichCommand NoneCommand { get; }
		IRichCommand HelpCommand { get; }
		IToggleCommand FullScreenCommand { get; }
		IRichCommand LayoutDetailsCommand { get; }
		IRichCommand PropertiesCommand { get; }

		IEnumerable<IRichCommand> EnumerateCommands();
	}
}
