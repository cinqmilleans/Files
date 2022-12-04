using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.CommandManager
{
	public interface ICommandContext
	{
		event CommandContextChangedEventHandler? Changed;

		IShellPage ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
