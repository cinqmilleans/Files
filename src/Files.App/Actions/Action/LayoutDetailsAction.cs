using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutDetailsAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutDetails;
		public override string Label => "Details".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeDetailsView(true);
	}
}
