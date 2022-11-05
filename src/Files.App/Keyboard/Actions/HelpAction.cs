using System;
using Windows.System;

namespace Files.App.Keyboard.Actions
{
	internal class HelpAction : KeyboardAction
	{
		public override KeyboardActionCodes Code => KeyboardActionCodes.Help;

		public override string Label => "Aide";
		public override string Description => "Ouvre l'aide dans le navigateur web.";

		public override ShortKey DefaultShortKey => "F1";

		public override async void Execute()
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}
}
