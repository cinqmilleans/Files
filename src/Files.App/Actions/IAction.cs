using Files.App.DataModels;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	public interface IAction
	{
		ActionCodes Code { get; }
		string Label { get; }

		IGlyph Glyph => DataModels.Glyph.None;
		HotKey HotKey => HotKey.None;

		Task ExecuteAsync();
	}
}
