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

		bool CanExecute { get; }
		ICommand Command { get; }

		Task ExecuteAsync();
	}
}
