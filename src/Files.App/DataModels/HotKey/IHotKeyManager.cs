using Files.App.Actions;

namespace Files.App.DataModels
{
	public interface IHotKeyManager
	{
		event HotKeyChangedEventHandler? HotKeyChanged;

		ActionCodes this[HotKey hotKey] { get; set; }
		HotKey this[ActionCodes commandCode] { get; set; }
	}
}
