using Files.App.CommandActions;
using System;

namespace Files.App.DataModels.HotKeys
{
    public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey OldHotKey { get; init; } = HotKey.None;
		public HotKey NewHotKey { get; init; } = HotKey.None;

		public CommandCodes OldActionCode { get; init; } = CommandCodes.None;
		public CommandCodes NewActionCode { get; init; } = CommandCodes.None;
	}
}
