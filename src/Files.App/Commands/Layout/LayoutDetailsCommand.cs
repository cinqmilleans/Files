using Files.App.CommandManager;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Commands
{
	internal class LayoutDetailsCommand : RichCommand
	{
		public override CommandCodes Code => CommandCodes.LayoutDetails;
		public override string Label => "Details".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public LayoutDetailsCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			Context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeDetailsView(true);
			return Task.CompletedTask;
		}
	}
}
