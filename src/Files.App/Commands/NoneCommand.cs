using Files.App.CommandManager;
using System;

namespace Files.App.Commands
{
	internal class NoneCommand : IRichCommand
	{
		public event EventHandler? CanExecuteChanged;

		public CommandCodes Code => CommandCodes.None;
		public string Label => string.Empty;

		public IGlyph Glyph => CommandManager.Glyph.None;
		public HotKey HotKey => HotKey.None;

		public bool CanExecute(object? parameter) => false;
		public void Execute(object? parameter) { }
	}
}
