using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Files.App.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Commands
{
	internal class CommandManager : ICommandManager
	{
		private readonly IImmutableDictionary<CommandCodes, Command> commands = new Dictionary<CommandCodes, Command>
		{
			[CommandCodes.None] = new Command(new NoneAction()),
			[CommandCodes.Help] = new Command(new HelpAction()),
			[CommandCodes.FullScreen] = new Command(new FullScreenAction()),
			[CommandCodes.LayoutDetails] = new Command(new LayoutDetailsAction()),
			[CommandCodes.Properties] = new Command(new PropertiesAction()),
		}.ToImmutableDictionary();

		public IRichCommand this[CommandCodes commandCode]
			=> commands.TryGetValue(CommandCodes.None, out var command) ? command : None;

		public IRichCommand None => this[CommandCodes.None];
		public IRichCommand Help => this[CommandCodes.Help];
		public IRichCommand FullScreen => this[CommandCodes.FullScreen];
		public IRichCommand LayoutDetails => this[CommandCodes.LayoutDetails];
		public IRichCommand Properties => this[CommandCodes.Properties];

		public CommandManager()
		{
			var hotKeyManager = Ioc.Default.GetService<IHotKeyManager>();
			if (hotKeyManager is not null)
			{
				hotKeyManager.HotKeyChanged += HotKeyManager_HotKeyChanged;

				var commandCodes = Enum.GetValues<CommandCodes>();
				foreach (CommandCodes commandCode in commandCodes)
				{
					var command = commands[commandCode];
					var userHotKey = hotKeyManager[commandCode];
					if (userHotKey != command.UserHotKey)
						command.InitializeUserHotKey(userHotKey);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<IRichCommand> GetEnumerator() => commands.Values.GetEnumerator();

		private void HotKeyManager_HotKeyChanged(IHotKeyManager manager, HotKeyChangedEventArgs e)
		{
			commands[e.OldCommandCode].UpdateUserHotKey(manager[e.OldCommandCode]);
			commands[e.NewCommandCode].UpdateUserHotKey(manager[e.NewCommandCode]);
		}

		public class Command : ObservableObject, IRichCommand
		{
			public event EventHandler? CanExecuteChanged;

			private readonly IAction action;
			private readonly ICommand command;

			public CommandCodes Code => action.Code;

			public string Label => action.Label;
			public IGlyph Glyph => action.Glyph;

			private HotKey userHotKey;
			public HotKey UserHotKey => userHotKey;
			public HotKey DefaultHotKey => action.HotKey;

			public bool IsOn
				=> action is IToggleAction toggleAction && toggleAction.IsOn;
			public bool IsExecutable
				=> action is not IObservableAction observableAction || observableAction.IsExecutable;

			public Command(IAction action)
			{
				this.action = action;
				userHotKey = action.HotKey;

				command = action is IObservableAction observableAction
					? new AsyncRelayCommand(ExecuteAsync, () => observableAction.IsExecutable)
					: new AsyncRelayCommand(ExecuteAsync);

				if (action is INotifyPropertyChanging notifyPropertyChanging)
					notifyPropertyChanging.PropertyChanging += Action_PropertyChanging;
				if (action is INotifyPropertyChanged notifyPropertyChanged)
					notifyPropertyChanged.PropertyChanged += Action_PropertyChanged;
			}

			public bool CanExecute(object? parameter) => command.CanExecute(parameter);
			public void Execute(object? parameter) => command.Execute(parameter);

			public Task ExecuteAsync() => action.ExecuteAsync();

			private void Action_PropertyChanging(object? sender, PropertyChangingEventArgs e)
			{
				switch (e.PropertyName)
				{
					case nameof(IAction.Label):
						OnPropertyChanging(nameof(Label));
						break;
					case nameof(IAction.Glyph):
						OnPropertyChanging(nameof(Glyph));
						break;
					case nameof(IAction.HotKey):
						OnPropertyChanging(nameof(DefaultHotKey));
						break;
					case nameof(IToggleAction.IsOn):
						OnPropertyChanging(nameof(IsOn));
						break;
					case nameof(IObservableAction.IsExecutable):
						OnPropertyChanging(nameof(IsExecutable));
						break;
				}
			}
			private void Action_PropertyChanged(object? sender, PropertyChangedEventArgs e)
			{
				switch (e.PropertyName)
				{
					case nameof(IAction.Label):
						OnPropertyChanged(nameof(Label));
						break;
					case nameof(IAction.Glyph):
						OnPropertyChanged(nameof(Glyph));
						break;
					case nameof(IAction.HotKey):
						OnPropertyChanged(nameof(DefaultHotKey));
						break;
					case nameof(IToggleAction.IsOn):
						OnPropertyChanged(nameof(IsOn));
						break;
					case nameof(IObservableAction.IsExecutable):
						OnPropertyChanged(nameof(IsExecutable));
						CanExecuteChanged?.Invoke(this, EventArgs.Empty);
						break;
				}
			}

			public void InitializeUserHotKey(HotKey newUserHotKey)
				=> userHotKey = newUserHotKey;
			public void UpdateUserHotKey(HotKey newUserHotKey)
			{
				if (userHotKey != newUserHotKey)
				{
					userHotKey = newUserHotKey;
					OnPropertyChanged(nameof(UserHotKey));
				}
			}
		}
	}
}
