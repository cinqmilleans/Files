using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutColumnsAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutColumns;
		public override string Label => "Columns".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number6, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeColumnView(true);
	}
}
