using Files.App.DataModels.HotKeys;
using Files.App.ViewModels.Actions;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.CommandActions.Global
{
    internal class HelpAction : IAction
    {
        public CommandCodes Code => CommandCodes.Help;

        public string Label => "Help";

        public HotKey HotKey => new(VirtualKey.F1);

        public async Task ExecuteAsync()
        {
            var url = new Uri(Constants.GitHub.DocumentationUrl);
            await Launcher.LaunchUriAsync(url);
        }
    }
}
