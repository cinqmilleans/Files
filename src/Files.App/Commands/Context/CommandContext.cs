using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.Filesystem;
using System.Collections.Immutable;

namespace Files.App.Commands
{
	public class CommandContext : ObservableObject, ICommandContext, ICommandContextWriter
	{
		private IShellPage? shellPage;
		public IShellPage? ShellPage
		{
			get => shellPage;
			set => SetProperty(ref shellPage, value);
		}

		private IImmutableList<ListedItem> items = ImmutableList<ListedItem>.Empty;
		public IImmutableList<ListedItem> Items
		{
			get => items;
			set => SetProperty(ref items, value);
		}
	}
}
