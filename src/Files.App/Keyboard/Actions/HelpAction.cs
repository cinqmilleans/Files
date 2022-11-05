using System;
using Windows.System;

namespace Files.App.Keyboard
{
	internal class HelpAction : KeyboardAction
	{
		public override KeyboardActionCodes Code => KeyboardActionCodes.Help;

		public override string Label => "Help";
		public override string Description => "Opens the help in the web browser.";

		public override ShortKey DefaultShortKey => "F1";

		public override async void Execute()
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}
}
