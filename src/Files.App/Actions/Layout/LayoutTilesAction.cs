using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Commands;
using Files.App.DataModels;
using Files.App.Extensions;
using Files.App.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutTilesAction : ObservableObject, IToggleAction
	{
		private readonly ICommandContext? context = Ioc.Default.GetService<ICommandContext>();

		public CommandCodes Code => CommandCodes.LayoutTiles;
		public string Label => "Tiles".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE15C");
		public HotKey HotKey => new(VirtualKey.Number2, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public bool IsOn => context?.ShellPage?.ToolbarViewModel?.IsLayoutTilesView ?? false;

		public LayoutTilesAction()
		{
			var toolbarViewModel = context?.ShellPage?.ToolbarViewModel;
			if (toolbarViewModel is null)
				return;

			toolbarViewModel.PropertyChanging += ToolbarViewModel_PropertyChanging;
			toolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private void Execute()
		{
			var settings = context?.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeTiles(true);
		}

		private void ToolbarViewModel_PropertyChanging(object? _, PropertyChangingEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutTilesView))
				OnPropertyChanging(nameof(IsOn));
		}
		private void ToolbarViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutTilesView))
				OnPropertyChanged(nameof(IsOn));
		}
	}
}
