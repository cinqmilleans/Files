using System.Collections.Generic;

namespace Files.App.Actions
{
	public class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, ActionCodes> hotKeys = new Dictionary<HotKey, ActionCodes>();

		public event HotKeyChangedEventHandler? HotKeyChanged;

		public HotKeyStatus GetStatus(HotKey hotKey) => hotKey switch
		{
			{ IsNone: true } => HotKeyStatus.Invalid,
			_ when hotKeys.ContainsKey(hotKey) => HotKeyStatus.Used,
			_ => HotKeyStatus.Available,
		};

		public ActionCodes GetActionCode(HotKey hotKey)
			=> hotKeys.TryGetValue(hotKey, out ActionCodes actionCode) ? actionCode : ActionCodes.None;

		public void SetAction(HotKey hotKey, ActionCodes actionCode)
		{
			var old = GetActionCode(hotKey);

			if (hotKeys.ContainsKey(hotKey))
				hotKeys[hotKey] = actionCode;
			else
				hotKeys.Add(hotKey, actionCode);

			if (actionCode != old)
				HotKeyChanged?.Invoke(this, new HotKeyChangedEventArgs(hotKey, old, actionCode));
		}
	}
}
