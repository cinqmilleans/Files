using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.Filesystem;
using Files.App.UserControls.MultitaskingControl;
using Files.App.ViewModels;
using Files.App.Views;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace Files.App.Contexts
{
	internal class PageContext : ObservableObject, IPageContext
	{
		private static readonly IReadOnlyList<ListedItem> EmptyListedItemList = Enumerable.Empty<ListedItem>().ToImmutableList();

		private IShellPage? shellPage;

		private PageTypes pageType = PageTypes.None;
		public PageTypes PageType => pageType;

		public bool CanCopy => shellPage?.ToolbarViewModel?.CanCopy ?? false;
		public bool CanRefresh => shellPage?.ToolbarViewModel?.CanRefresh ?? false;

		private IReadOnlyList<ListedItem> selectedItems = EmptyListedItemList;
		public IReadOnlyList<ListedItem> SelectedItems => selectedItems;

		public PageContext() => UpdateShellPage();

		private void UpdateShellPage()
		{
			var newShellPage = GetShellPage();

			if (shellPage != newShellPage)
				return;

			if (shellPage is not null)
			{
				shellPage.InstanceViewModel.PropertyChanged -= InstanceViewModel_PropertyChanged;
				shellPage.ToolbarViewModel.PropertyChanged -= ToolbarViewModel_PropertyChanged;
			}

			shellPage = newShellPage;

			if (shellPage is not null)
			{
				shellPage.InstanceViewModel.PropertyChanged += InstanceViewModel_PropertyChanged;
				shellPage.ToolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;
			}

			UpdatePageType();
			UpdateSelectedItems();

			OnPropertyChanged(nameof(CanCopy));
			OnPropertyChanged(nameof(CanRefresh));

			static IShellPage? GetShellPage()
			{
				var tabItem = MainPageViewModel.AppInstances.FirstOrDefault(IsCurrentInstance);
				return (tabItem?.Control?.TabItemContent as PaneHolderPage)?.ActivePane;

				static bool IsCurrentInstance(TabItem instance) => instance.Control.TabItemContent.IsCurrentInstance;
			}
		}

		private void UpdatePageType()
		{
			var type = shellPage?.InstanceViewModel switch
			{
				null => PageTypes.None,
				{ IsPageTypeNotHome: false } => PageTypes.Home,
				{ IsPageTypeRecycleBin: true } => PageTypes.RecycleBin,
				{ IsPageTypeZipFolder: true } => PageTypes.ZipFolder,
				{ IsPageTypeFtp: true } => PageTypes.Ftp,
				{ IsPageTypeLibrary: true } => PageTypes.Library,
				{ IsPageTypeCloudDrive: true } => PageTypes.CloudDrive,
				{ IsPageTypeMtpDevice: true } => PageTypes.MtpDevice,
				{ IsPageTypeSearchResults: true } => PageTypes.SearchResults,
				_ => PageTypes.Folder,
			};
			SetProperty(ref pageType, type, nameof(PageType));
		}

		private void UpdateSelectedItems()
		{
			var items = shellPage?.ToolbarViewModel?.SelectedItems?.AsReadOnly() ?? EmptyListedItemList;
			SetProperty(ref selectedItems, items, nameof(SelectedItems));
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
				case nameof(ToolbarViewModel.CanCopy):
				case nameof(ToolbarViewModel.CanRefresh):
					OnPropertyChanged(e.PropertyName);
					break;
				case nameof(ToolbarViewModel.SelectedItems):
					UpdateSelectedItems();
					break;
			}
		}
	}
}
