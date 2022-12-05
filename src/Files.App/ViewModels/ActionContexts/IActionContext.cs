using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.ViewModels.ActionContexts
{
	public interface IActionContext
	{
		event ActionContextChangedEventHandler? Changed;

		IShellPage? ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
