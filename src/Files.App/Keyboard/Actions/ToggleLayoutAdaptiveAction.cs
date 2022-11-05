using Files.App.Extensions;
using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleLayoutAdaptiveAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override string Code => "ToggleLayoutAdaptive";

		public override string Label => "Adaptive".GetLocalizedResource();

		public override ShortKey DefaultShortKey => "Ctrl+Shift+7";

		public ToggleLayoutAdaptiveAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
			=> viewModel.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeAdaptive();
	}
}
