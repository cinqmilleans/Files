using System.ComponentModel;

namespace Files.App.CommandManager
{
	public interface IObservableCommand : IRichCommand, INotifyPropertyChanging, INotifyPropertyChanged
	{
		bool IsExecutable { get; }
	}
}
