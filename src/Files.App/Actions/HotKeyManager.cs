using System.Collections.Generic;
using System.Linq;
using Windows.System;

namespace Files.App.Actions
{
	internal class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, Actions> hotKeys = new Dictionary<HotKey, Actions>
		{
			[new(VirtualKey.B, VirtualKeyModifiers.Control)] = Actions.Rename,
		};

		public bool IsAvailable(HotKey hotKey) => !hotKeys.ContainsKey(hotKey);

		public HotKey GetHotKey(Actions action) => hotKeys.FirstOrDefault(key => key.Value == action).Key;
		public Actions GetAction(HotKey hotKey) => hotKeys.TryGetValue(hotKey, out Actions value) ? value : Actions.None;

		public void SetHotKey(Actions action, HotKey hotKey)
		{
			var oldHotKey = GetHotKey(action);
			if (!oldHotKey.IsNone)
				hotKeys.Remove(oldHotKey);

			if (hotKeys.ContainsKey(hotKey))
				hotKeys[hotKey] = action;
			else
				hotKeys.Add(hotKey, action);
		}
	}
}
