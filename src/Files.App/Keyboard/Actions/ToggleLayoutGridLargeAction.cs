using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutGridLargeAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.ToggleLayoutGridLarge;

		public override string Label => "LargeIcons".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+5";

		public ToggleLayoutGridLargeAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewLarge(true);
	}
}
