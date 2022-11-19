using System.Threading.Tasks;

namespace Files.App.Actions
{
	public interface IAction
	{
		ActionCodes Code { get; }

		string Label { get; }
		HotKey HotKey => HotKey.None;

		string Glyph => string.Empty;
		string GlyphOverlay => string.Empty;
		string GlyphFamily => string.Empty;

		bool CanExecute() => true;
		Task ExecuteAsync();
	}
}
