using Files.App.Filesystem;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Files.App.Commands
{
	public interface ICommandContext : INotifyPropertyChanged, INotifyPropertyChanging
	{
		IShellPage? ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}
}
