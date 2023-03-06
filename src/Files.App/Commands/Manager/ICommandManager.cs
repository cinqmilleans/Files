using System;
using System.Collections.Generic;

namespace Files.App.Commands
{
	public interface ICommandManager : IEnumerable<IRichCommand>
	{
		event EventHandler<HotKeyChangedEventArgs>? HotKeyChanged;

		IRichCommand this[CommandCodes code] { get; }
		IRichCommand this[HotKey customHotKey] { get; }

		IRichCommand None { get; }

		IRichCommand OpenHelp { get; }
		IRichCommand ToggleFullScreen { get; }

		IRichCommand ToggleShowHiddenItems { get; }
		IRichCommand ToggleShowFileExtensions { get; }

		IRichCommand CopyItem { get; }
		IRichCommand CutItem { get; }
		IRichCommand DeleteItem { get; }
		IRichCommand MultiSelect { get; }
		IRichCommand SelectAll { get; }
		IRichCommand InvertSelection { get; }
		IRichCommand ClearSelection { get; }
		IRichCommand CreateFolder { get; }
		IRichCommand CreateShortcut { get; }
		IRichCommand CreateShortcutFromDialog { get; }
		IRichCommand EmptyRecycleBin { get; }
		IRichCommand RestoreRecycleBin { get; }
		IRichCommand RestoreAllRecycleBin { get; }

		IRichCommand PinToStart { get; }
		IRichCommand UnpinFromStart { get; }
		IRichCommand PinItemToFavorites { get; }
		IRichCommand UnpinItemFromFavorites { get; }

		IRichCommand LayoutPrevious { get; }
		IRichCommand LayoutNext { get; }
		IRichCommand LayoutDetails { get; }
		IRichCommand LayoutTiles { get; }
		IRichCommand LayoutGridSmall { get; }
		IRichCommand LayoutGridMedium { get; }
		IRichCommand LayoutGridLarge { get; }
		IRichCommand LayoutColumns { get; }
		IRichCommand LayoutAdaptive { get; }

		IRichCommand SortByName { get; }
		IRichCommand SortByDateModified { get; }
		IRichCommand SortByDateCreated { get; }
		IRichCommand SortBySize { get; }
		IRichCommand SortByType { get; }
		IRichCommand SortBySyncStatus { get; }
		IRichCommand SortByTag { get; }
		IRichCommand SortByOriginalFolder { get; }
		IRichCommand SortByDateDeleted { get; }
		IRichCommand SortAscending { get; }
		IRichCommand SortDescending { get; }
		IRichCommand ToggleSortDirection { get; }
		IRichCommand ToggleSortDirectoriesAlongsideFiles { get; }

		IRichCommand GroupAscending { get; }
		IRichCommand GroupDescending { get; }
		IRichCommand ToggleGroupDirection { get; }
	}
}
