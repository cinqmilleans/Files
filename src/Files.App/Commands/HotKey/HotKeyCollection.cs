using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Files.App.Commands
{
	public class HotKeyCollection : IEnumerable<HotKey>
	{
		private const char separator = ',';

		private readonly ImmutableArray<HotKey> hotKeys;

		public bool IsEmpty => hotKeys.IsEmpty;

		public string Code => string.Join(separator, hotKeys.Select(hotKey => hotKey.Code));
		public string Label => string.Join(separator, hotKeys.Where(hotKey => hotKey.IsVisible).Select(hotKey => hotKey.Code));

		public HotKeyCollection(params HotKey[] hotKeys) => this.hotKeys = hotKeys
			.Distinct()
			.Where(HotKey => !HotKey.IsNone)
			.ToImmutableArray();
		public HotKeyCollection(IEnumerable<HotKey> hotKeys) => this.hotKeys = hotKeys
			.Distinct()
			.Where(HotKey => !HotKey.IsNone)
			.ToImmutableArray();

		public static HotKeyCollection Parse(string code)
		{
			var hotKeys = code
				.Split(separator)
				.Select(part => part.Trim())
				.Select(HotKey.Parse);
			return new(hotKeys);
		}

		public void Contains(HotKey hotkey) => hotKeys.Contains(hotkey);

		IEnumerator IEnumerable.GetEnumerator() => hotKeys.AsEnumerable().GetEnumerator();
		public IEnumerator<HotKey> GetEnumerator() => hotKeys.AsEnumerable().GetEnumerator();

		public override string ToString() => Label;
	}
}
