using Files.App.CommandManager;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Commands
{
	internal class HelpCommand : RichCommand
	{
		public override CommandCodes Code => CommandCodes.Help;

		public override string Label => "Help";
		public override HotKey HotKey => new(VirtualKey.F1);

		public HelpCommand(ICommandContext context) : base(context) {}

		protected override async Task ExecuteAsync()
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}
}
