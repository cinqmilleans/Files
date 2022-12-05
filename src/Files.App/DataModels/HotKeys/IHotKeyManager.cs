using Files.App.Actions;

namespace Files.App.DataModels.HotKeys
{
	public interface IHotKeyManager
	{
		event HotKeyChangedEventHandler? HotKeyChanged;

		ActionCodes this[HotKey hotKey] { get; set; }
		HotKey this[ActionCodes commandCode] { get; set; }
	}
}
