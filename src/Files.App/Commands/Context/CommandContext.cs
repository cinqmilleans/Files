using Files.App.DataModels;
using Files.App.Filesystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Files.App.Commands
{
	public class CommandContext : ICommandContext, ICommandContextWriter
	{
		public event CommandContextChangedEventHandler? Changed;

		private AppModel? appModel;
		public AppModel? AppModel
		{
			get => appModel;
			set => Set(ref appModel, value);
		}

		private IShellPage? shellPage;
		public IShellPage? ShellPage
		{
			get => shellPage;
			set => Set(ref shellPage, value);
		}

		private IImmutableList<ListedItem> items = ImmutableList<ListedItem>.Empty;
		public IImmutableList<ListedItem> Items
		{
			get => items;
			set => Set(ref items, value);
		}

		private void Set<T>(ref T field, T value)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;

			field = value;
			Changed?.Invoke(this, EventArgs.Empty);
		}
	}
}
