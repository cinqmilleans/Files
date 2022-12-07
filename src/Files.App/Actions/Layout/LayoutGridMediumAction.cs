using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutGridMediumAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutGridMedium;
		public string Label => "MediumIcons".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uF0E2");
		public HotKey HotKey => new(VirtualKey.Number4, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeGridViewMedium(true);
		}
	}
}
