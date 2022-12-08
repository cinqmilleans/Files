using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutDetailsAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutDetails;
		public string Label => "Details".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE179");
		public HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeDetailsView(true);
		}
	}
}
