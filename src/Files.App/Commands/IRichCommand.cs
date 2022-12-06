using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Commands
{
	public interface IRichCommand : ICommand, INotifyPropertyChanging, INotifyPropertyChanged
	{
		CommandCodes Code { get; }

		string Label { get; }
		IGlyph Glyph { get; }

		HotKey UserHotKey { get; }
		HotKey DefaultHotKey { get; }

		bool IsOn { get; }
		bool IsExecutable { get; }

		Task ExecuteAsync();
	}
}
