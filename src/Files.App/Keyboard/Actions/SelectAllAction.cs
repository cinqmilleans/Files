using Files.App.ViewModels;

namespace Files.App.Keyboard.Actions
{
	internal class SelectAllAction : KeyboardAction
	{
		private readonly SidebarViewModel viewModel;

		public override KeyboardActionCodes Code => KeyboardActionCodes.SelectAll;

		public override string Label => "SelectAll";

		public override ShortKey DefaultShortKey => "Ctrl+B";

		public SelectAllAction(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public override void Execute()
		{
			var pane = viewModel.PaneHolder?.ActivePaneOrColumn;

			bool isEditing = pane?.ToolbarViewModel?.IsEditModeEnabled ?? true;
			bool isRenaming = pane?.SlimContentPage?.IsRenamingItem ?? true;

			if (!isEditing && !isRenaming)
				pane?.SlimContentPage?.ItemManipulationModel?.SelectAllItems();
		}
	}
}
