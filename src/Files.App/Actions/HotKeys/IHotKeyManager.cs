namespace Files.App.Actions.HotKeys
{
	public interface IHotKeyManager
	{
		event HotKeyChangedEventHandler? HotKeyChanged;

		HotKeyStatus GetStatus(HotKey hotKey);
		ActionCodes GetActionCode(HotKey hotKey);
		void SetAction(HotKey hotKey, ActionCodes actionCode);
	}
}
