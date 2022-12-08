using Files.App.DataModels;
using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.Commands
{
	public interface ICommandContextWriter
	{
		IShellPage? ShellPage { get; set; }
		IImmutableList<ListedItem> Items { get; set; }
	}
}
