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
	internal class LayoutDetailsAction : ObservableObject, IToggleAction
	{
		private readonly ICommandContext? context = Ioc.Default.GetService<ICommandContext>();

		public CommandCodes Code => CommandCodes.LayoutDetails;
		public string Label => "Details".GetLocalizedResource();

		public IGlyph Glyph { get; } = new Glyph("\uE179");
		public HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public bool IsOn => context?.ToolbarViewModel?.IsLayoutDetailsView ?? false;

		public LayoutDetailsAction()
		{
			if (context is null)
				return;

			context.PropertyChanging += Context_PropertyChanging;
			context.PropertyChanged += Context_PropertyChanged;
		}

		public Task ExecuteAsync()
		{
			Execute();
			return Task.CompletedTask;
		}

		private void Execute()
		{
			var settings = context?.ShellPage?.InstanceViewModel?.FolderSettings;
			settings?.ToggleLayoutModeDetailsView(true);
		}

		private void Context_PropertyChanging(object? _, PropertyChangingEventArgs e)
		{
			if (e.PropertyName is nameof(ICommandContext.ToolbarViewModel) && context?.ToolbarViewModel is not null)
			{
				context.ToolbarViewModel.PropertyChanging -= ToolbarViewModel_PropertyChanging;
				context.ToolbarViewModel.PropertyChanged -= ToolbarViewModel_PropertyChanged;
				OnPropertyChanging(nameof(IsOn));
			}
		}
		private void Context_PropertyChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ICommandContext.ToolbarViewModel) && context?.ToolbarViewModel is not null)
			{
				context.ToolbarViewModel.PropertyChanging += ToolbarViewModel_PropertyChanging;
				context.ToolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;
				OnPropertyChanged(nameof(IsOn));
			}
		}

		private void ToolbarViewModel_PropertyChanging(object? _, PropertyChangingEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutDetailsView))
				OnPropertyChanging(nameof(IsOn));
		}
		private void ToolbarViewModel_PropertyChanged(object? _, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ToolbarViewModel.IsLayoutDetailsView))
				OnPropertyChanged(nameof(IsOn));
		}
	}
}
