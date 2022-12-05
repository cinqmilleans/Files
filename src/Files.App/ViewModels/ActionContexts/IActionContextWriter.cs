using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.ViewModels.ActionContexts
{
	public interface IActionContextWriter
	{
		IShellPage? ShellPage { get; set; }
		IImmutableList<ListedItem> Items { get; set; }
	}
}
