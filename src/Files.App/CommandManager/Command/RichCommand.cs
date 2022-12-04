using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.CommandManager
{
	public abstract class RichCommand : IRichCommand
	{
		public event EventHandler? CanExecuteChanged;

		private readonly ICommand command;

		public abstract CommandCodes Code { get; }
		public abstract string Label { get; }

		public virtual IGlyph Glyph => CommandManager.Glyph.None;
		public virtual HotKey HotKey => HotKey.None;

		protected ICommandContext Context { get; }

		public RichCommand(ICommandContext context)
		{
			Context = context;
			command = new AsyncRelayCommand(ExecuteAsync);
		}

		public bool CanExecute(object? parameter) => true;
		public void Execute(object? parameter) => command.Execute(parameter);

		protected abstract Task ExecuteAsync();
	}
}
