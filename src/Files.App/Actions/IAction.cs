using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Files.App.Actions.HotKeys;
using Files.App.Extensions;
using Files.App.Filesystem;
using Files.App.Helpers;
using Microsoft.UI.Windowing;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace Files.App.Actions
{
	public interface IAction
	{
		ActionCodes Code { get; }
		string Label { get; }

		IGlyph Glyph { get; }
		HotKey HotKey { get; }

		bool CanExecute(IActionContext context);
		Task ExecuteAsync(IActionContext context);
	}

	public enum CommandCodes
	{
		None,
		Help,
		ToggleFullScreen,
		Properties,
		LayoutDetails,
	}

	public interface IRichCommand : ICommand, INotifyPropertyChanged
	{
		CommandCodes Code { get; }
		string Label { get; }

		IGlyph Glyph { get; }
		HotKey HotKey { get; }

		bool IsExecutable { get; }
	}

	public interface IToggleCommand : IRichCommand
	{
		bool IsOn { get; set; }
	}

	public delegate void CommandContextChangedEventHandler(ICommandContext context, EventArgs e);

	public interface ICommandContext
	{
		event CommandContextChangedEventHandler? Changed;

		IShellPage? ShellPage { get; }
		IImmutableList<ListedItem> Items { get; }
	}

	public class CommandContext : ICommandContext
	{
		public event CommandContextChangedEventHandler? Changed;

		private IShellPage? shellPage;
		public IShellPage? ShellPage
		{
			get => shellPage;
			set
			{
				if (shellPage != value)
				{
					shellPage = value;
					OnChanged();
				}
			}
		}

		private IImmutableList<ListedItem> items = ImmutableList<ListedItem>.Empty;
		public IImmutableList<ListedItem> Items
		{
			get => items;
			set
			{
				if (items != value)
				{
					items = value;
					OnChanged();
				}
			}
		}

		private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
	}

	public class NoneCommand : IRichCommand
	{
		public event EventHandler? CanExecuteChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		public CommandCodes Code => CommandCodes.None;
		public string Label => string.Empty;

		public IGlyph Glyph => Actions.Glyph.None;
		public HotKey HotKey => HotKey.None;

		public bool IsExecutable => false;

		public bool CanExecute(object? parameter) => false;
		public void Execute(object? parameter) {}
	}

	public abstract class RichCommand : IRichCommand
	{
		public event EventHandler? CanExecuteChanged;
		public event PropertyChangedEventHandler? PropertyChanged;

		private readonly ICommand command;

		public abstract CommandCodes Code { get; }
		public abstract string Label { get; }

		public virtual IGlyph Glyph => Actions.Glyph.None;
		public virtual HotKey HotKey => HotKey.None;

		public bool IsExecutable => true;

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

	public abstract class ChangeableCommand : ObservableObject, IRichCommand
	{
		public event EventHandler? CanExecuteChanged;

		private readonly ICommand command;

		public abstract CommandCodes Code { get; }
		public abstract string Label { get; }

		public virtual IGlyph Glyph => Actions.Glyph.None;
		public virtual HotKey HotKey => HotKey.None;

		private bool isExecutable = false;
		public bool IsExecutable => isExecutable;

		protected ICommandContext Context { get; }

		public ChangeableCommand(ICommandContext context)
		{
			command = new AsyncRelayCommand(ExecuteAsync, CanExecute);

			Context = context;
			Context.Changed += Context_Changed;
		}

		public bool CanExecute(object? parameter) => command.CanExecute(parameter);
		public void Execute(object? parameter) => command.Execute(parameter);

		protected virtual bool CanExecute() => true;
		protected abstract Task ExecuteAsync();

		protected virtual void OnContextChanged() {}

		private void Context_Changed(ICommandContext context, EventArgs e)
		{
			if (SetProperty(ref isExecutable, CanExecute(), nameof(IsExecutable)))
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			OnContextChanged();
		}
	}

	public abstract class ToggleCommand : RichCommand, IToggleCommand
	{
		public abstract bool IsOn { get; set; }

		public ToggleCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			IsOn = !IsOn;
			return Task.CompletedTask;
		}
	}

	public abstract class ToggleChangeableCommand : ChangeableCommand, IToggleCommand
	{
		public override string Label => IsOn ? OnLabel : OffLabel;

		protected abstract string OnLabel { get; }
		protected abstract string OffLabel { get; }

		private bool isOn = false;
		public bool IsOn
		{
			get => isOn;
			set
			{
				if (SetProperty(ref isOn, value))
					OnPropertyChanged(nameof(Label));
			}
		}

		public ToggleChangeableCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			IsOn = !IsOn;
			return Task.CompletedTask;
		}
	}

	public class HelpCommand : RichCommand
	{
		public override CommandCodes Code => CommandCodes.Help;

		public override string Label => "Help";
		public override HotKey HotKey => new(VirtualKey.F1);

		public HelpCommand(ICommandContext context) : base(context) {}

		protected override async Task ExecuteAsync()
		{
			var url = new Uri(Constants.GitHub.DocumentationUrl);
			await Launcher.LaunchUriAsync(url);
		}
	}

	public class ToggleFullScreenCommand : ToggleCommand, IToggleCommand
	{
		public override CommandCodes Code => CommandCodes.ToggleFullScreen;

		public override string Label => "Full Screen";
		public override HotKey HotKey => new(VirtualKey.F11);

		public override bool IsOn
		{
			get
			{
				var window = App.GetAppWindow(App.Window);
				return window.Presenter.Kind is AppWindowPresenterKind.FullScreen;
			}
			set
			{
				var kind = value ? AppWindowPresenterKind.FullScreen : AppWindowPresenterKind.Overlapped;

				var window = App.GetAppWindow(App.Window);
				window.SetPresenter(kind);
			}
		}

		public ToggleFullScreenCommand(ICommandContext context) : base(context) {}
	}

	internal class LayoutDetailsCommand : RichCommand
	{
		public override CommandCodes Code => CommandCodes.LayoutDetails;
		public override string Label => "Details".GetLocalizedResource();

		public override HotKey HotKey => new(VirtualKey.Number1, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		public LayoutDetailsCommand(ICommandContext context) : base(context) {}

		protected override Task ExecuteAsync()
		{
			Context.ShellPage?.PaneHolder?.ActivePane?.InstanceViewModel?.FolderSettings?.ToggleLayoutModeDetailsView(true);
			return Task.CompletedTask;
		}
	}

	internal class PropertiesCommand : ChangeableCommand
	{
		public override CommandCodes Code => CommandCodes.Properties;
		public override string Label => "BaseLayoutItemContextFlyoutProperties/Text".GetLocalizedResource();

		public override IGlyph Glyph { get; } = new Glyph("\uF031", "\uF032");

		public PropertiesCommand(ICommandContext context) : base(context) { }

		protected override Task ExecuteAsync()
		{
			var flyout = Context.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				if (flyout.IsOpen)
					flyout.Closed += OpenProperties;
				else
					FilePropertiesHelpers.ShowProperties(Context.ShellPage!);
			}
			return Task.CompletedTask;
		}

		private void OpenProperties(object? sender, object e)
		{
			var flyout = Context.ShellPage?.SlimContentPage?.ItemContextMenuFlyout;
			if (flyout is not null)
			{
				flyout.Closed -= OpenProperties;
				FilePropertiesHelpers.ShowProperties(Context.ShellPage!);
			}
		}
	}
}
