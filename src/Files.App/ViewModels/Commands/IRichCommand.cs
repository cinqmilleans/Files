using Files.App.DataModels.Glyphs;
using Files.App.DataModels.HotKeys;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.ViewModels.Commands
{
	public interface IRichCommand : ICommand, INotifyPropertyChanging, INotifyPropertyChanged
	{
		string Label { get; }
		IGlyph Glyph { get; }

		HotKey UserHotKey { get; }
		HotKey DefaultHotKey { get; }

		bool IsExecutable { get; }

		Task ExecuteAsync();
	}
}
