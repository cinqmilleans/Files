using Files.App.DataModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Files.App.ViewModels
{
	internal class ShortKeysViewModel : IShortKeysViewModel
	{
		private readonly IImmutableDictionary<string, ShortKey> shortKeys;

		public ShortKey ToggleMultiSelection => Get();
		public ShortKey SelectAll => Get();
		public ShortKey InvertSelection => Get();
		public ShortKey ClearSelection => Get();

		public ShortKey ToggleLayoutDetails => Get();
		public ShortKey ToggleLayoutTiles => Get();
		public ShortKey ToggleLayoutGridSmall => Get();
		public ShortKey ToggleLayoutGridMedium => Get();
		public ShortKey ToggleLayoutGridLarge => Get();
		public ShortKey ToggleLayoutColumns => Get();
		public ShortKey ToggleLayoutAdaptative => Get();

		public ShortKey ToggleShowHiddenItems => Get();
		public ShortKey ToggleShowFileExtensions => Get();

		public ShortKeysViewModel(IDictionary<string, ShortKey> shortKeys)
			=> this.shortKeys = shortKeys.ToImmutableDictionary();

		private ShortKey Get([CallerMemberName] string propertyName = "")
			=> shortKeys?[propertyName] ?? ShortKey.None;
	}
}
