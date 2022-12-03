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
		IActionViewModel FullScreen { get; }

		IActionViewModel LayoutDetails { get; }
		IActionViewModel LayoutTiles { get; }
		IActionViewModel LayoutGridSmall { get; }
		IActionViewModel LayoutGridMedium { get; }
		IActionViewModel LayoutGridLarge { get; }
		IActionViewModel LayoutColumns { get; }
		IActionViewModel LayoutAdaptive { get; }

		IActionViewModel OpenFolderInNewTab { get; }
	}
}
