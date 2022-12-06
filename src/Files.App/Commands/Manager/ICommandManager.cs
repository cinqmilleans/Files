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
		IRichCommand Properties { get; }
	}
}
