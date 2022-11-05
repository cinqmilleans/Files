using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutGridMediumAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override string Code => "ToggleLayoutGridMedium";

		public override string Label => "MediumIcons".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+4";

		public ToggleLayoutGridMediumAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewMedium(true);
	}
}
