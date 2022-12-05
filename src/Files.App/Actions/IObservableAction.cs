﻿using System.ComponentModel;

namespace Files.App.Actions
{
	public interface IObservableAction : IAction, INotifyPropertyChanging, INotifyPropertyChanged
	{
		bool IsExecutable { get; }
	}
}
