using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Actions
{
	public interface IActionViewModel
	{
		string Label { get; }
		HotKey HotKey { get; }

		string Glyph { get; }
		string GlyphOverlay { get; }
		string GlyphFamily { get; }

		ICommand Command { get; }

		Task ExecuteAsync();
	}
}
