using System;

namespace Files.App.Actions.HotKeys
{
	public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey HotKey { get; }
		public ActionCodes OldActionCode { get; }
		public ActionCodes NewActionCode { get; }

		public HotKeyChangedEventArgs(HotKey hotKey, ActionCodes oldActionCode, ActionCodes newActionCode)
			=> (HotKey, OldActionCode, NewActionCode) = (hotKey, oldActionCode, newActionCode);
	}
}
