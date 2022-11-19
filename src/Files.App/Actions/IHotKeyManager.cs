using System;

namespace Files.App.Actions
{
	public delegate void HotKeyChangedEventHandler(IHotKeyManager manager, HotKeyChangedEventArgs args);

	public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey HotKey { get; }
		public ActionCodes OldActionCode { get; }
		public ActionCodes NewActionCode { get; }

		public HotKeyChangedEventArgs(HotKey hotKey, ActionCodes oldActionCode, ActionCodes newActionCode)
			=> (HotKey, OldActionCode, NewActionCode) = (hotKey, oldActionCode, newActionCode);
	}

	public interface IHotKeyManager
	{
		event HotKeyChangedEventHandler? HotKeyChanged;

		HotKeyStatus GetStatus(HotKey hotKey);
		ActionCodes GetActionCode(HotKey hotKey);
		void SetAction(HotKey hotKey, ActionCodes actionCode);
	}
}
