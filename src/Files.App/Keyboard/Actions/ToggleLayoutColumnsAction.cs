using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutColumnsAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.ToggleLayoutColumns;

		public override string Label => "Columns".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+6";

		public ToggleLayoutColumnsAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeColumnView(true);
	}
}
