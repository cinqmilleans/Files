using Files.App.DataModels;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Commands
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
