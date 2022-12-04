namespace Files.App.CommandManager
{
	public interface IToggleCommand : IObservableCommand
	{
		bool IsOn { get; set; }
	}
}
