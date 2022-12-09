using Files.App.Filesystem;
using Files.App.ViewModels;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Files.App.Commands
{
	public interface ICommandContext : INotifyPropertyChanged, INotifyPropertyChanging
	{
		IShellPage? ShellPage { get; }
		ToolbarViewModel? ToolbarViewModel { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
