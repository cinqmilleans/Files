namespace Files.App.Actions
{
	public enum ActionCodes
	{
		None,

		// general
		Help,

		// selection
		MultiSelection,
		SelectAll,
		InvertSelection,
		ClearSelection,

		// layout
		LayoutDetails,
		LayoutTiles,
		LayoutGridSmall,
		LayoutGridMedium,
		LayoutGridLarge,
		LayoutColumns,
		LayoutAdaptive,

		// folder
		OpenFolderInNewTab,
		OpenFolderInNewWindow,
		OpenFolderInMainPane,
		OpenFolderInSecondPane,

		// setting
		ShowHiddenItems,
		ShowFileExtensions,
	}
}
