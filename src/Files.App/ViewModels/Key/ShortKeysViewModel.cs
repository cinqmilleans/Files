using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.App.Keyboard;
using Files.Backend.Services.Settings;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Windows.System;

namespace Files.App.ViewModels
{
    internal class ShortKeysViewModel : ObservableObject, IShortKeysViewModel
	{
		private readonly IImmutableDictionary<string, ShortKey> shortKeys;

		public ShortKey Help => Get();

		private int n = 0;
		public ShortKey ToggleMultiSelection => n++ % 2 == 0 ? SelectAll : ToggleLayoutAdaptative;

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

		private readonly List<(IList<KeyboardAccelerator> accelerators, string field)> items = new();

		public void Register(IList<KeyboardAccelerator> accelerators, string field)
		{
			items.Add((accelerators, field));
			accelerators.Clear();
			var shortKey = (ShortKey)GetType()?.GetProperty(field)?.GetValue(this)!;
			accelerators.Add(new KeyboardAccelerator { Key = shortKey.Key, Modifiers = shortKey.Modifiers, IsEnabled = false });
		}

		private ShortKey Get([CallerMemberName] string propertyName = "")
			=> shortKeys.ContainsKey(propertyName) ? shortKeys[propertyName] : ShortKey.None;
	}
}
