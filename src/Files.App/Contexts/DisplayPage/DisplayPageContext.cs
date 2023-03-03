using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.ViewModels;
using Files.App.Views;
using Files.Backend.Services.Settings;
using Files.Shared.Enums;
using System.ComponentModel;

namespace Files.App.Contexts
{
	internal class DisplayPageContext : ObservableObject, IDisplayPageContext
	{
		private static readonly IFoldersSettingsService settings = Ioc.Default.GetRequiredService<IFoldersSettingsService>();

		private IContext context = new NoneContext();
		private FolderSettingsViewModel? viewModel;

		private LayoutTypes layoutType = LayoutTypes.None;
		public LayoutTypes LayoutType
		{
			get => layoutType;
			set => context.LayoutType = value;
		}

		private SortOption sortOption = SortOption.Name;
		public SortOption SortOption
		{
			get => sortOption;
			set => context.SortOption = value;
		}

		private SortDirection sortDirection = SortDirection.Ascending;
		public SortDirection SortDirection
		{
			get => sortDirection;
			set => context.SortDirection = value;
		}

		private GroupOption groupOption = GroupOption.None;
		public GroupOption GroupOption
		{
			get => groupOption;
			set => context.GroupOption = value;
		}

		private SortDirection groupDirection = SortDirection.Ascending;
		public SortDirection GroupDirection
		{
			get => groupDirection;
			set => context.GroupDirection = value;
		}

		public DisplayPageContext()
		{
			BaseShellPage.CurrentInstanceChanged += BaseShellPage_CurrentInstanceChanged;
			settings.PropertyChanged += Settings_PropertyChanged;
		}

		private void BaseShellPage_CurrentInstanceChanged(object? sender, BaseShellPage shellPage)
		{
			var newViewModel = shellPage?.FolderSettings;
			if (viewModel == newViewModel)
				return;

			if (viewModel is not null)
				viewModel.PropertyChanged -= ViewModel_PropertyChanged;

			viewModel = newViewModel;

			if (viewModel is not null)
				viewModel.PropertyChanged += ViewModel_PropertyChanged;

			context = viewModel is null ? new NoneContext() : new PageContext(viewModel);

			SetProperty(ref layoutType, context.LayoutType, nameof(LayoutType));
			SetProperty(ref sortOption, context.SortOption, nameof(SortOption));
			SetProperty(ref sortDirection, context.SortDirection, nameof(SortDirection));
			SetProperty(ref groupOption, context.GroupOption, nameof(GroupOption));
			SetProperty(ref groupDirection, context.GroupDirection, nameof(GroupDirection));
		}

		private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IFoldersSettingsService.SyncFolderPreferencesAcrossDirectories))
				SetProperty(ref layoutType, context.LayoutType, nameof(LayoutType));
		}

		private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(FolderSettingsViewModel.LayoutMode):
				case nameof(FolderSettingsViewModel.GridViewSizeKind):
				case nameof(FolderSettingsViewModel.IsLayoutModeFixed):
				case nameof(FolderSettingsViewModel.IsAdaptiveLayoutEnabled):
					SetProperty(ref layoutType, context.LayoutType, nameof(LayoutType));
					break;
				case nameof(FolderSettingsViewModel.DirectorySortOption):
					SetProperty(ref sortOption, context.SortOption, nameof(SortOption));
					break;
				case nameof(FolderSettingsViewModel.DirectorySortDirection):
					SetProperty(ref sortDirection, context.SortDirection, nameof(SortDirection));
					break;
				case nameof(FolderSettingsViewModel.DirectoryGroupOption):
					SetProperty(ref groupOption, context.GroupOption, nameof(GroupOption));
					break;
				case nameof(FolderSettingsViewModel.DirectoryGroupDirection):
					SetProperty(ref groupDirection, context.GroupDirection, nameof(GroupDirection));
					break;
			}
		}

		private interface IContext
		{
			LayoutTypes LayoutType { get; set; }

			SortOption SortOption { get; set; }
			SortDirection SortDirection { get; set; }

			GroupOption GroupOption { get; set; }
			SortDirection GroupDirection { get; set; }
		}

		private class NoneContext : IContext
		{
			public LayoutTypes LayoutType
			{
				get => LayoutTypes.None;
				set { }
			}
			public SortOption SortOption
			{
				get => SortOption.Name;
				set { }
			}
			public SortDirection SortDirection
			{
				get => SortDirection.Ascending;
				set { }
			}
			public GroupOption GroupOption
			{
				get => GroupOption.None;
				set { }
			}
			public SortDirection GroupDirection
			{
				get => SortDirection.Ascending;
				set { }
			}
		}

		private class PageContext : IContext
		{
			private readonly FolderSettingsViewModel viewModel;

			public LayoutTypes LayoutType
			{
				get
				{
					bool isAdaptive = viewModel.IsAdaptiveLayoutEnabled
						&& !viewModel.IsLayoutModeFixed
						&& settings.SyncFolderPreferencesAcrossDirectories;

					if (isAdaptive)
						return LayoutTypes.Adaptive;

					return viewModel.LayoutMode switch
					{
						FolderLayoutModes.DetailsView => LayoutTypes.Details,
						FolderLayoutModes.TilesView => LayoutTypes.Tiles,
						FolderLayoutModes.GridView => viewModel.GridViewSizeKind switch
						{
							GridViewSizeKind.Small => LayoutTypes.GridSmall,
							GridViewSizeKind.Medium => LayoutTypes.GridMedium,
							GridViewSizeKind.Large => LayoutTypes.GridLarge,
							_ => throw new InvalidEnumArgumentException(),
						},
						FolderLayoutModes.ColumnView => LayoutTypes.Columns,
						_ => throw new InvalidEnumArgumentException(),
					};
				}
				set
				{
					switch (value)
					{
						case LayoutTypes.Details:
							viewModel.ToggleLayoutModeDetailsView(true);
							break;
						case LayoutTypes.Tiles:
							viewModel.ToggleLayoutModeTiles(true);
							break;
						case LayoutTypes.GridSmall:
							viewModel.ToggleLayoutModeGridViewSmall(true);
							break;
						case LayoutTypes.GridMedium:
							viewModel.ToggleLayoutModeGridViewMedium(true);
							break;
						case LayoutTypes.GridLarge:
							viewModel.ToggleLayoutModeGridViewLarge(true);
							break;
						case LayoutTypes.Columns:
							viewModel.ToggleLayoutModeColumnView(true);
							break;
						case LayoutTypes.Adaptive:
							viewModel.ToggleLayoutModeAdaptive();
							break;
					}
				}
			}

			public SortOption SortOption
			{
				get => viewModel.DirectorySortOption;
				set => viewModel.DirectorySortOption = value;
			}
			public SortDirection SortDirection
			{
				get => viewModel.DirectorySortDirection;
				set => viewModel.DirectorySortDirection = value;
			}
			public GroupOption GroupOption
			{
				get => viewModel.DirectoryGroupOption;
				set => viewModel.DirectoryGroupOption = value;
			}
			public SortDirection GroupDirection
			{
				get => viewModel.DirectoryGroupDirection;
				set => viewModel.DirectoryGroupDirection = value;
			}

			public PageContext(FolderSettingsViewModel viewModel) => this.viewModel = viewModel;
		}
	}
}
