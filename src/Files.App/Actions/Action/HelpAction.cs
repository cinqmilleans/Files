using Files.App.Actions.HotKeys;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions.Action
{
	internal class HelpAction : AsyncAction
	{
		public override ActionCodes Code => ActionCodes.Help;
		public override string Label => "Help";

		public override HotKey HotKey => new(VirtualKey.F1);

		public async override Task ExecuteAsync(IActionContext _)
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}
}
