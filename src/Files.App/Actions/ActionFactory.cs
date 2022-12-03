using Files.App.Actions.Action;
using Files.App.ViewModels;
using System;

namespace Files.App.Actions
{
	public class ActionFactory : IActionFactory
	{
		private readonly SidebarViewModel viewModel;

		public ActionFactory(SidebarViewModel viewModel) => this.viewModel = viewModel;

		public IAction CreateAction(ActionCodes code) => code switch
		{
			ActionCodes.None => new NoneAction(),
			ActionCodes.Help => new HelpAction(),
			ActionCodes.ToggleFullScreen => new ToggleFullScreenAction(),
			ActionCodes.OpenFolderInNewTab => new OpenFolderInNewTabAction(),
			_ => throw new ArgumentOutOfRangeException(nameof(code)),
		};
	}
}
