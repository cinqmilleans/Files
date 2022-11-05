using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutTilesAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.ToggleLayoutTiles;

		public override string Label => "Tiles";

		public override ShortKey DefaultShortKey => "Ctrl+Shift+2";

		public ToggleLayoutTilesAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeTiles(true);
	}
}
