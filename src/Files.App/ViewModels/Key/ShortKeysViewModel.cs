using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.DataModels;
using Files.Backend.Services.Settings;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Files.App.ViewModels
{
	internal class ShortKeysViewModel : IShortKeysViewModel
	{
		private readonly IImmutableDictionary<string, ShortKey> shortKeys;

		public ShortKey Help => Get();

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

		public ShortKeysViewModel()
		{
			var service = Ioc.Default.GetService<IShortKeySettingsService>();
			shortKeys = service?.GetShortKeys()
				?.ToImmutableDictionary(item => item.Key, item => ShortKey.Parse(item.Value))
				?? ImmutableDictionary<string, ShortKey>.Empty;
		}

		private ShortKey Get([CallerMemberName] string propertyName = "")
			=> shortKeys?[propertyName] ?? ShortKey.None;
	}
}
