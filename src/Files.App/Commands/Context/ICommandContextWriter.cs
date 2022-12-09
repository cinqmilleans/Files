using Files.App.DataModels;
using Files.App.Filesystem;
using Files.App.ViewModels;
using System.Collections.Immutable;

namespace Files.App.Commands
{
	public interface ICommandContextWriter
	{
		IShellPage? ShellPage { get; set; }
		ToolbarViewModel? ToolbarViewModel { get; set; }
		IImmutableList<ListedItem> Items { get; set; }
	}
}
