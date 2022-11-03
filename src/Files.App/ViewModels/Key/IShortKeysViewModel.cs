using Files.Backend.Models;
using System.ComponentModel;

namespace Files.App.ViewModels
{
	public interface IShortKeysViewModel : INotifyPropertyChanged
	{
		// selection
		ShortKey ToggleMultiSelection { get; set; }
		ShortKey SelectAll { get; set; }
		ShortKey InvertSelection { get; set; }
		ShortKey ClearSelection { get; set; }

		// layout
		ShortKey ToggleLayoutDetails { get; set; }
		ShortKey ToggleLayoutTiles { get; set; }
		ShortKey ToggleLayoutGridSmall { get; set; }
		ShortKey ToggleLayoutGridMedium { get; set; }
		ShortKey ToggleLayoutGridLarge { get; set; }
		ShortKey ToggleLayoutColumns { get; set; }
		ShortKey ToggleLayoutAdaptative { get; set; }

		ShortKey ToggleShowHiddenItems { get; set; }
		ShortKey ToggleShowFileExtensions { get; set; }
	}
}
