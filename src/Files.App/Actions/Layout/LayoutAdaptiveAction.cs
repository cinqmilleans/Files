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
	internal class LayoutAdaptiveAction : ObservableObject, IToggleAction
	{
		private readonly ICommandContext? context = Ioc.Default.GetService<ICommandContext>();

		public CommandCodes Code => CommandCodes.LayoutAdaptive;
		public string Label => "Adaptive".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uF576");
		public HotKey HotKey => new(VirtualKey.Number7, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public bool IsOn => context?.ShellPage?.ToolbarViewModel?.IsLayoutAdaptive ?? false;
		public bool IsExecutable => context?.ShellPage?.ToolbarViewModel?.IsAdaptiveLayoutEnabled ?? false;

		public LayoutAdaptiveAction()
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
			settings?.ToggleLayoutModeAdaptive();
		}

		private void ToolbarViewModel_PropertyChanging(object? _, PropertyChangingEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ToolbarViewModel.IsLayoutAdaptive):
					OnPropertyChanging(nameof(IsOn));
					break;
				case nameof(ToolbarViewModel.IsAdaptiveLayoutEnabled):
					OnPropertyChanging(nameof(IsExecutable));
					break;
			}
		}
		private void ToolbarViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ToolbarViewModel.IsLayoutAdaptive):
					OnPropertyChanged(nameof(IsOn));
					break;
				case nameof(ToolbarViewModel.IsAdaptiveLayoutEnabled):
					OnPropertyChanged(nameof(IsExecutable));
					break;
			}
		}
	}
}
