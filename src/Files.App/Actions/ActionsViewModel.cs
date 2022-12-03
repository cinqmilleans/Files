using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Files.App.Actions.HotKeys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Actions
{
	internal class ActionsViewModel : IActionsViewModel
	{
		private static IDictionary<ActionCodes, ActionViewModel> actions = new Dictionary<ActionCodes, ActionViewModel>();

		private static readonly IHotKeyManager? hotKeyManager = Ioc.Default.GetService<IHotKeyManager>();

		public IActionContext Context { get; set; } = ActionContext.Empty;

		public IActionViewModel this[ActionCodes code] => actions[code];
		public IActionViewModel this[HotKey hotKey] => actions[hotKeyManager?[hotKey] ?? ActionCodes.None];

		public IActionViewModel None => actions[ActionCodes.None];
		public IActionViewModel Help => actions[ActionCodes.Help];
		public IActionViewModel FullScreen => actions[ActionCodes.FullScreen];
		public IActionViewModel OpenFolderInNewTab => actions[ActionCodes.OpenFolderInNewTab];

		public ActionsViewModel()
		{
			IActionFactory? factory = Ioc.Default.GetService<IActionFactory>();

			if (factory is not null)
				actions = Enum.GetValues<ActionCodes>()
					.ToImmutableDictionary(code => code, code => new ActionViewModel(this, factory.CreateAction(code)));

			if (hotKeyManager is not null)
				hotKeyManager.HotKeyChanged += HotKeyManager_HotKeyChanged;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<IActionViewModel> GetEnumerator() => actions.Values.GetEnumerator();

		private static void HotKeyManager_HotKeyChanged(IHotKeyManager _, HotKeyChangedEventArgs e)
		{
			if (actions.TryGetValue(e.OldActionCode, out ActionViewModel? value))
				value.UserHotKey = HotKey.None;
		}

		private class ActionViewModel : IActionViewModel
		{
			private readonly IActionsViewModel parent;
			private readonly IAction action;

			public ActionCodes Code => action.Code;
			public string Label => action.Label;

			public IGlyph Glyph => action.Glyph;

			public HotKey UserHotKey { get; set; }
			public HotKey DefaultHotKey => action.HotKey;

			public ICommand Command { get; }

			public ActionViewModel(IActionsViewModel parent, IAction action)
			{
				this.parent = parent;
				this.action = action;

				UserHotKey = action.HotKey;
				Command = new AsyncRelayCommand(ExecuteAsync, CanExecute);
			}

			public bool CanExecute() => action.CanExecute(parent.Context);
			public async Task ExecuteAsync() => await action.ExecuteAsync(parent.Context);
		}
	}
}
