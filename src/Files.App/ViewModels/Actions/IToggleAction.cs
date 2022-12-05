namespace Files.App.ViewModels.Actions
{
	public interface IToggleAction : IAction
	{
		bool IsOn { get; set; }
	}
}
