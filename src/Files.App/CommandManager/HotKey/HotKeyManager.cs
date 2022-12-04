using System.Collections.Generic;
using System.Linq;

namespace Files.App.CommandManager
{
	public class HotKeyManager : IHotKeyManager
	{
		private readonly IDictionary<HotKey, CommandCodes> hotKeys = new Dictionary<HotKey, CommandCodes>();

		public event HotKeyChangedEventHandler? HotKeyChanged;

		public CommandCodes this[HotKey hotKey]
		{
			get => hotKeys.TryGetValue(hotKey, out CommandCodes commandCode) ? commandCode : CommandCodes.None;
			set
			{
				var oldCommandCode = this[hotKey];
				if (oldCommandCode == value)
					return;

				if (value is CommandCodes.None)
					hotKeys.Remove(hotKey);
				else if (oldCommandCode is CommandCodes.None)
					hotKeys[hotKey] = value;
				else
					hotKeys.Add(hotKey, value);

				var args = new HotKeyChangedEventArgs
				{
					OldHotKey = hotKey,
					NewHotKey = hotKey,
					OldCommandCode = oldCommandCode,
					NewCommandCode = value,
				};
				HotKeyChanged?.Invoke(this, args);
			}
		}

		public HotKey this[CommandCodes commandCode]
		{
			get => hotKeys.FirstOrDefault(hotKey => hotKey.Value == commandCode).Key;
			set
			{
				var oldHotKey = this[commandCode];
				if (oldHotKey == value)
					return;

				if (value.IsNone)
					hotKeys.Remove(oldHotKey);
				else if (!oldHotKey.IsNone)
					hotKeys[oldHotKey] = commandCode;
				else
					hotKeys.Add(value, commandCode);

				var args = new HotKeyChangedEventArgs
				{
					OldHotKey = oldHotKey,
					NewHotKey = value,
					OldCommandCode = commandCode,
					NewCommandCode = commandCode,
				};
				HotKeyChanged?.Invoke(this, args);
			}
		}
	}
}
