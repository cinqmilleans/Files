using Files.App.ViewModels.Actions;
using System.Threading.Tasks;

namespace Files.App.CommandActions.Global
{
    internal class NoneAction : IAction
    {
        public CommandCodes Code => CommandCodes.None;
        public string Label => string.Empty;

        public Task ExecuteAsync() => Task.CompletedTask;
    }
}
