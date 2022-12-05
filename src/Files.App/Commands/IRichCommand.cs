using Files.App.DataModels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Commands
{
	public interface IRichCommand : ICommand
	{
		string Label { get; }

		IGlyph Glyph { get; }
		HotKey HotKey { get; }

		Task ExecuteAsync();
	}
}
