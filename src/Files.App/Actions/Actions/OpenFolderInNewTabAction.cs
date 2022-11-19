using CommunityToolkit.WinUI;
using Files.App.Extensions;
using Files.App.Filesystem;
using Files.App.ViewModels;
using Files.App.Views;
using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.App.Actions
{
	public class OpenFolderInNewTabAction : IAction
	{
		private readonly IPaneHolder paneHolder;

		public ActionCodes Code => ActionCodes.OpenFolderInNewTab;

		public string Label => "BaseLayoutItemContextFlyoutOpenInNewTab/Text".GetLocalizedResource();

		public string Glyph => "\uF113";
		public string GlyphFamily => "CustomGlyph";

		public OpenFolderInNewTabAction(IPaneHolder paneHolder) => this.paneHolder = paneHolder;

		public bool CanExecute()
		{
			var items = GetItems();
			return items is not null && items.Count < 5 && items.All(i => i.PrimaryItemAttribute is StorageItemTypes.Folder);
		}
		public async Task ExecuteAsync()
		{
			var items = GetItems();
			if (items is null)
				return;

			foreach (var item in items)
			{
				await App.Window.DispatcherQueue.EnqueueAsync(async () =>
				{
					string path = item is ShortcutItem shortcut ? shortcut.TargetPath : item.ItemPath;
					await MainPageViewModel.AddNewTabByPathAsync(typeof(PaneHolderPage), path);
				}, DispatcherQueuePriority.Low);
			}
		}

		private IList<ListedItem>? GetItems() => paneHolder.ActivePane.SlimContentPage.SelectedItems;
	}
}
