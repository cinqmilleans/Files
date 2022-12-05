namespace Files.App.ViewModels.Commands
{
	public interface ICommandManager
	{
		IRichCommand NoneCommand { get; }
		IRichCommand HelpCommand { get; }
		IToggleCommand FullScreenCommand { get; }
		IRichCommand LayoutDetailsCommand { get; }
		IRichCommand PropertiesCommand { get; }
	}
}
