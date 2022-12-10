using System.Collections.Generic;

namespace Files.App.Commands
{
	public interface ICommandManager : IEnumerable<IRichCommand>
	{
		IRichCommand this[CommandCodes commandCode] { get; }

		IRichCommand None { get; }
		IRichCommand Help { get; }
		IRichCommand FullScreen { get; }

		IRichCommand LayoutDetails { get; }
		IRichCommand LayoutTiles { get; }
		IRichCommand LayoutGridSmall { get; }
		IRichCommand LayoutGridMedium { get; }
		IRichCommand LayoutGridLarge { get; }
		IRichCommand LayoutColumns { get; }
		IRichCommand LayoutAdaptive { get; }

		IRichCommand MultiSelect { get; }

		IRichCommand Rename { get; }
		IRichCommand Properties { get; }
	}
}
