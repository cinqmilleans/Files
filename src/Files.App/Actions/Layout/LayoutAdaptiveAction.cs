using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutAdaptiveAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutAdaptive;
		public string Label => "Adaptive".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uF576");
		public HotKey HotKey => new(VirtualKey.Number7, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeAdaptive();
		}
	}
}
