using Files.App.Filesystem;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Files.App.Actions.HotKeys
{
	internal class ActionContext : IActionContext
	{
		public static ActionContext Empty { get; } = new ActionContext();

		public IShellPage? ShellPage { get; }
		public IImmutableList<ListedItem> Items { get; }

		private ActionContext() => Items = ImmutableList<ListedItem>.Empty;
		public ActionContext(IEnumerable<ListedItem> items) => Items = items.ToImmutableList();

		public ActionContext(IShellPage shellPage)
		{
			ShellPage = shellPage;
			Items = shellPage.PaneHolder?.ActivePane?.SlimContentPage?.SelectedItems?.ToImmutableList()
				?? ImmutableList<ListedItem>.Empty;
		}
	}
}
