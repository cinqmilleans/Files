using Files.App.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Files.App.DataModels
{
	public class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, ActionCodes> hotKeys = new Dictionary<HotKey, ActionCodes>();

		public event HotKeyChangedEventHandler? HotKeyChanged;

		public ActionCodes this[HotKey hotKey]
		{
			get => hotKeys.TryGetValue(hotKey, out ActionCodes ActionCode) ? ActionCode : ActionCodes.None;
			set
			{
				var oldActionCode = this[hotKey];
				if (oldActionCode == value)
					return;

				if (value is ActionCodes.None)
					hotKeys.Remove(hotKey);
				else if (oldActionCode is ActionCodes.None)
					hotKeys[hotKey] = value;
				else
					hotKeys.Add(hotKey, value);

				var args = new HotKeyChangedEventArgs
				{
					OldHotKey = hotKey,
					NewHotKey = hotKey,
					OldActionCode = oldActionCode,
					NewActionCode = value,
				};
				HotKeyChanged?.Invoke(this, args);
			}
		}

		public HotKey this[ActionCodes ActionCode]
		{
			get => hotKeys.FirstOrDefault(hotKey => hotKey.Value == ActionCode).Key;
			set
			{
				var oldHotKey = this[ActionCode];
				if (oldHotKey == value)
					return;

				if (value.IsNone)
					hotKeys.Remove(oldHotKey);
				else if (!oldHotKey.IsNone)
					hotKeys[oldHotKey] = ActionCode;
				else
					hotKeys.Add(value, ActionCode);

				var args = new HotKeyChangedEventArgs
				{
					OldHotKey = oldHotKey,
					NewHotKey = value,
					OldActionCode = ActionCode,
					NewActionCode = ActionCode,
				};
				HotKeyChanged?.Invoke(this, args);
			}
		}
	}
}
