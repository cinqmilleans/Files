using Files.Backend.Services.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files.App.ServicesImplementation.Settings
{
	public class ShortKeySettingsService : IShortKeySettingsService
	{
		public IDictionary<string, string> GetUserShortKeys()
		{
			return new Dictionary<string, string>
			{
				["SelectAll"] = "ctrl+b",
			};
		}
		public IDictionary<string, string> GetDefaultShortKeys()
		{
			return new Dictionary<string, string>
			{
				["SelectAll"] = "ctrl+a",
			};
		}

		public void SaveUserShortKeys(IDictionary<string, string> shortKeys)
		{
		}
	}
}
