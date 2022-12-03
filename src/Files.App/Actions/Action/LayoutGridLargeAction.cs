using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutGridLargeAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutGridLarge;
		public override string Label => "LargeIcons".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number5, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewLarge(true);
	}
}
