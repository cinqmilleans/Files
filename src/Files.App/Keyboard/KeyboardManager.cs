using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Windows.System;

namespace Files.App.Keyboard
{
	public class KeyboardManager : IKeyboardManager
	{
		private IImmutableDictionary<KeyboardActionCodes, IKeyboardAction> actions
			= ImmutableDictionary<KeyboardActionCodes, IKeyboardAction>.Empty;

		public IKeyboardAction this[KeyboardActionCodes code] => actions[code];

		public IEnumerable<IKeyboardAction> EnumerateActions() => actions.Values;

		public void Initialize(IEnumerable<IKeyboardAction> actions)
			=> this.actions = actions.ToImmutableDictionary(action => action.Code);

		public void FillKeyboard(UIElement element)
		{
			var accelerators = element.KeyboardAccelerators;
			accelerators.Clear();
			foreach (var action in actions.Values)
				accelerators.Add(new ActionKeyboardAccelerator(action));
		}

		public void FillMenu(UIElement element, KeyboardActionCodes code)
		{
			var action = actions[code];
			var accelerators = element.KeyboardAccelerators;

			accelerators.Clear();
			if (action.ShortKey.Key is not VirtualKey.None)
			{
				var accelerator = new KeyboardAccelerator
				{
					IsEnabled = false,
					Key = action.ShortKey.Key,
					Modifiers = action.ShortKey.Modifiers,
				};
				accelerators.Add(accelerator);
			}
		}

		private class ActionKeyboardAccelerator : KeyboardAccelerator
		{
			private readonly IKeyboardAction action;

			public ActionKeyboardAccelerator(IKeyboardAction action)
			{
				this.action = action;

				Invoked += ActionKeyboardAccelerator_Invoked;
				action.ShortKeyChanged += Action_ShortKeyChanged;

				Update();
			}

			private void ActionKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
			{
				action.Execute();
				e.Handled = true;
			}

			private void Action_ShortKeyChanged(object? _, EventArgs e) => Update();

			private void Update()
			{
				Key = action.ShortKey.Key;
				Modifiers = action.ShortKey.Modifiers;
				IsEnabled = Key is not VirtualKey.None;
			}
		}
	}
}
