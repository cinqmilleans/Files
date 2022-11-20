using Files.App.Actions.HotKeys;
using System.Collections.Generic;

namespace Files.App.Actions
{
	public interface IActionsViewModel : IEnumerable<IActionViewModel>
	{
		IActionViewModel None { get; }

		IActionViewModel Help { get; }

		IActionViewModel OpenFolderInNewTab { get; }

		HotKeyStatus GetStatus(HotKey hotkey);
		IActionViewModel GetAction(HotKey hotkey);
	}
}
