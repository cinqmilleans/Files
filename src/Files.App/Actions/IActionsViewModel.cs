using Files.App.Actions.HotKeys;
using System.Collections.Generic;

namespace Files.App.Actions
{
	public interface IActionsViewModel : IEnumerable<IActionViewModel>
	{
		IActionContext Context { get; }

		IActionViewModel this[ActionCodes code] { get; }
		IActionViewModel this[HotKey hotKey] { get; }

		IActionViewModel None { get; }

		IActionViewModel Help { get; }
		IActionViewModel ToggleFullScreen { get; }

		IActionViewModel OpenFolderInNewTab { get; }
	}
}
