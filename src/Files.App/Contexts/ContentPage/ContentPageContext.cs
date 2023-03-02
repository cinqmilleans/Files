﻿using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.Filesystem;
using Files.App.ViewModels;
using Files.App.Views;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace Files.App.Contexts
{
	internal class ContentPageContext : ObservableObject, IContentPageContext
	{
		private static readonly IReadOnlyList<ListedItem> EmptyListedItemList = Enumerable.Empty<ListedItem>().ToImmutableList();

		private ItemViewModel? filesystemViewModel;

		private BaseShellPage? shellPage;
		public IShellPage? ShellPage => shellPage;

		private ContentPageTypes pageType = ContentPageTypes.None;
		public ContentPageTypes PageType => pageType;

		private ContentLayoutTypes layoutType = ContentLayoutTypes.None;
		public ContentLayoutTypes LayoutType
		{
			get => layoutType;
			set => SetLayout(value);
		}

		public ListedItem? Folder => shellPage?.FilesystemViewModel?.CurrentFolder;

		public bool HasItem => shellPage?.ToolbarViewModel?.HasItem ?? false;

		public bool HasSelection => SelectedItems.Count is not 0;
		public ListedItem? SelectedItem => SelectedItems.Count is 1 ? SelectedItems[0] : null;

		private IReadOnlyList<ListedItem> selectedItems = EmptyListedItemList;
		public IReadOnlyList<ListedItem> SelectedItems => selectedItems;

		public ContentPageContext()
		{
			BaseShellPage.CurrentInstanceChanged += BaseShellPage_CurrentInstanceChanged;
		}

		private void UpdateShellPage(BaseShellPage? newShellPage)
		{
			if (shellPage == newShellPage)
				return;

			if (shellPage is not null)
			{
				shellPage.PropertyChanged -= ShellPage_PropertyChanged;
				shellPage.InstanceViewModel.PropertyChanged -= InstanceViewModel_PropertyChanged;
				shellPage.ToolbarViewModel.PropertyChanged -= ToolbarViewModel_PropertyChanged;

				if (filesystemViewModel is not null)
				{
					filesystemViewModel.PropertyChanged -= FilesystemViewModel_PropertyChanged;
					filesystemViewModel = null;
				}
			}

			shellPage = newShellPage;

			if (shellPage is not null)
			{
				shellPage.PropertyChanged += ShellPage_PropertyChanged;
				shellPage.InstanceViewModel.PropertyChanged += InstanceViewModel_PropertyChanged;
				shellPage.ToolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;

				if (shellPage.FilesystemViewModel is not null)
				{
					filesystemViewModel = shellPage.FilesystemViewModel;
					filesystemViewModel.PropertyChanged += FilesystemViewModel_PropertyChanged;
				}
			}

			UpdatePageType();
			UpdateLayoutType();
			UpdateSelectedItems();

			OnPropertyChanged(nameof(HasItem));
			OnPropertyChanged(nameof(Folder));
			OnPropertyChanged(nameof(ShellPage));
		}

		private void UpdatePageType()
		{
			var type = shellPage?.InstanceViewModel switch
			{
				null => ContentPageTypes.None,
				{ IsPageTypeNotHome: false } => ContentPageTypes.Home,
				{ IsPageTypeRecycleBin: true } => ContentPageTypes.RecycleBin,
				{ IsPageTypeZipFolder: true } => ContentPageTypes.ZipFolder,
				{ IsPageTypeFtp: true } => ContentPageTypes.Ftp,
				{ IsPageTypeLibrary: true } => ContentPageTypes.Library,
				{ IsPageTypeCloudDrive: true } => ContentPageTypes.CloudDrive,
				{ IsPageTypeMtpDevice: true } => ContentPageTypes.MtpDevice,
				{ IsPageTypeSearchResults: true } => ContentPageTypes.SearchResults,
				_ => ContentPageTypes.Folder,
			};
			SetProperty(ref pageType, type, nameof(PageType));
		}

		private void UpdateLayoutType()
		{
			var type = shellPage?.ToolbarViewModel switch
			{
				null => ContentLayoutTypes.None,
				{ IsLayoutDetailsView: true } => ContentLayoutTypes.Details,
				{ IsLayoutTilesView: true } => ContentLayoutTypes.Tiles,
				{ IsLayoutGridViewSmall: true } => ContentLayoutTypes.GridSmall,
				{ IsLayoutGridViewMedium: true } => ContentLayoutTypes.GridMedium,
				{ IsLayoutGridViewLarge: true } => ContentLayoutTypes.GridLarge,
				{ IsLayoutColumnsView: true } => ContentLayoutTypes.Columns,
				{ IsLayoutAdaptive: true } => ContentLayoutTypes.Adaptive,
				_ => ContentLayoutTypes.None,
			};
			SetProperty(ref layoutType, type, nameof(LayoutType));
		}

		private void UpdateSelectedItems()
		{
			bool oldHasSelection = HasSelection;
			ListedItem? oldSelectedItem = SelectedItem;

			IReadOnlyList<ListedItem> items = shellPage?.ToolbarViewModel?.SelectedItems?.AsReadOnly() ?? EmptyListedItemList;
			if (SetProperty(ref selectedItems, items, nameof(SelectedItems)))
			{
				if (HasSelection != oldHasSelection)
					OnPropertyChanged(nameof(HasSelection));
				if (SelectedItem != oldSelectedItem)
					OnPropertyChanged(nameof(SelectedItem));
			}
		}

		protected void SetLayout(ContentLayoutTypes layoutType)
		{
			var viewModel = shellPage?.InstanceViewModel?.FolderSettings;
			if (viewModel is null)
				return;

			switch (layoutType)
			{
				case ContentLayoutTypes.Details:
					viewModel.ToggleLayoutModeDetailsView(true);
					break;
				case ContentLayoutTypes.Tiles:
					viewModel.ToggleLayoutModeTiles(true);
					break;
				case ContentLayoutTypes.GridSmall:
					viewModel.ToggleLayoutModeGridViewSmall(true);
					break;
				case ContentLayoutTypes.GridMedium:
					viewModel.ToggleLayoutModeGridViewMedium(true);
					break;
				case ContentLayoutTypes.GridLarge:
					viewModel.ToggleLayoutModeGridViewLarge(true);
					break;
				case ContentLayoutTypes.Columns:
					viewModel.ToggleLayoutModeColumnView(true);
					break;
				case ContentLayoutTypes.Adaptive:
					viewModel.ToggleLayoutModeAdaptive();
					break;
			}
		}

		private void BaseShellPage_CurrentInstanceChanged(object? sender, BaseShellPage newShellPage)
		{
			UpdateShellPage(newShellPage);
		}

		private void ShellPage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(IShellPage.IsCurrentInstance):
					if (shellPage is BaseShellPage { IsCurrentInstance: false })
						UpdateShellPage(null);
					break;
				case nameof(IShellPage.FilesystemViewModel):
					if (filesystemViewModel is not null)
						filesystemViewModel.PropertyChanged -= FilesystemViewModel_PropertyChanged;
					filesystemViewModel = shellPage?.FilesystemViewModel;
					if (filesystemViewModel is not null)
						filesystemViewModel.PropertyChanged += FilesystemViewModel_PropertyChanged;
					OnPropertyChanged(nameof(Folder));
					break;
			}
		}

		private void InstanceViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(CurrentInstanceViewModel.IsPageTypeNotHome):
				case nameof(CurrentInstanceViewModel.IsPageTypeRecycleBin):
				case nameof(CurrentInstanceViewModel.IsPageTypeZipFolder):
				case nameof(CurrentInstanceViewModel.IsPageTypeFtp):
				case nameof(CurrentInstanceViewModel.IsPageTypeLibrary):
				case nameof(CurrentInstanceViewModel.IsPageTypeCloudDrive):
				case nameof(CurrentInstanceViewModel.IsPageTypeMtpDevice):
				case nameof(CurrentInstanceViewModel.IsPageTypeSearchResults):
					UpdatePageType();
					break;
			}
		}

		private void ToolbarViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ToolbarViewModel.HasItem):
					OnPropertyChanged(e.PropertyName);
					break;
				case nameof(ToolbarViewModel.SelectedItems):
					UpdateSelectedItems();
					break;
				case nameof(ToolbarViewModel.IsLayoutDetailsView):
				case nameof(ToolbarViewModel.IsLayoutTilesView):
				case nameof(ToolbarViewModel.IsLayoutGridViewSmall):
				case nameof(ToolbarViewModel.IsLayoutGridViewMedium):
				case nameof(ToolbarViewModel.IsLayoutGridViewLarge):
				case nameof(ToolbarViewModel.IsLayoutColumnsView):
				case nameof(ToolbarViewModel.IsLayoutAdaptive):
					UpdateLayoutType();
					break;
			}
		}

		private void FilesystemViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ItemViewModel.CurrentFolder))
				OnPropertyChanged(nameof(Folder));
		}
	}
}
