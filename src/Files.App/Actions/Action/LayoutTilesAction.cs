using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutTilesAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutTiles;
		public override string Label => "Tiles".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number2, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeTiles(true);
	}
}
