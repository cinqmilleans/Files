using Files.App.Actions.Action;
using System;

namespace Files.App.Actions
{
	public class ActionFactory : IActionFactory
	{
		public IAction CreateAction(ActionCodes code) => code switch
		{
			ActionCodes.None => new NoneAction(),
			ActionCodes.Help => new HelpAction(),
			ActionCodes.FullScreen => new FullScreenAction(),
			ActionCodes.LayoutDetails => new LayoutDetailsAction(),
			ActionCodes.LayoutTiles => new LayoutTilesAction(),
			ActionCodes.LayoutGridSmall => new LayoutGridSmallAction(),
			ActionCodes.LayoutGridMedium => new LayoutGridMediumAction(),
			ActionCodes.LayoutGridLarge => new LayoutGridLargeAction(),
			ActionCodes.LayoutColumns => new LayoutColumnsAction(),
			ActionCodes.LayoutAdaptive => new LayoutAdaptiveAction(),
			ActionCodes.OpenFolderInNewTab => new OpenFolderInNewTabAction(),
			_ => throw new ArgumentOutOfRangeException(nameof(code)),
		};
	}
}
