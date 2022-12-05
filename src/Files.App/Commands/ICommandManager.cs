namespace Files.App.Commands
{
	public interface ICommandManager
	{
		IRichCommand NoneCommand { get; }
		IRichCommand HelpCommand { get; }
		IToggleObservableCommand FullScreenCommand { get; }
		IRichCommand LayoutDetailsCommand { get; }
		IObservableCommand PropertiesCommand { get; }
	}
}
