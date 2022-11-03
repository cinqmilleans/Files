using Files.Backend.Services.Settings;
using System.Collections.Generic;

namespace Files.App.ServicesImplementation.Settings
{
	public class ShortKeySettingsService : IShortKeySettingsService
	{
		public IDictionary<string, string> GetUserShortKeys()
			=> GetDefaultShortKeys();
		public IDictionary<string, string> GetDefaultShortKeys()
		{
			return new Dictionary<string, string>
			{
				["SelectAll"] = "Ctrl+A",
				["ToggleLayoutDetails"] = "Ctrl+Shift+1",
				["ToggleLayoutTiles"] = "Ctrl+Shift+2",
				["ToggleLayoutGridSmall"] = "Ctrl+Shift+3",
				["ToggleLayoutGridMedium"] = "Ctrl+Shift+4",
				["ToggleLayoutGridLarge"] = "Ctrl+Shift+5",
				["ToggleLayoutColumns"] = "Ctrl+Shift+6",
				["ToggleLayoutAdaptative"] = "Ctrl+Shift+7",
			};
		}

		public void SaveUserShortKeys(IDictionary<string, string> shortKeys)
		{
		}
	}
}
