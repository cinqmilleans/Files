using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutTilesAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutTiles;
		public string Label => "Tiles".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE15C");
		public HotKey HotKey => new(VirtualKey.Number2, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeTiles(true);
		}
	}
}
