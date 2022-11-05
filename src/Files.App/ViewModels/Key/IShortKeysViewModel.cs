using Files.App.Keyboard;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace Files.App.ViewModels
{
    public interface IShortKeysViewModel : INotifyPropertyChanged
	{
		// general
		ShortKey Help { get; }

		// selection
		ShortKey ToggleMultiSelection { get; }
		ShortKey SelectAll { get; }
		ShortKey InvertSelection { get; }
		ShortKey ClearSelection { get; }

		// layout
		ShortKey ToggleLayoutDetails { get; }
		ShortKey ToggleLayoutTiles { get; }
		ShortKey ToggleLayoutGridSmall { get; }
		ShortKey ToggleLayoutGridMedium { get; }
		ShortKey ToggleLayoutGridLarge { get; }
		ShortKey ToggleLayoutColumns { get; }
		ShortKey ToggleLayoutAdaptative { get; }

		ShortKey ToggleShowHiddenItems { get; }
		ShortKey ToggleShowFileExtensions { get; }

		// register
		void Register(IList<KeyboardAccelerator> accelerators, string field);
	}
}
