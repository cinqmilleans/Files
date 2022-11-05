namespace Files.App.Keyboard
{
	public enum KeyboardActionCodes
	{
		None,

		// general
		Help,

		// selection
		ToggleMultiSelection,
		SelectAll,
		InvertSelection,
		ClearSelection,

		// layout
		ToggleLayoutDetails,
		ToggleLayoutTiles,
		ToggleLayoutGridSmall,
		ToggleLayoutGridMedium,
		ToggleLayoutGridLarge,
		ToggleLayoutColumns,
		ToggleLayoutAdaptative,

		// file
		ToggleShowHiddenItems,
		ToggleShowFileExtensions,
	}
}
