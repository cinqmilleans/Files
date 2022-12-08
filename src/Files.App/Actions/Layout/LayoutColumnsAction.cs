using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutColumnsAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutColumns;
		public string Label => "Columns".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uF115") { Family = "CustomGlyph" };
		public HotKey HotKey => new(VirtualKey.Number6, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeColumnView(true);
		}
	}
}
