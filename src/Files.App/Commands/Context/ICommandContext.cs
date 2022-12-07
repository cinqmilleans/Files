using Files.App.DataModels;
using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.Commands
{
	public interface ICommandContext
	{
		event CommandContextChangedEventHandler? Changed;

		AppModel? AppModel { get; }
		IShellPage? ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
