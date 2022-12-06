using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.DataModels.HotKeys;
using Files.App.Extensions;
using Files.App.ViewModels.ActionContexts;
using Files.App.ViewModels.Actions;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.CommandActions.Layout
{
    internal class LayoutDetailsAction : IAction
    {
        public CommandCodes Code => CommandCodes.LayoutDetails;
        public string Label => "Details".GetLocalizedResource();

        public HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

        public Task ExecuteAsync()
        {
            Execute();
            return Task.CompletedTask;
        }

        private static void Execute()
        {
            var context = Ioc.Default.GetService<IActionContext>();
            var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
            settings?.ToggleLayoutModeDetailsView(true);
        }
    }
}
