namespace Files.App.Actions
{
	public interface IHotKeyManager
	{
		bool IsAvailable(HotKey hotKey);

		HotKey GetHotKey(Actions action);
		Actions GetAction(HotKey hotKey);

		void SetHotKey(Actions action, HotKey hotKey);
	}
}
