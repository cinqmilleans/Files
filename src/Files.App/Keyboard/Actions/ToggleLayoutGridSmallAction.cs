using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutGridSmallAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override string Code => "ToggleLayoutGridSmall";

		public override string Label => "SmallIcons".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+3";

		public ToggleLayoutGridSmallAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewSmall(true);
	}
}
