using Files.App.Filesystem;
using System;
using System.Collections.Immutable;

namespace Files.App.ViewModels.ActionContexts
{
	public class ActionContext : IActionContext, IActionContextWriter
	{
		public event ActionContextChangedEventHandler? Changed;

		private IShellPage? shellPage;
		public IShellPage? ShellPage
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

		private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
	}
}
