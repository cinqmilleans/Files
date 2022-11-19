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
			ActionCodes.Help => new HelpAction(),
			ActionCodes.OpenFolderInNewTab => new OpenFolderInNewTabAction(viewModel.PaneHolder),
			_ => throw new ArgumentOutOfRangeException(nameof(code)),
		};
	}
}
