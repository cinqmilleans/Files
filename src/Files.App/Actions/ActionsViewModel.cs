using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Files.App.Actions.HotKeys;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Files.App.Actions
{
	internal class ActionsViewModel : IActionsViewModel
	{
		private readonly IImmutableDictionary<ActionCodes, ActionViewModel> actions;

		private readonly IHotKeyManager hotKeyManager = new HotKeyManager();

		public IActionViewModel None => actions[ActionCodes.None];
		public IActionViewModel Help => actions[ActionCodes.Help];
		public IActionViewModel OpenFolderInNewTab => actions[ActionCodes.OpenFolderInNewTab];

		public ActionsViewModel()
		{
			IActionFactory factory = new ActionFactory(null);

			actions = Enum.GetValues<ActionCodes>()
				.ToImmutableDictionary(code => code, code => new ActionViewModel(factory.CreateAction(code)));

			hotKeyManager.HotKeyChanged += HotKeyManager_HotKeyChanged;
		}

		public HotKeyStatus GetStatus(HotKey hotkey) => hotKeyManager.GetStatus(hotkey);
		public IActionViewModel GetAction(HotKey hotkey) => actions[hotKeyManager.GetActionCode(hotkey)];

		private void HotKeyManager_HotKeyChanged(IHotKeyManager manager, HotKeyChangedEventArgs e)
		{
			if (actions.ContainsKey(e.OldActionCode))
				actions[e.OldActionCode].UserHotKey = HotKey.None;
		}

		private class ActionViewModel : IActionViewModel
		{
			private readonly IAction action;

			public ActionCodes Code => action.Code;
			public string Label => action.Label;

			public HotKey UserHotKey { get; set; }
			public HotKey DefaultHotKey => action.HotKey;

			public string Glyph => action.Glyph;
			public string GlyphOverlay => action.GlyphOverlay;
			public string GlyphFamily => action.GlyphFamily;

			public ICommand Command { get; }

			public ActionViewModel(IAction action)
			{
				this.action = action;
				UserHotKey = action.HotKey;
				Command = new AsyncRelayCommand(action.ExecuteAsync, action.CanExecute);
			}

			public bool CanExecute() => action.CanExecute();
			public async Task ExecuteAsync() => await action.ExecuteAsync();
		}
	}
}
