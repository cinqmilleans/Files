using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutGridMediumAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutGridMedium;
		public override string Label => "MediumIcons".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number4, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeGridViewMedium(true);
	}
}
