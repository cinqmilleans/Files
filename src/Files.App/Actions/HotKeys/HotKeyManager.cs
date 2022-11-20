using System.Collections.Generic;

namespace Files.App.Actions.HotKeys
{
	public class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, ActionCodes> hotKeys = new Dictionary<HotKey, ActionCodes>();

		public event HotKeyChangedEventHandler? HotKeyChanged;

		public ActionCodes this[HotKey hotKey]
		{
			get
			{
				return hotKeys.TryGetValue(hotKey, out ActionCodes actionCode)
					? actionCode
					: ActionCodes.None;
			}
			set
			{
				var old = this[hotKey];

				if (hotKeys.ContainsKey(hotKey))
					hotKeys[hotKey] = value;
				else
					hotKeys.Add(hotKey, value);

				if (value != old)
					HotKeyChanged?.Invoke(this, new HotKeyChangedEventArgs(hotKey, old, value));
			}
		}

		public HotKeyStatus GetStatus(HotKey hotKey) => hotKey switch
		{
			{ IsNone: true } => HotKeyStatus.Invalid,
			_ when hotKeys.ContainsKey(hotKey) => HotKeyStatus.Used,
			_ => HotKeyStatus.Available,
		};
	}
}
