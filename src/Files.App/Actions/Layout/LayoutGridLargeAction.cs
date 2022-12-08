using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutGridLargeAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutGridLarge;
		public string Label => "LargeIcons".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE739");
		public HotKey HotKey => new(VirtualKey.Number5, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeGridViewLarge(true);
		}
	}
}
