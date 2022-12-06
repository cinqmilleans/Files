using System.Threading.Tasks;

namespace Files.App.Commands
{
	public interface IAction
	{
		CommandCodes Code { get; }
		string Label { get; }

		IGlyph Glyph => Commands.Glyph.None;
		HotKey HotKey => HotKey.None;

		Task ExecuteAsync();
	}
}
