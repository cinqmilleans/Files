using Files.App.DataModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleMultiSelectionAction : KeyboardAction
	{
		private readonly AppModel model;

		public override KeyboardActionCodes Code => KeyboardActionCodes.ToggleMultiSelection;

		public override string Label => "MultiSelection";

		public override ShortKey DefaultShortKey => ShortKey.None;

		public ToggleMultiSelectionAction(AppModel model) => this.model = model;

		public override void Execute() => model.MultiselectEnabled = !model.MultiselectEnabled;
	}
}
