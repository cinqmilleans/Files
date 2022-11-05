using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard
{
	internal class ToggleLayoutTilesAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override string Code => "ToggleLayoutTiles";

		public override string Label => "Tiles".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+2";

		public ToggleLayoutTilesAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeTiles(true);
	}
}
