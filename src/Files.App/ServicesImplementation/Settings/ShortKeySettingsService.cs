using Files.Backend.Services.Settings;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Files.App.ServicesImplementation.Settings
{
	public class ShortKeySettingsService : IShortKeySettingsService
	{
		private static readonly IImmutableDictionary<string, string> shortKeys = new Dictionary<string, string>
		{
			["Help"] = "F1",
			["SelectAll"] = "Ctrl+A",
			["ToggleLayoutDetails"] = "Ctrl+Shift+1",
			["ToggleLayoutTiles"] = "Ctrl+Shift+2",
			["ToggleLayoutGridSmall"] = "Ctrl+Shift+3",
			["ToggleLayoutGridMedium"] = "Ctrl+Shift+4",
			["ToggleLayoutGridLarge"] = "Ctrl+Shift+5",
			["ToggleLayoutColumns"] = "Ctrl+Shift+6",
			["ToggleLayoutAdaptative"] = "Ctrl+Shift+7",
		}.ToImmutableDictionary();

		public string GetShortKey(string id) => shortKeys?[id] ?? string.Empty;
		public IImmutableDictionary<string, string> GetShortKeys() => shortKeys;

		public void SetShortKey(string id, string shortKey) {}
		public void SetShortKeys(IDictionary<string, string> shortKeys) {}
	}
}
