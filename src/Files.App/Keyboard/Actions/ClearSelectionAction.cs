using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class ClearSelectionAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override string Code => "ClearSelection";

		public override string Label => "ClearSelection";

		public override ShortKey DefaultShortKey => ShortKey.None;

		public ClearSelectionAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
		{
			var pane = viewModel.PaneHolder?.ActivePaneOrColumn;

			bool isEditing = pane?.ToolbarViewModel?.IsEditModeEnabled ?? true;
			bool isRenaming = pane?.SlimContentPage?.IsRenamingItem ?? true;

			if (!isEditing && !isRenaming)
				pane?.SlimContentPage?.ItemManipulationModel?.ClearSelection();
		}
	}
}
