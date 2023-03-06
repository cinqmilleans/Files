using Microsoft.UI.Xaml.Controls;

namespace Files.App.Commands
{
	public enum CommandCodes
	{
		None,

		// Global
		OpenHelp,
		ToggleFullScreen,

		// Show
		ToggleShowHiddenItems,
		ToggleShowFileExtensions,

		// File System
		CopyItem,
		CutItem,
		DeleteItem,
		CreateFolder,
		CreateShortcut,
		CreateShortcutFromDialog,
		EmptyRecycleBin,
		RestoreRecycleBin,
		RestoreAllRecycleBin,

		// Selection
		MultiSelect,
		SelectAll,
		InvertSelection,
		ClearSelection,

		// Start
		PinToStart,
		UnpinFromStart,

		// Favorites
		PinItemToFavorites,
		UnpinItemFromFavorites,

		// Layout
		LayoutPrevious,
		LayoutNext,
		LayoutDetails,
		LayoutTiles,
		LayoutGridSmall,
		LayoutGridMedium,
		LayoutGridLarge,
		LayoutColumns,
		LayoutAdaptive,

		// Sort
		SortByName,
		SortByDateModified,
		SortByDateCreated,
		SortBySize,
		SortByType,
		SortBySyncStatus,
		SortByTag,
		SortByOriginalFolder,
		SortByDateDeleted,
		SortAscending,
		SortDescending,
		ToggleSortDirection,
	}
}
