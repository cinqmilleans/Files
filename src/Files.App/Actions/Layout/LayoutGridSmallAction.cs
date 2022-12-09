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
	internal class LayoutGridSmallAction : ObservableObject, IToggleAction
	{
		private readonly ICommandContext? context = Ioc.Default.GetService<ICommandContext>();

		public CommandCodes Code => CommandCodes.LayoutGridSmall;
		public string Label => "SmallIcons".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE80A");
		public HotKey HotKey => new(VirtualKey.Number3, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public bool IsOn => context?.ShellPage?.ToolbarViewModel?.IsLayoutGridViewSmall ?? false;

		public LayoutGridSmallAction()
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
			settings?.ToggleLayoutModeGridViewSmall(true);
		}

		private void ToolbarViewModel_PropertyChanging(object? _, PropertyChangingEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutGridViewSmall))
				OnPropertyChanging(nameof(IsOn));
		}
		private void ToolbarViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutGridViewSmall))
				OnPropertyChanged(nameof(IsOn));
		}
	}
}
