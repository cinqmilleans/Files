using Files.App.Filesystem;
using System;
using System.Collections.Immutable;

namespace Files.App.CommandManager
{
	public class CommandContext : ICommandContext
	{
		public event CommandContextChangedEventHandler? Changed;

		private IShellPage shellPage;
		public IShellPage ShellPage
		{
			get => shellPage;
			set
			{
				if (shellPage != value)
				{
					shellPage = value;
					OnChanged();
				}
			}
		}

		private IImmutableList<ListedItem> items = ImmutableList<ListedItem>.Empty;
		public IImmutableList<ListedItem> Items
		{
			get => items;
			set
			{
				if (items != value)
				{
					items = value;
					OnChanged();
				}
			}
		}

		public CommandContext(IShellPage shellPage) => this.shellPage = shellPage;

		private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
	}
}
