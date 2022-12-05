namespace Files.App.Commands
{
	public interface IToggleCommand : IRichCommand
	{
		bool IsOn { get; set; }

		void Toggle();
	}
}
