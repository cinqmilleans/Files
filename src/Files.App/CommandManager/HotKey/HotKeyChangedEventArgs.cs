using System;

namespace Files.App.CommandManager
{
	public class HotKeyChangedEventArgs : EventArgs
	{
		public HotKey OldHotKey { get; init; } = HotKey.None;
		public HotKey NewHotKey { get; init; } = HotKey.None;

		public CommandCodes OldCommandCode { get; init; } = CommandCodes.None;
		public CommandCodes NewCommandCode { get; init; } = CommandCodes.None;
	}
}
