using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Files.App.Actions.HotKeys;

namespace Files.App.Actions
{
	public interface IActionViewModel
	{
		ActionCodes Code { get; }
		string Label { get; }

		HotKey UserHotKey { get; }
		HotKey DefaultHotKey { get; }

		string Glyph { get; }
		string GlyphOverlay { get; }
		string GlyphFamily { get; }

		ICommand Command { get; }

		bool CanExecute() => true;
		Task ExecuteAsync();
	}
}
