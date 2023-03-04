using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.ViewModels;
using Files.Backend.Services.Settings;
using Files.Shared.Enums;
using System.ComponentModel;

namespace Files.App.Contexts
{
	internal class DisplayPageContext : PageContext, IDisplayPageContext
	{
		private static readonly IFoldersSettingsService settings = Ioc.Default.GetRequiredService<IFoldersSettingsService>();

		private bool isLayoutAdaptiveEnabled = false;
		public bool IsLayoutAdaptiveEnabled
		{
			get => isLayoutAdaptiveEnabled;
			set => settings.SyncFolderPreferencesAcrossDirectories = value;
		}

		private LayoutTypes layoutType = LayoutTypes.None;
		public LayoutTypes LayoutType
		{
			get => layoutType;
			set
			{
				var viewModel = Page?.FolderSettings;
				if (viewModel is null)
					return;

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

		private SortOption sortOption = SortOption.Name;
		public SortOption SortOption
		{
			get => sortOption;
			set
			{
				if (Page?.FolderSettings is FolderSettingsViewModel viewModel)
					viewModel.DirectorySortOption = value;
			}
		}

		private SortDirection sortDirection = SortDirection.Ascending;
		public SortDirection SortDirection
		{
			get => sortDirection;
			set
			{
				if (Page?.FolderSettings is FolderSettingsViewModel viewModel)
					viewModel.DirectorySortDirection = value;
			}
		}

		private GroupOption groupOption = GroupOption.None;
		public GroupOption GroupOption
		{
			get => groupOption;
			set
			{
				if (Page?.FolderSettings is FolderSettingsViewModel viewModel)
					viewModel.DirectoryGroupOption = value;
			}
		}

		private SortDirection groupDirection = SortDirection.Ascending;
		public SortDirection GroupDirection
		{
			get => groupDirection;
			set
			{
				if (Page?.FolderSettings is FolderSettingsViewModel viewModel)
					viewModel.DirectoryGroupDirection = value;
			}
		}

		public DisplayPageContext()
		{
			settings.PropertyChanged += Settings_PropertyChanged;
		}

		protected override void OnContentChanged()
		{
			var viewModel = Page?.FolderSettings;
			if (viewModel is null)
			{
				SetProperty(ref layoutType, LayoutTypes.None, nameof(LayoutType));
				SetProperty(ref sortOption, SortOption.Name, nameof(SortOption));
				SetProperty(ref sortDirection, SortDirection.Ascending, nameof(SortDirection));
				SetProperty(ref groupOption, GroupOption.None, nameof(GroupOption));
				SetProperty(ref groupDirection, SortDirection.Ascending, nameof(GroupDirection));
			}
			else
			{
				SetProperty(ref layoutType, GetLayoutType(), nameof(LayoutType));
				SetProperty(ref sortOption, viewModel.DirectorySortOption, nameof(SortOption));
				SetProperty(ref sortDirection, viewModel.DirectorySortDirection, nameof(SortDirection));
				SetProperty(ref groupOption, viewModel.DirectoryGroupOption, nameof(GroupOption));
				SetProperty(ref groupDirection, viewModel.DirectoryGroupDirection, nameof(GroupDirection));
			}
		}

		private void Settings_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(IFoldersSettingsService.SyncFolderPreferencesAcrossDirectories))
			{
				bool isEnabled = settings.SyncFolderPreferencesAcrossDirectories;
				if (SetProperty(ref isLayoutAdaptiveEnabled, isEnabled, nameof(IsLayoutAdaptiveEnabled)))
					SetProperty(ref layoutType, GetLayoutType(), nameof(LayoutType));
			}
		}

		private LayoutTypes GetLayoutType()
		{
			var viewModel = Page?.FolderSettings;
			if (viewModel is null)
				return LayoutTypes.None;

			bool isAdaptive = isLayoutAdaptiveEnabled && viewModel.IsAdaptiveLayoutEnabled && !viewModel.IsLayoutModeFixed;
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
	}
}
