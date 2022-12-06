﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Files.App.CommandActions;
using Files.App.DataModels.Glyphs;
using Files.App.DataModels.HotKeys;
using Files.App.ViewModels.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.ViewModels.Commands
{
    internal class CommandManager : ICommandManager
	{
		private static readonly IHotKeyManager? hotKeyManager = Ioc.Default.GetService<IHotKeyManager>();

		public IRichCommand this[CommandCodes actionCode] => actionCode switch
		{
			CommandCodes.Help => HelpCommand,
			CommandCodes.FullScreen => FullScreenCommand,
			CommandCodes.LayoutDetails => LayoutDetailsCommand,
			CommandCodes.Properties => PropertiesCommand,
			_ => NoneCommand,
		};

		public IRichCommand NoneCommand { get; } = new Command(new NoneAction());
		public IRichCommand HelpCommand { get; } = new Command(new HelpAction());
		public IToggleCommand FullScreenCommand { get; } = new ToggleCommand(new FullScreenAction());
		public IRichCommand LayoutDetailsCommand { get; } = new Command(new LayoutDetailsAction());
		public IRichCommand PropertiesCommand { get; } = new Command(new PropertiesAction());

		public IEnumerable<IRichCommand> EnumerateCommands() => Enum
			.GetValues<CommandCodes>()
			.Select(actionCode => this[actionCode])
			.Where(command => command.Code is not CommandCodes.None);

		public class Command : ObservableObject, IRichCommand
		{
			public event EventHandler? CanExecuteChanged;

			private readonly IAction action;
			private readonly ICommand command;

			public CommandCodes Code => action.Code;

			public string Label => action.Label;
			public IGlyph Glyph => action.Glyph;

			public HotKey UserHotKey => hotKeyManager?[action.Code] ?? HotKey.None;
			public HotKey DefaultHotKey => action.HotKey;

			public bool IsExecutable
				=> action is not IObservableAction observableAction || observableAction.IsExecutable;

			public Command(IAction action)
			{
				this.action = action;
				command = new AsyncRelayCommand(ExecuteAsync);

				if (hotKeyManager is not null)
					hotKeyManager.HotKeyChanged += HotKeyManager_HotKeyChanged;
			}
			public Command(IObservableAction action)
			{
				this.action = action;
				command = new AsyncRelayCommand(ExecuteAsync, () => action.IsExecutable);

				action.PropertyChanging += Action_PropertyChanging;
				action.PropertyChanged += Action_PropertyChanged;

				if (hotKeyManager is not null)
					hotKeyManager.HotKeyChanged += HotKeyManager_HotKeyChanged;
			}

			public bool CanExecute(object? parameter) => command.CanExecute(parameter);
			public void Execute(object? parameter) => command.Execute(parameter);

			public Task ExecuteAsync() => action.ExecuteAsync();

			private void Action_PropertyChanging(object? sender, PropertyChangingEventArgs e)
			{
				switch (e.PropertyName)
				{
					case nameof(IObservableAction.Label):
						OnPropertyChanging(nameof(Label));
						break;
					case nameof(IObservableAction.Glyph):
						OnPropertyChanging(nameof(Glyph));
						break;
					case nameof(IObservableAction.HotKey):
						OnPropertyChanging(nameof(HotKey));
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
					case nameof(IObservableAction.Label):
						OnPropertyChanged(nameof(Label));
						break;
					case nameof(IObservableAction.Glyph):
						OnPropertyChanged(nameof(Glyph));
						break;
					case nameof(IObservableAction.HotKey):
						OnPropertyChanged(nameof(HotKey));
						break;
					case nameof(IObservableAction.IsExecutable):
						OnPropertyChanged(nameof(IsExecutable));
						CanExecuteChanged?.Invoke(this, EventArgs.Empty);
						break;
				}
			}

			private void HotKeyManager_HotKeyChanged(IHotKeyManager manager, HotKeyChangedEventArgs e)
			{
				if (action.Code == e.OldActionCode || action.Code == e.NewActionCode)
					OnPropertyChanged(nameof(UserHotKey));
			}
		}

		public class ToggleCommand : Command, IToggleCommand
		{
			private readonly IToggleAction action;

			public bool IsOn
			{
				get => action.IsOn;
				set => action.IsOn = value;
			}

			public ToggleCommand(IToggleAction action) : base(action) => this.action = action;

			public void Toggle() => IsOn = !IsOn;
		}
	}
}
