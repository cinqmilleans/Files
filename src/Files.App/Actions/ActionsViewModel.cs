using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Files.App.Actions.HotKeys;
using Files.Shared.Extensions;
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

		private IActionContext context = ActionContext.Empty;
		public IActionContext Context
		{
			get => context;
			set
			{
				context = value;
				actions.Values.ForEach(action => action.UpdateCanExecute());
			}
		}

		public IActionViewModel this[ActionCodes code] => actions[code];
		public IActionViewModel this[HotKey hotKey] => actions[hotKeyManager?[hotKey] ?? ActionCodes.None];

		public IActionViewModel None => actions[ActionCodes.None];
		public IActionViewModel Help => actions[ActionCodes.Help];
		public IActionViewModel FullScreen => actions[ActionCodes.FullScreen];
		public IActionViewModel LayoutDetails => actions[ActionCodes.LayoutDetails];
		public IActionViewModel LayoutTiles => actions[ActionCodes.LayoutTiles];
		public IActionViewModel LayoutGridSmall => actions[ActionCodes.LayoutGridSmall];
		public IActionViewModel LayoutGridMedium => actions[ActionCodes.LayoutGridMedium];
		public IActionViewModel LayoutGridLarge => actions[ActionCodes.LayoutGridLarge];
		public IActionViewModel LayoutColumns => actions[ActionCodes.LayoutColumns];
		public IActionViewModel LayoutAdaptive => actions[ActionCodes.LayoutAdaptive];
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

		private class ActionViewModel : ObservableObject, IActionViewModel
		{
			private readonly IActionsViewModel parent;
			private readonly IAction action;

			public ActionCodes Code => action.Code;
			public string Label => action.Label;

			public IGlyph Glyph => action.Glyph;

			public HotKey UserHotKey { get; set; }
			public HotKey DefaultHotKey => action.HotKey;

			private bool canExecute = false;
			public bool CanExecute
			{
				get => canExecute;
				private set => SetProperty(ref canExecute, value);
			}

			public ICommand Command { get; }

			public ActionViewModel(IActionsViewModel parent, IAction action)
			{
				this.parent = parent;
				this.action = action;

				UserHotKey = action.HotKey;
				Command = new AsyncRelayCommand(ExecuteAsync, CanExec);
				UpdateCanExecute();
			}

			public void UpdateCanExecute() => CanExecute = CanExec();

			public async Task ExecuteAsync() => await action.ExecuteAsync(parent.Context);
			private bool CanExec() => action.CanExecute(parent.Context);
		}
	}
}
