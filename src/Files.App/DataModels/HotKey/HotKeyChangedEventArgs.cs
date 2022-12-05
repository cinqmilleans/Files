using System;
using Files.App.Actions;

namespace Files.App.DataModels
{
	public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey OldHotKey { get; init; } = HotKey.None;
		public HotKey NewHotKey { get; init; } = HotKey.None;

		public ActionCodes OldActionCode { get; init; } = ActionCodes.None;
		public ActionCodes NewActionCode { get; init; } = ActionCodes.None;
	}
}
