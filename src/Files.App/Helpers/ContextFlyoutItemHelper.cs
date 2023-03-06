using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Files.App.Commands;
using Files.App.DataModels.NavigationControlItems;
using Files.App.Extensions;
using Files.App.Filesystem;
using Files.App.Interacts;
using Files.App.ViewModels;
using Files.Backend.Helpers;
using Files.Backend.Services;
using Files.Backend.Services.Settings;
using Files.Shared.Enums;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace Files.App.Helpers
{
	/// <summary>
	/// Used to create lists of ContextMenuFlyoutItemViewModels that can be used by ItemModelListToContextFlyoutHelper to create context
	/// menus and toolbars for the user.
	/// <see cref="ContextMenuFlyoutItemViewModel"/>
	/// <see cref="Files.App.Helpers.ContextFlyouts.ItemModelListToContextFlyoutHelper"/>
	/// </summary>
	public static class ContextFlyoutItemHelper
	{
		private static readonly IUserSettingsService userSettingsService = Ioc.Default.GetRequiredService<IUserSettingsService>();
		private static readonly ICommandManager commands = Ioc.Default.GetRequiredService<ICommandManager>();
		private static readonly IAddItemService addItemService = Ioc.Default.GetRequiredService<IAddItemService>();

		public static List<ContextMenuFlyoutItemViewModel> GetItemContextCommandsWithoutShellItems(CurrentInstanceViewModel currentInstanceViewModel, List<ListedItem> selectedItems, BaseLayoutCommandsViewModel commandsViewModel, bool shiftPressed, SelectedItemsPropertiesViewModel? selectedItemsPropertiesViewModel, ItemViewModel? itemViewModel = null)
		{
			var menuItemsList = GetBaseItemMenuItems(commandsViewModel: commandsViewModel, selectedItems: selectedItems, selectedItemsPropertiesViewModel: selectedItemsPropertiesViewModel, currentInstanceViewModel: currentInstanceViewModel, itemViewModel: itemViewModel);
			menuItemsList = Filter(items: menuItemsList, shiftPressed: shiftPressed, currentInstanceViewModel: currentInstanceViewModel, selectedItems: selectedItems, removeOverflowMenu: false);
			return menuItemsList;
		}

		public static Task<List<ContextMenuFlyoutItemViewModel>> GetItemContextShellCommandsAsync(string workingDir, List<ListedItem> selectedItems, bool shiftPressed, bool showOpenMenu, CancellationToken cancellationToken)
			=> ShellContextmenuHelper.GetShellContextmenuAsync(shiftPressed: shiftPressed, showOpenMenu: showOpenMenu, workingDirectory: workingDir, selectedItems: selectedItems, cancellationToken: cancellationToken);

		public static List<ContextMenuFlyoutItemViewModel> Filter(List<ContextMenuFlyoutItemViewModel> items, List<ListedItem> selectedItems, bool shiftPressed, CurrentInstanceViewModel currentInstanceViewModel, bool removeOverflowMenu = true)
		{
			items = items.Where(x => Check(item: x, currentInstanceViewModel: currentInstanceViewModel, selectedItems: selectedItems)).ToList();
			items.ForEach(x => x.Items = x.Items?.Where(y => Check(item: y, currentInstanceViewModel: currentInstanceViewModel, selectedItems: selectedItems)).ToList());

			var overflow = items.Where(x => x.ID == "ItemOverflow").FirstOrDefault();
			if (overflow is not null)
			{
				if (!shiftPressed && userSettingsService.PreferencesSettingsService.MoveShellExtensionsToSubMenu) // items with ShowOnShift to overflow menu
				{
					var overflowItems = items.Where(x => x.ShowOnShift).ToList();

					// Adds a separator between items already there and the new ones
					if (overflow.Items.Count != 0 && overflowItems.Count > 0 && overflow.Items.Last().ItemType != ItemType.Separator)
						overflow.Items.Add(new ContextMenuFlyoutItemViewModel { ItemType = ItemType.Separator });

					items = items.Except(overflowItems).ToList();
					overflow.Items.AddRange(overflowItems);
				}

				// remove the overflow if it has no child items
				if (overflow.Items.Count == 0 && removeOverflowMenu)
					items.Remove(overflow);
			}

			return items;
		}

		private static bool Check(ContextMenuFlyoutItemViewModel item, CurrentInstanceViewModel currentInstanceViewModel, List<ListedItem> selectedItems)
		{
			return (item.ShowInRecycleBin || !currentInstanceViewModel.IsPageTypeRecycleBin)
				&& (item.ShowInSearchPage || !currentInstanceViewModel.IsPageTypeSearchResults)
				&& (item.ShowInFtpPage || !currentInstanceViewModel.IsPageTypeFtp)
				&& (item.ShowInZipPage || !currentInstanceViewModel.IsPageTypeZipFolder)
				&& (!item.SingleItemOnly || selectedItems.Count == 1)
				&& item.ShowItem;
		}

		public static List<ContextMenuFlyoutItemViewModel> GetBaseItemMenuItems(
			BaseLayoutCommandsViewModel commandsViewModel,
			SelectedItemsPropertiesViewModel? selectedItemsPropertiesViewModel,
			List<ListedItem> selectedItems,
			CurrentInstanceViewModel currentInstanceViewModel,
			ItemViewModel itemViewModel = null)
		{
			bool itemsSelected = itemViewModel is null;
			bool canDecompress = selectedItems.Any() && selectedItems.All(x => x.IsArchive)
				|| selectedItems.All(x => x.PrimaryItemAttribute == StorageItemTypes.File && FileExtensionHelpers.IsZipFile(x.FileExtension));
			bool canCompress = !canDecompress || selectedItems.Count > 1;
			bool showOpenItemWith = selectedItems.All(
				i => (i.PrimaryItemAttribute == StorageItemTypes.File && !i.IsShortcut && !i.IsExecutable) || (i.PrimaryItemAttribute == StorageItemTypes.Folder && i.IsArchive));
			bool areAllItemsFolders = selectedItems.All(i => i.PrimaryItemAttribute == StorageItemTypes.Folder);
			bool isFirstFileExecutable = FileExtensionHelpers.IsExecutableFile(selectedItems.FirstOrDefault()?.FileExtension);
			string newArchiveName =
				Path.GetFileName(selectedItems.Count is 1 ? selectedItems[0].ItemPath : Path.GetDirectoryName(selectedItems[0].ItemPath))
				?? string.Empty;

			return new List<ContextMenuFlyoutItemViewModel>()
			{
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "LayoutMode".GetLocalizedResource(),
					Glyph = "\uE152",
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					Items = new List<ContextMenuFlyoutItemViewModel>()
					{
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutDetails).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutTiles).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutGridSmall).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutGridMedium).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutGridLarge).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutColumns).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.LayoutAdaptive).Build(),
					},
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "SortBy".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconSort",
					},
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					Items = new List<ContextMenuFlyoutItemViewModel>()
					{
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByName).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByDateModified).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByDateCreated).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByType).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortBySize).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortBySyncStatus).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByTag).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByOriginalFolder).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortByDateDeleted).Build(),
						new ContextMenuFlyoutItemViewModel()
						{
							ItemType = ItemType.Separator,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
						},
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortAscending).Build(),
						new ContextMenuFlyoutItemViewModelBuilder(commands.SortDescending).Build(),
					},
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "NavToolbarGroupByRadioButtons/Text".GetLocalizedResource(),
					Glyph = "\uF168",
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					Items = new List<ContextMenuFlyoutItemViewModel>()
					{
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "None".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.None,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.None,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "Name".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.Name,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.Name,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "DateModifiedLowerCase".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.DateModified,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.DateModified,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "DateCreated".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.DateCreated,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.DateCreated,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "Type".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.FileType,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.FileType,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "Size".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.Size,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.Size,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "SyncStatus".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.SyncStatus,
							ShowItem = currentInstanceViewModel.IsPageTypeCloudDrive,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.SyncStatus,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "FileTags".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.FileTag,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.FileTag,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "NavToolbarArrangementOptionOriginalFolder/Text".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.OriginalFolder,
							ShowInRecycleBin = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.OriginalFolder,
							ItemType = ItemType.Toggle,
							ShowItem = currentInstanceViewModel.IsPageTypeRecycleBin,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "DateDeleted".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.DateDeleted,
							ShowInRecycleBin = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.DateDeleted,
							ItemType = ItemType.Toggle,
							ShowItem = currentInstanceViewModel.IsPageTypeRecycleBin,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "NavToolbarArrangementOptionFolderPath/Text".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupOption == GroupOption.FolderPath,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupOptionCommand,
							CommandParameter = GroupOption.FolderPath,
							ItemType = ItemType.Toggle,
							ShowItem = currentInstanceViewModel.IsPageTypeLibrary,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							ItemType = ItemType.Separator,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "Ascending".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupDirection == SortDirection.Ascending,
							IsEnabled = currentInstanceViewModel.FolderSettings.DirectoryGroupOption != GroupOption.None,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupDirectionCommand,
							CommandParameter = SortDirection.Ascending,
							ItemType = ItemType.Toggle,
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "Descending".GetLocalizedResource(),
							IsChecked = currentInstanceViewModel.FolderSettings.DirectoryGroupDirection == SortDirection.Descending,
							IsEnabled = currentInstanceViewModel.FolderSettings.DirectoryGroupOption != GroupOption.None,
							ShowInRecycleBin = true,
							ShowInSearchPage = true,
							ShowInFtpPage = true,
							ShowInZipPage = true,
							Command = currentInstanceViewModel.FolderSettings.ChangeGroupDirectionCommand,
							CommandParameter = SortDirection.Descending,
							ItemType = ItemType.Toggle,
						},
					},
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutContextFlyoutRefresh/Text".GetLocalizedResource(),
					Glyph = "\uE72C",
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					Command = commandsViewModel.RefreshCommand,
					KeyboardAccelerator = new KeyboardAccelerator
					{
						Key = VirtualKey.F5,
						IsEnabled = false,
					},
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					ItemType = ItemType.Separator,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutContextFlyoutNew/Label".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconNew",
					},
					KeyboardAccelerator = new KeyboardAccelerator
					{
						Key = VirtualKey.N,
						Modifiers = VirtualKeyModifiers.Control,
						IsEnabled = false,
					},
					Items = GetNewItemItems(commandsViewModel, currentInstanceViewModel.CanCreateFileInPage),
					ShowInFtpPage = true,
					ShowInZipPage = true,
					ShowItem = !itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "FormatDriveText".GetLocalizedResource(),
					Command = commandsViewModel.FormatDriveCommand,
					CommandParameter = itemViewModel?.CurrentFolder,
					ShowItem = itemViewModel?.CurrentFolder is not null && (App.DrivesManager.Drives.FirstOrDefault(x => string.Equals(x.Path, itemViewModel?.CurrentFolder.ItemPath))?.MenuOptions.ShowFormatDrive ?? false),
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.EmptyRecycleBin)
				{
					IsVisible = currentInstanceViewModel.IsPageTypeRecycleBin && !itemsSelected,
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.RestoreAllRecycleBin)
				{
					IsVisible = currentInstanceViewModel.IsPageTypeRecycleBin && !itemsSelected,
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.RestoreRecycleBin)
				{
					IsVisible = currentInstanceViewModel.IsPageTypeRecycleBin && itemsSelected,
				}.Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "Open".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconOpenFile"
					},
					Command = commandsViewModel.OpenItemCommand,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					ShowItem = itemsSelected && selectedItems.Count <= 10,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutOpenItemWith/Text".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconOpenWith"
					},
					Command = commandsViewModel.OpenItemWithApplicationPickerCommand,
					Tag = "OpenWith",
					CollapseLabel = true,
					ShowInSearchPage = true,
					ShowItem = itemsSelected && showOpenItemWith
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutOpenItemWith/Text".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconOpenWith"
					},
					Tag = "OpenWithOverflow",
					IsHidden = true,
					CollapseLabel = true,
					Items = new List<ContextMenuFlyoutItemViewModel>() {
						new()
						{
							Text = "Placeholder",
							ShowInSearchPage = true,
						}
					},
					ShowInSearchPage = true,
					ShowItem = itemsSelected && showOpenItemWith
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "OpenFileLocation".GetLocalizedResource(),
					Glyph = "\uE8DA",
					Command = commandsViewModel.OpenFileLocationCommand,
					ShowItem = itemsSelected && selectedItems.All(i => i.IsShortcut),
					ShowInSearchPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "OpenInNewTab".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconOpenInNewTab"
					},
					Command = commandsViewModel.OpenDirectoryInNewTabCommand,
					ShowItem = itemsSelected && selectedItems.Count < 5 && areAllItemsFolders && userSettingsService.PreferencesSettingsService.ShowOpenInNewTab,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "OpenInNewWindow".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconOpenInNewWindow"
					},
					Command = commandsViewModel.OpenInNewWindowItemCommand,
					ShowItem = itemsSelected && selectedItems.Count < 5 && areAllItemsFolders && userSettingsService.PreferencesSettingsService.ShowOpenInNewWindow,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "OpenInNewPane".GetLocalizedResource(),
					Command = commandsViewModel.OpenDirectoryInNewPaneCommand,
					ShowItem = itemsSelected && userSettingsService.PreferencesSettingsService.ShowOpenInNewPane && areAllItemsFolders,
					SingleItemOnly = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutSetAs/Text".GetLocalizedResource(),
					ShowItem = itemsSelected && (selectedItemsPropertiesViewModel?.IsSelectedItemImage ?? false),
					ShowInSearchPage = true,
					Items = new List<ContextMenuFlyoutItemViewModel>()
					{
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "SetAsBackground".GetLocalizedResource(),
							Glyph = "\uE91B",
							Command = commandsViewModel.SetAsDesktopBackgroundItemCommand,
							ShowInSearchPage = true,
							ShowItem = selectedItemsPropertiesViewModel?.SelectedItemsCount == 1
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "BaseLayoutItemContextFlyoutSetAsLockscreenBackground/Text".GetLocalizedResource(),
							Command = commandsViewModel.SetAsLockscreenBackgroundItemCommand,
							ShowInSearchPage = true,
							ShowItem = selectedItemsPropertiesViewModel?.SelectedItemsCount == 1
						},
						new ContextMenuFlyoutItemViewModel()
						{
							Text = "SetAsSlideshow".GetLocalizedResource(),
							Command = commandsViewModel.SetAsDesktopBackgroundItemCommand,
							ShowInSearchPage = true,
							ShowItem = selectedItemsPropertiesViewModel?.SelectedItemsCount > 1
						},
					}
				},
				new ContextMenuFlyoutItemViewModel
				{
					Text = "RotateLeft".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconRotateLeft"
					},
					Command = commandsViewModel.RotateImageLeftCommand,
					ShowInSearchPage = true,
					ShowItem = selectedItemsPropertiesViewModel?.IsSelectedItemImage ?? false
				},
				new ContextMenuFlyoutItemViewModel
				{
					Text = "RotateRight".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconRotateRight"
					},
					Command = commandsViewModel.RotateImageRightCommand,
					ShowInSearchPage = true,
					ShowItem = selectedItemsPropertiesViewModel?.IsSelectedItemImage ?? false
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "RunAsAdministrator".GetLocalizedResource(),
					Glyph = "\uE7EF",
					Command = commandsViewModel.RunAsAdminCommand,
					ShowInSearchPage = true,
					ShowItem = itemsSelected && isFirstFileExecutable
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutContextFlyoutRunAsAnotherUser/Text".GetLocalizedResource(),
					Glyph = "\uE7EE",
					Command = commandsViewModel.RunAsAnotherUserCommand,
					ShowInSearchPage = true,
					ShowItem = itemsSelected && isFirstFileExecutable
				},
				new ContextMenuFlyoutItemViewModel()
				{
					ItemType = ItemType.Separator,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.CutItem)
				{
					IsVisible = itemsSelected,
					IsPrimary = true,
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.CopyItem)
				{
					IsVisible = itemsSelected,
					IsPrimary = true,
				}.Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "CopyLocation".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconCopyLocation",
					},
					Command = commandsViewModel.CopyPathOfSelectedItemCommand,
					SingleItemOnly = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "Paste".GetLocalizedResource(),
					IsPrimary = true,
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconPaste",
					},
					Command = commandsViewModel.PasteItemsFromClipboardCommand,
					ShowItem = areAllItemsFolders || !itemsSelected,
					SingleItemOnly = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					IsEnabled = App.AppModel.IsPasteEnabled,
					KeyboardAccelerator = new KeyboardAccelerator
					{
						Key = VirtualKey.V,
						Modifiers = VirtualKeyModifiers.Control,
						IsEnabled = false,
					},
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutCreateFolderWithSelection/Text".GetLocalizedResource(),
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconNewFolder",
					},
					Command = commandsViewModel.CreateFolderWithSelection,
					ShowItem = itemsSelected,
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.CreateShortcut)
				{
					IsVisible = itemsSelected && (!selectedItems.FirstOrDefault()?.IsShortcut ?? false)
						&& !currentInstanceViewModel.IsPageTypeRecycleBin,
				}.Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "Rename".GetLocalizedResource(),
					IsPrimary = true,
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconRename",
					},
					Command = commandsViewModel.RenameItemCommand,
					SingleItemOnly = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					KeyboardAccelerator = new KeyboardAccelerator
					{
						Key = VirtualKey.F2,
						IsEnabled = false,
					},
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutShare/Text".GetLocalizedResource(),
					IsPrimary = true,
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconShare",
					},
					Command = commandsViewModel.ShareItemCommand,
					ShowItem = itemsSelected && DataTransferManager.IsSupported() && !selectedItems.Any(i => i.IsHiddenItem || (i.IsShortcut && !i.IsLinkItem) || (i.PrimaryItemAttribute == StorageItemTypes.Folder && !i.IsArchive)),
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.DeleteItem)
				{
					IsVisible = itemsSelected,
					IsPrimary = true,
				}.Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "Properties".GetLocalizedResource(),
					IsPrimary = true,
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconProperties",
					},
					Command = commandsViewModel.ShowPropertiesCommand,
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					ShowInFtpPage = true,
					ShowInZipPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "BaseLayoutItemContextFlyoutOpenParentFolder/Text".GetLocalizedResource(),
					Glyph = "\uE197",
					Command = commandsViewModel.OpenParentFolderCommand,
					ShowItem = itemsSelected && currentInstanceViewModel.IsPageTypeSearchResults,
					SingleItemOnly = true,
					ShowInSearchPage = true,
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.PinItemToFavorites)
				{
					IsVisible = userSettingsService.PreferencesSettingsService.ShowFavoritesSection && selectedItems.All(x => x.PrimaryItemAttribute == StorageItemTypes.Folder && !x.IsArchive && !x.IsPinned),
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.UnpinItemFromFavorites)
				{
					IsVisible = userSettingsService.PreferencesSettingsService.ShowFavoritesSection && selectedItems.All(x => x.PrimaryItemAttribute == StorageItemTypes.Folder && !x.IsArchive && x.IsPinned),
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.PinToStart)
				{
					IsVisible = selectedItems.All(x => !x.IsShortcut && (x.PrimaryItemAttribute == StorageItemTypes.Folder || x.IsExecutable) && !x.IsArchive && !x.IsItemPinnedToStart),
					ShowOnShift = true,
				}.Build(),
				new ContextMenuFlyoutItemViewModelBuilder(commands.UnpinFromStart)
				{
					IsVisible = selectedItems.All(x => !x.IsShortcut && (x.PrimaryItemAttribute == StorageItemTypes.Folder || x.IsExecutable) && !x.IsArchive && x.IsItemPinnedToStart),
					ShowOnShift = true,
				}.Build(),
				new ContextMenuFlyoutItemViewModel
				{
					Text = "Archive".GetLocalizedResource(),
					ShowInSearchPage = true,
					OpacityIcon = new OpacityIconModel()
					{
						OpacityIconStyle = "ColorIconZip",
					},
					Items = new List<ContextMenuFlyoutItemViewModel>
					{
						new ContextMenuFlyoutItemViewModel
						{
							Text = "ExtractFiles".GetLocalizedResource(),
							Command = commandsViewModel.DecompressArchiveCommand,
							ShowItem = canDecompress,
							ShowInSearchPage = true,
						},
						new ContextMenuFlyoutItemViewModel
						{
							Text = "ExtractHere".GetLocalizedResource(),
							Command = commandsViewModel.DecompressArchiveHereCommand,
							ShowItem = canDecompress,
							ShowInSearchPage = true,
						},
						new ContextMenuFlyoutItemViewModel
						{
							Text = selectedItems.Count > 1
								? string.Format("BaseLayoutItemContextFlyoutExtractToChildFolder".GetLocalizedResource(), "*")
								: string.Format("BaseLayoutItemContextFlyoutExtractToChildFolder".GetLocalizedResource(),
									Path.GetFileNameWithoutExtension(selectedItems.First().Name)),
							Command = commandsViewModel.DecompressArchiveToChildFolderCommand,
							ShowInSearchPage = true,
							ShowItem = canDecompress,
						},
						new ContextMenuFlyoutItemViewModel
						{
							ShowItem = canDecompress && canCompress,
							ItemType = ItemType.Separator,
						},
						new ContextMenuFlyoutItemViewModel
						{
							Text = "CreateArchive".GetLocalizedResource(),
							Command = commandsViewModel.CompressIntoArchiveCommand,
							ShowItem = canCompress,
							ShowInSearchPage = true,
						},
						new ContextMenuFlyoutItemViewModel
						{
							Text = string.Format("CreateNamedArchive".GetLocalizedResource(), $"{newArchiveName}.zip"),
							Command = commandsViewModel.CompressIntoZipCommand,
							ShowItem = canCompress,
							ShowInSearchPage = true,
						},
						new ContextMenuFlyoutItemViewModel
						{
							Text = string.Format("CreateNamedArchive".GetLocalizedResource(), $"{newArchiveName}.7z"),
							Command = commandsViewModel.CompressIntoSevenZipCommand,
							ShowItem = canCompress,
							ShowInSearchPage = true,
						},
					},
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "SendTo".GetLocalizedResource(),
					Tag = "SendTo",
					CollapseLabel = true,
					ShowInSearchPage = true,
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "SendTo".GetLocalizedResource(),
					Tag = "SendToOverflow",
					IsHidden = true,
					CollapseLabel = true,
					Items = new List<ContextMenuFlyoutItemViewModel>() {
						new()
						{
							Text = "Placeholder",
							ShowInSearchPage = true,
						}
					},
					ShowInSearchPage = true,
					ShowItem = itemsSelected
				},
				new ContextMenuFlyoutItemViewModel()
				{
					ItemType = ItemType.Separator,
					Tag = "OverflowSeparator",
					ShowInSearchPage = true,
				},
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "Loading".GetLocalizedResource(),
					Glyph = "\xE712",
					Items = new List<ContextMenuFlyoutItemViewModel>(),
					ID = "ItemOverflow",
					Tag = "ItemOverflow",
					ShowInRecycleBin = true,
					ShowInSearchPage = true,
					IsEnabled = false
				},
			}.Where(x => x.ShowItem).ToList();
		}

		public static List<ContextMenuFlyoutItemViewModel> GetNewItemItems(BaseLayoutCommandsViewModel commandsViewModel, bool canCreateFileInPage)
		{
			var list = new List<ContextMenuFlyoutItemViewModel>()
			{
				new ContextMenuFlyoutItemViewModelBuilder(commands.CreateFolder).Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					Text = "File".GetLocalizedResource(),
					Glyph = "\uE7C3",
					Command = commandsViewModel.CreateNewFileCommand,
					CommandParameter = null,
					ShowInFtpPage = true,
					ShowInZipPage = true,
					IsEnabled = canCreateFileInPage
				},
				new ContextMenuFlyoutItemViewModelBuilder(commands.CreateShortcutFromDialog).Build(),
				new ContextMenuFlyoutItemViewModel()
				{
					ItemType = ItemType.Separator,
				}
			};

			if (canCreateFileInPage)
			{
				var cachedNewContextMenuEntries = addItemService.GetNewEntriesAsync().Result;
				cachedNewContextMenuEntries?.ForEach(i =>
				{
					if (!string.IsNullOrEmpty(i.IconBase64))
					{
						// loading the bitmaps takes a while, so this caches them
						byte[] bitmapData = Convert.FromBase64String(i.IconBase64);
						using var ms = new MemoryStream(bitmapData);
						var bitmap = new BitmapImage();
						_ = bitmap.SetSourceAsync(ms.AsRandomAccessStream());
						list.Add(new ContextMenuFlyoutItemViewModel()
						{
							Text = i.Name,
							BitmapIcon = bitmap,
							Command = commandsViewModel.CreateNewFileCommand,
							CommandParameter = i,
						});
					}
					else
					{
						list.Add(new ContextMenuFlyoutItemViewModel()
						{
							Text = i.Name,
							Glyph = "\xE7C3",
							Command = commandsViewModel.CreateNewFileCommand,
							CommandParameter = i,
						});
					}
				});
			}

			return list;
		}
	}
}