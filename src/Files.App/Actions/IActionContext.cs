using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.Actions
{
	public interface IActionContext
	{
		IShellPage? ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
