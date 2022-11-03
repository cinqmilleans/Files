using System.Collections.Generic;

namespace Files.Backend.Services.Settings
{
	public interface IShortKeySettingsService
	{
		IDictionary<string, string> GetUserShortKeys();
		IDictionary<string, string> GetDefaultShortKeys();

		void SaveUserShortKeys(IDictionary<string, string> shortKeys);
	}
}
