using System.Windows.Input;

namespace Files.App.CommandManager
{
	public interface IRichCommand : ICommand
	{
		CommandCodes Code { get; }
		string Label { get; }

		IGlyph Glyph { get; }
		HotKey HotKey { get; }
	}
}
