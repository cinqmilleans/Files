using Files.App.Keyboard.Actions;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Windows.System;

namespace Files.App.Keyboard
{
	public class KeyboardManager : IKeyboardManager
	{
		private readonly IImmutableDictionary<KeyboardActionCodes, IKeyboardAction> actions;

		public KeyboardManager() => actions = CreateActions().ToImmutableDictionary(action => action.Code);

		public void FillKeyboard(IList<KeyboardAccelerator> accelerators)
		{
			foreach (var action in actions.Values)
				accelerators.Add(new ActionKeyboardAccelerator(action));
		}

		public void FillMenu(IList<KeyboardAccelerator> accelerators, KeyboardActionCodes actionCode)
		{
			var action = actions[actionCode];
			var accelerator = new KeyboardAccelerator
			{
				IsEnabled = false,
				Key = action.ShortKey.Key,
				Modifiers = action.ShortKey.Modifiers,
			};

			accelerators.Clear();
			accelerators.Add(accelerator);
		}

		private static IEnumerable<IKeyboardAction> CreateActions()
		{
			yield return new HelpAction();
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
				=> action.Execute();

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
