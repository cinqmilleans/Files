using System.Collections.Generic;
using System.Collections.Immutable;

namespace Files.Backend.Services.Settings
{
	public interface IShortKeySettingsService
	{
		string GetShortKey(string id);
		IImmutableDictionary<string, string> GetShortKeys();

		void SetShortKey(string id, string shortKey);
		void SetShortKeys(IDictionary<string, string> shortKeys);
	}
}
