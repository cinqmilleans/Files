using System.ComponentModel;

namespace Files.App.Commands
{
	public interface IObservableAction : IAction, INotifyPropertyChanging, INotifyPropertyChanged
	{
		bool IsExecutable { get; }
	}
}
