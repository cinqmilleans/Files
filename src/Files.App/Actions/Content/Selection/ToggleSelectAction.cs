using Files.App.Commands;
using Files.App.Extensions;
using Files.App.Filesystem;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class ToggleSelectAction : IAction
	{
		public string Label { get; } = "ToggleSelect".GetLocalizedResource();
		public string Description => "TODO: Need to be described.";

		public HotKey HotKey { get; } = new(Keys.Space, KeyModifiers.Ctrl);

		public bool IsExecutable => GetFocusedElement() is not null;

		public Task ExecuteAsync()
		{
			var focused = GetFocusedElement();
			if (focused is FrameworkElement element && element.DataContext is ListedItem a)
				a.IsChecked = !a.IsChecked;
			else if (focused is ListViewItem b && b.Content is ListedItem c)
				c.IsChecked = !c.IsChecked;
			else if (focused is SelectorItem item)
				item.IsSelected = !item.IsSelected;
			return Task.CompletedTask;
		}

		private static SelectorItem? GetFocusedElement()
		{
			return FocusManager.GetFocusedElement(App.Window.Content.XamlRoot) as SelectorItem;
		}
	}
}
