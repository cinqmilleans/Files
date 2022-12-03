using CommunityToolkit.WinUI;
using Files.App.Extensions;
using Files.App.Filesystem;
using Files.App.ViewModels;
using Files.App.Views;
using Microsoft.UI.Dispatching;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.App.Actions.Action
{
	internal class OpenFolderInNewTabAction : AsyncAction
	{
		public override ActionCodes Code => ActionCodes.OpenFolderInNewTab;
		public override string Label => "BaseLayoutItemContextFlyoutOpenInNewTab/Text".GetLocalizedResource();

		public override IGlyph Glyph { get; } = new Glyph("\uF113") { Family = "CustomGlyph" };

		public override bool CanExecute()
		{
			var items = GetItems();

			return items.Count < 5
				&& items.All(i => i.PrimaryItemAttribute is StorageItemTypes.Folder);
		}

		public override async Task ExecuteAsync()
		{
			var items = GetItems();
			foreach (var item in items)
			{
				await App.Window.DispatcherQueue.EnqueueAsync(async () =>
				{
					string path = item is ShortcutItem shortcut ? shortcut.TargetPath : item.ItemPath;
					await MainPageViewModel.AddNewTabByPathAsync(typeof(PaneHolderPage), path);
				}, DispatcherQueuePriority.Low);
			}
		}

		private static IImmutableList<ListedItem> GetItems() => ImmutableList<ListedItem>.Empty;
	}
}
