using Files.App.Actions.HotKeys;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Actions
{
	public interface IActionViewModel
	{
		ActionCodes Code { get; }
		string Label { get; }
		IGlyph Glyph { get; }

		HotKey UserHotKey { get; }
		HotKey DefaultHotKey { get; }

		ICommand Command { get; }

		bool CanExecute();
		Task ExecuteAsync();
	}
}
