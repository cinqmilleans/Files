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

		// Start
		PinToStart,
		UnpinFromStart,

		// Favorites
		PinItemToFavorites,
		UnpinItemFromFavorites,
	}
}
