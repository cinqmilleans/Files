using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class LayoutAdaptiveAction : SyncAction
	{
		public override ActionCodes Code => ActionCodes.LayoutAdaptive;
		public override string Label => "Adaptive".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number7, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public override void Execute(IActionContext context)
			=> context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeAdaptive();
	}
}
