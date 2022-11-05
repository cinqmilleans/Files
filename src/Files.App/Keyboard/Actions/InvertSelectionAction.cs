using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class InvertSelectionAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.InvertSelection;

		public override string Label => "InvertSelection";

		public override ShortKey DefaultShortKey => ShortKey.None;

		public InvertSelectionAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
		{
			var pane = viewModel.PaneHolder?.ActivePaneOrColumn;

			bool isEditing = pane?.ToolbarViewModel?.IsEditModeEnabled ?? true;
			bool isRenaming = pane?.SlimContentPage?.IsRenamingItem ?? true;

			if (!isEditing && !isRenaming)
				pane?.SlimContentPage?.ItemManipulationModel?.InvertSelection();
		}
	}
}
