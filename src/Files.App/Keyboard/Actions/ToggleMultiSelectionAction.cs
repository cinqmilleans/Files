using Files.App.DataModels;

namespace Files.App.Keyboard.Actions
{
	internal class ToggleMultiSelectionAction : KeyboardAction
	{
		private readonly AppModel model;

		public override string Code => "ToggleMultiSelection";

		public override string Label => "MultiSelection";

		public override ShortKey DefaultShortKey => "Ctrl+B";

		public ToggleMultiSelectionAction(AppModel model) => this.model = model;

		public override void Execute() => model.MultiselectEnabled = !model.MultiselectEnabled;
	}
}
