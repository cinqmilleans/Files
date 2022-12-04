using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.CommandManager
{
	public abstract class ObservableCommand : ObservableObject, IObservableCommand
	{
		public event EventHandler? CanExecuteChanged;

		private readonly ICommand command;

		public abstract CommandCodes Code { get; }
		public abstract string Label { get; }

		public virtual IGlyph Glyph => CommandManager.Glyph.None;
		public virtual HotKey HotKey => HotKey.None;

		private bool isExecutable = false;
		public bool IsExecutable => isExecutable;

		protected ICommandContext Context { get; }

		public ObservableCommand(ICommandContext context)
		{
			command = new AsyncRelayCommand(ExecuteAsync, CanExecute);

			Context = context;
			Context  .Changed += Context_Changed;
		}

		public bool CanExecute(object? parameter) => command.CanExecute(parameter);
		public void Execute(object? parameter) => command.Execute(parameter);

		protected virtual bool CanExecute() => true;
		protected abstract Task ExecuteAsync();

		protected virtual void OnContextChanged() {}

		private void Context_Changed(ICommandContext _, EventArgs e)
		{
			if (SetProperty(ref isExecutable, CanExecute(), nameof(IsExecutable)))
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			OnContextChanged();
		}
	}
}
