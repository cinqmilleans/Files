using Files.App.Actions.HotKeys;
using System.Threading.Tasks;

namespace Files.App.Actions
{
	public interface IAction
	{
		ActionCodes Code { get; }
		string Label { get; }

		IGlyph Glyph { get; }
		HotKey HotKey { get; }

		bool CanExecute(IActionContext context);
		Task ExecuteAsync(IActionContext context);
	}
}
