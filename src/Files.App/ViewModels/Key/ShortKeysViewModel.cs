using Files.Backend.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Files.App.ViewModels
{
	internal class ShortKeysViewModel : IShortKeysViewModel
	{
		private readonly IImmutableDictionary<string, ShortKey> shortKeys;

		public ShortKey SelectAll => Get();

		public ShortKeysViewModel(IDictionary<string, string> shortKeys)
			=> this.shortKeys = shortKeys
				.ToDictionary(item => item.Key, item => ShortKey.Parse(item.Value))
				.ToImmutableDictionary();

		private ShortKey Get([CallerMemberName] string propertyName = "")
			=> shortKeys[propertyName];
	}
}
