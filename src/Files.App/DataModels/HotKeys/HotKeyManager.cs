using Files.App.CommandActions;
using System.Collections.Generic;
using System.Linq;

namespace Files.App.DataModels.HotKeys
{
    public class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, CommandCodes> hotKeys = new Dictionary<HotKey, CommandCodes>();

		public event HotKeyChangedEventHandler? HotKeyChanged;

		public CommandCodes this[HotKey hotKey]
		{
			get => hotKeys.TryGetValue(hotKey, out CommandCodes ActionCode) ? ActionCode : CommandCodes.None;
			set
			{
				var oldActionCode = this[hotKey];
				if (oldActionCode == value)
					return;

				if (value is CommandCodes.None)
					hotKeys.Remove(hotKey);
				else if (oldActionCode is CommandCodes.None)
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

		public HotKey this[CommandCodes ActionCode]
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
