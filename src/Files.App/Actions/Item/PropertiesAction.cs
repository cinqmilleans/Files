using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.DataModels;
using Files.App.Extensions;
using Files.App.Helpers;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	internal class PropertiesAction : ObservableObject, IObservableAction
	{
		private readonly IActionContext? context = Ioc.Default.GetService<IActionContext>();

		public ActionCodes Code => ActionCodes.Properties;
		public string Label => "BaseLayoutItemContextFlyoutProperties/Text".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uF031", "\uF032");

		public bool IsExecutable => true;

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private void Execute()
		{
			var flyout = context?.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				if (flyout.IsOpen)
					flyout.Closed += OpenProperties;
				else
					FilePropertiesHelpers.ShowProperties(context?.ShellPage!);
			}
		}


		private void OpenProperties(object? sender, object e)
		{
			var flyout = context.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				flyout.Closed -= OpenProperties;
				FilePropertiesHelpers.ShowProperties(context?.ShellPage!);
			}
		}
	}
}
