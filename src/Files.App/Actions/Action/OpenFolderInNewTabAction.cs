using CommunityToolkit.WinUI;
using Files.App.Extensions;
using Files.App.Filesystem;
using Files.App.ViewModels;
using Files.App.Views;
using Microsoft.UI.Dispatching;
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

		public override bool CanExecute(IActionContext context)
		{
			return context.Items.Count < 5
				&& context.Items.All(i => i.PrimaryItemAttribute is StorageItemTypes.Folder);
		}

		public override async Task ExecuteAsync(IActionContext context)
		{
			foreach (var item in context.Items)
			{
				await App.Window.DispatcherQueue.EnqueueAsync(async () =>
				{
					string path = item is ShortcutItem shortcut ? shortcut.TargetPath : item.ItemPath;
					await MainPageViewModel.AddNewTabByPathAsync(typeof(PaneHolderPage), path);
				}, DispatcherQueuePriority.Low);
			}
		}
	}
}
