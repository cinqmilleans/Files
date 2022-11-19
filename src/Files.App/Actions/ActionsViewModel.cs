namespace Files.App.Actions
{
	internal class ActionsViewModel : IActionsViewModel
	{
		public IAction Help { get; } = new HelpAction();

		//public IAction OpenFolderInNewTab { get; } = new OpenFolderInNewTabAction();
	}
}
