namespace Files.App.Commands
{
	public interface IToggleAction : IAction
	{
		bool IsOn { get; set; }
	}
}
