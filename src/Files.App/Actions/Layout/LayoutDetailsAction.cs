using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.DataModels;
using Files.App.Extensions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutDetailsAction : IAction
	{
		public ActionCodes Code => ActionCodes.LayoutDetails;
		public string Label => "Details".GetLocalizedResource();

		public HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private void Execute()
		{
			var context = Ioc.Default.GetService<IActionContext>();
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeDetailsView(true);
		}
	}
}
