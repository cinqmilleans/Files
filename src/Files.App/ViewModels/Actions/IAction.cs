using Files.App.CommandActions;
using Files.App.DataModels.Glyphs;
using Files.App.DataModels.HotKeys;
using System.Threading.Tasks;

namespace Files.App.ViewModels.Actions
{
    public interface IAction
	{
		CommandCodes Code { get; }
		string Label { get; }

		IGlyph Glyph => DataModels.Glyphs.Glyph.None;
		HotKey HotKey => HotKey.None;

		Task ExecuteAsync();
	}
}
