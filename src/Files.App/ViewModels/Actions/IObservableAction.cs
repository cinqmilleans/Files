using System.ComponentModel;

namespace Files.App.ViewModels.Actions
{
	public interface IObservableAction : IAction, INotifyPropertyChanging, INotifyPropertyChanged
	{
		bool IsExecutable { get; }
	}
}
