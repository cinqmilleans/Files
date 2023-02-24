using CommunityToolkit.Mvvm.ComponentModel;
using Files.App.Filesystem;
using Files.App.Helpers;
using Files.App.ViewModels;
using Files.App.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.App.Contexts
{
	internal class ContentPageContext : ObservableObject, IContentPageContext
	{
		private static readonly IReadOnlyList<ListedItem> EmptyListedItemList = Enumerable.Empty<ListedItem>().ToImmutableList();

		private IShellPage shellPage = new NoneShellPage();
		public IShellPage ShellPage => shellPage;

		private ContentPageTypes pageType = ContentPageTypes.None;
		public ContentPageTypes PageType => pageType;

		public string Folder => ShellPage.FilesystemViewModel?.WorkingDirectory ?? string.Empty;

		public bool HasItem => ShellPage.ToolbarViewModel?.HasItem ?? false;
		public int ItemCount => ShellPage.FilesystemViewModel?.FilesAndFolders?.Count ?? 0;

		private IReadOnlyList<ListedItem> selectedItems = EmptyListedItemList;
		public IReadOnlyList<ListedItem> SelectedItems => selectedItems;

		public ContentPageContext() => BaseShellPage.CurrentInstanceChanged += BaseShellPage_CurrentInstanceChanged;

		public void SelectAll() => ShellPage.SlimContentPage?.ItemManipulationModel?.SelectAllItems();
		public void ClearSelection() => ShellPage.SlimContentPage?.ItemManipulationModel?.ClearSelection();
		public void InvertSelection() => ShellPage.SlimContentPage?.ItemManipulationModel?.InvertSelection();

		public void StartRename() => ShellPage.SlimContentPage?.ItemManipulationModel?.StartRenameItem();

		public async Task Restore()
		{
			var items = SelectedItems.Where(x => x is RecycleBinItem).Select(ToPath);

			await ShellPage.FilesystemHelpers.RestoreItemsFromTrashAsync(items.Select(x => x.Source), items.Select(x => x.Destination), true);

			static (IStorageItemWithPath Source, string Destination) ToPath(ListedItem item) => (
				StorageHelpers.FromPathAndType(item.ItemPath, ToFilesystemItemType(item.PrimaryItemAttribute)),
				((RecycleBinItem)item).ItemOriginalPath
			);

			static FilesystemItemType ToFilesystemItemType(StorageItemTypes type)
				=> type is StorageItemTypes.File ? FilesystemItemType.File : FilesystemItemType.Directory;
		}

		private void UpdateShellPage(IShellPage newShellPage)
		{
			if (ShellPage == newShellPage)
				return;

			if (ShellPage is not NoneShellPage)
			{
				ShellPage.FilesystemViewModel.WorkingDirectoryModified -= FilesystemViewModel_WorkingDirectoryModified;
				ShellPage.FilesystemViewModel.FilesAndFolders.CollectionChanged -= FilesAndFolders_CollectionChanged;
				ShellPage.InstanceViewModel.PropertyChanged -= InstanceViewModel_PropertyChanged;
				ShellPage.ToolbarViewModel.PropertyChanged -= ToolbarViewModel_PropertyChanged;
			}

			shellPage = newShellPage;

			if (ShellPage is not NoneShellPage)
			{
				ShellPage.FilesystemViewModel.WorkingDirectoryModified += FilesystemViewModel_WorkingDirectoryModified;
				ShellPage.FilesystemViewModel.FilesAndFolders.CollectionChanged += FilesAndFolders_CollectionChanged;
				ShellPage.InstanceViewModel.PropertyChanged += InstanceViewModel_PropertyChanged;
				ShellPage.ToolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;
			}

			UpdatePageType();
			UpdateSelectedItems();

			OnPropertyChanged(nameof(Folder));
			OnPropertyChanged(nameof(HasItem));
			OnPropertyChanged(nameof(ItemCount));
			OnPropertyChanged(nameof(ShellPage));
		}

		private void UpdatePageType()
		{
			var type = ShellPage.InstanceViewModel switch
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

		private void UpdateSelectedItems()
		{
			var items = ShellPage.ToolbarViewModel?.SelectedItems?.AsReadOnly() ?? EmptyListedItemList;
			SetProperty(ref selectedItems, items, nameof(SelectedItems));
		}

		private void BaseShellPage_CurrentInstanceChanged(object? sender, BaseShellPage newShellPage)
		{
			UpdateShellPage(newShellPage);
		}

		private void FilesystemViewModel_WorkingDirectoryModified(object sender, WorkingDirectoryModifiedEventArgs e)
		{
			OnPropertyChanged(nameof(Folder));
		}

		private void FilesAndFolders_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			OnPropertyChanged(nameof(ItemCount));
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
				case nameof(ToolbarViewModel.HasItem):
					OnPropertyChanged(e.PropertyName);
					break;
				case nameof(ToolbarViewModel.SelectedItems):
					UpdateSelectedItems();
					break;
			}
		}

		private class NoneShellPage : BaseShellPage
		{
			public NoneShellPage() : base(new()) {}

			public override bool CanNavigateForward => false;

			public override bool CanNavigateBackward => false;

			protected override Frame ItemDisplay => throw new NotImplementedException();

			public override void NavigateHome() {}
			public override void NavigateToPath(string? navigationPath, Type? sourcePageType, NavigationArguments? navArgs = null) {}

			public override void Up_Click() {}
		}
	}
}
