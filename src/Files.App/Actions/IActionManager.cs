namespace Files.App.Actions
{
	public interface IActionManager
	{
		event ActionEventHandler? ActionEvent;

		void Execute(Actions action);
	}
}
