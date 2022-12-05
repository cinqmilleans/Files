using Files.App.Actions;
using System;

namespace Files.App.DataModels.HotKeys
{
	public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey OldHotKey { get; init; } = HotKey.None;
		public HotKey NewHotKey { get; init; } = HotKey.None;

		public ActionCodes OldActionCode { get; init; } = ActionCodes.None;
		public ActionCodes NewActionCode { get; init; } = ActionCodes.None;
	}
}
