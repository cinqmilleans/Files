namespace Files.App.Actions
{
	public interface IActionFactory
	{
		IAction CreateAction(ActionCodes code);
	}
}
