﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutGridSmallAction : IAction
	{
		public CommandCodes Code => CommandCodes.LayoutGridSmall;
		public string Label => "SmallIcons".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE80A");
		public HotKey HotKey => new(VirtualKey.Number3, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private static void Execute()
		{
			var context = Ioc.Default.GetService<ICommandContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeGridViewSmall(true);
		}
	}
}
