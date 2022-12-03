using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutGridSmallAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutGridSmall;
		public override string Label => "SmallIcons".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number3, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewSmall(true);
	}
}
