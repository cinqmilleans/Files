using System.ComponentModel;

namespace Files.App.Commands
{
	public interface IObservableCommand : IRichCommand, INotifyPropertyChanging, INotifyPropertyChanged
	{
		bool IsExecutable { get; }
	}
}
