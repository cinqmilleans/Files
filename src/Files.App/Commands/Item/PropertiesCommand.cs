using Files.App.CommandManager;
using Files.App.Extensions;
using Files.App.Helpers;
using System.Threading.Tasks;

namespace Files.App.Commands
{
	internal class PropertiesCommand : ObservableCommand
	{
		public override CommandCodes Code => CommandCodes.Properties;
		public override string Label => "BaseLayoutItemContextFlyoutProperties/Text".GetLocalizedResource();

		public override IGlyph Glyph { get; } = new Glyph("\uF031", "\uF032");

		public PropertiesCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			var flyout = Context.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				if (flyout.IsOpen)
					flyout.Closed += OpenProperties;
				else
					FilePropertiesHelpers.ShowProperties(Context.ShellPage!);
			}
			return Task.CompletedTask;
		}

		private void OpenProperties(object? sender, object e)
		{
			var flyout = Context.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				flyout.Closed -= OpenProperties;
				FilePropertiesHelpers.ShowProperties(Context.ShellPage!);
			}
		}
	}
}
