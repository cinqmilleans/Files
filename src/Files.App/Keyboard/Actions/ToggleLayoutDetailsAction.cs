using Files.App.ViewModels;

namespace Files.App.Keyboard
{
	internal class ToggleLayoutDetailsAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.ToggleLayoutDetails;

		public override string Label => "Détails";

		public override ShortKey DefaultShortKey => "Ctrl+Shift+1";

		public ToggleLayoutDetailsAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeDetailsView(true);
	}
}
