using Files.App.Filesystem;
using Files.App.ViewModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace Files.App.Contexts
{
	internal class ContentPageContext : PageContext, IContentPageContext
	{
		private static readonly IReadOnlyList<ListedItem> EmptyListedItemList = Enumerable.Empty<ListedItem>().ToImmutableList();

		public IShellPage? ShellPage => Page;

		private ContentPageTypes pageType = ContentPageTypes.None;
		public ContentPageTypes PageType => pageType;

		public ListedItem? Folder => Page?.FilesystemViewModel?.CurrentFolder;

		public bool HasItem => Page?.ToolbarViewModel?.HasItem ?? false;

		public bool HasSelection => SelectedItems.Count is not 0;
		public ListedItem? SelectedItem => SelectedItems.Count is 1 ? SelectedItems[0] : null;

		private IReadOnlyList<ListedItem> selectedItems = EmptyListedItemList;
		public IReadOnlyList<ListedItem> SelectedItems => selectedItems;

		protected override void OnPageChanging()
		{
			if (Page is null)
				return;
			Page.InstanceViewModel.PropertyChanged -= InstanceViewModel_PropertyChanged;
			Page.ToolbarViewModel.PropertyChanged -= ToolbarViewModel_PropertyChanged;
		}

		protected override void OnPageChanged()
		{
			if (Page is null)
				return;
			Page.InstanceViewModel.PropertyChanged += InstanceViewModel_PropertyChanged;
			Page.ToolbarViewModel.PropertyChanged += ToolbarViewModel_PropertyChanged;
		}

		protected override void OnContentChanged() => Update();

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
			}
		}

		private void Update()
		{
			UpdatePageType();
			UpdateSelectedItems();

			OnPropertyChanged(nameof(Folder));
			OnPropertyChanged(nameof(HasItem));
		}

		private void UpdatePageType()
		{
			var type = Page?.InstanceViewModel switch
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
			bool oldHasSelection = HasSelection;
			ListedItem? oldSelectedItem = SelectedItem;

			IReadOnlyList<ListedItem> items = Page?.ToolbarViewModel?.SelectedItems?.AsReadOnly() ?? EmptyListedItemList;
			if (SetProperty(ref selectedItems, items, nameof(SelectedItems)))
			{
				if (HasSelection != oldHasSelection)
					OnPropertyChanged(nameof(HasSelection));
				if (SelectedItem != oldSelectedItem)
					OnPropertyChanged(nameof(SelectedItem));
			}
		}
	}
}
