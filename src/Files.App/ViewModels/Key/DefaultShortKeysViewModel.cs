using Files.App.DataModels;

namespace Files.App.ViewModels
{
	internal class DefaultShortKeysViewModel : IShortKeysViewModel
	{
		public ShortKey ToggleMultiSelection { get; } = ShortKey.None;
		public ShortKey SelectAll { get; } = "Ctrl+A";
		public ShortKey InvertSelection { get; } = ShortKey.None;
		public ShortKey ClearSelection { get; } = ShortKey.None;

		public ShortKey ToggleLayoutDetails { get; } = "Ctrl+Shift+1";
		public ShortKey ToggleLayoutTiles { get; } = "Ctrl+Shift+2";
		public ShortKey ToggleLayoutGridSmall { get; } = "Ctrl+Shift+3";
		public ShortKey ToggleLayoutGridMedium { get; } = "Ctrl+Shift+4";
		public ShortKey ToggleLayoutGridLarge { get; } = "Ctrl+Shift+5";
		public ShortKey ToggleLayoutColumns { get; } = "Ctrl+Shift+6";
		public ShortKey ToggleLayoutAdaptative { get; } = "Ctrl+Shift+7";

		public ShortKey ToggleShowHiddenItems { get; } = ShortKey.None;
		public ShortKey ToggleShowFileExtensions { get; } = ShortKey.None;
	}
}
