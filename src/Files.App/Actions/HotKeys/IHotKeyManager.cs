namespace Files.App.Actions.HotKeys
{
	public interface IHotKeyManager
	{
		event HotKeyChangedEventHandler? HotKeyChanged;

		ActionCodes this[HotKey hotKey] { get; set; }

		HotKeyStatus GetStatus(HotKey hotKey);
	}
}
