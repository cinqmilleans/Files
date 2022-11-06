using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Windows.System;

namespace Files.App.Keyboard
{
	public class KeyboardManager : IKeyboardManager
	{
		private readonly IKeyboardAction NoneAction = new NoneAction();

		private IImmutableDictionary<KeyboardActionCodes, IKeyboardAction> actions
			= ImmutableDictionary<KeyboardActionCodes, IKeyboardAction>.Empty;

		private IDictionary<ShortKey, IKeyboardAction> shortKeys = new Dictionary<ShortKey, IKeyboardAction>();

		private UIElement? keyboard;

		public IKeyboardAction this[KeyboardActionCodes code]
			=> actions.TryGetValue(code, out IKeyboardAction? action) ? action : NoneAction;
		public IKeyboardAction this[ShortKey shortKey]
			=> shortKeys.TryGetValue(shortKey, out IKeyboardAction? action) ? action : NoneAction;

		public ShortKeyStatus GetStatus(ShortKey shortKey) => shortKey switch
		{
			{ Key: VirtualKey.None } => ShortKeyStatus.Invalid,
			_ when shortKeys.ContainsKey(shortKey) => ShortKeyStatus.Used,
			_ => ShortKeyStatus.Available,
		};
		public void SetShortKey(KeyboardActionCodes code, ShortKey shortKey)
		{
			if (shortKeys.ContainsKey(shortKey))
			{
				shortKeys.Remove(shortKey);
				RemoveFromKeyboard(shortKey);
			}

			var oldShortKey = shortKeys.FirstOrDefault(s => s.Value.Code == code).Key;
			if (oldShortKey.Key is not VirtualKey.None)
			{
				shortKeys.Remove(oldShortKey);
				RemoveFromKeyboard(shortKey);
			}

			if (shortKey.Key is not VirtualKey.None && actions.TryGetValue(code, out IKeyboardAction? action))
			{
				shortKeys.Add(shortKey, action);
				AddToKeyboard(shortKey);
			}
		}

		public void Initialize(IEnumerable<IKeyboardAction> actions)
		{
			this.actions = actions.ToImmutableDictionary(action => action.Code);
			shortKeys = actions.Where(action => action.ShortKey.Key is not VirtualKey.None).ToDictionary(action => action.ShortKey);
		}

		public void RegisterKeyboard(UIElement keyboard)
		{
			this.keyboard = keyboard;
			keyboard.KeyboardAccelerators.Clear();
			foreach (var shortKey in shortKeys.Keys)
				AddToKeyboard(shortKey);
		}

		public void FillMenu(UIElement element, KeyboardActionCodes code)
		{
			element.KeyboardAccelerators.Clear();

			var shortKey = shortKeys.FirstOrDefault(s => s.Value.Code == code).Key;
			if (shortKey.Key is not VirtualKey.None)
			{
				var accelerator = new KeyboardAccelerator
				{
					IsEnabled = false,
					Key = shortKey.Key,
					Modifiers = shortKey.Modifiers,
				};
				element.KeyboardAccelerators.Add(accelerator);
			}
		}

		private void AddToKeyboard(ShortKey shortKey)
		{
			if (keyboard is null || shortKey.Key is VirtualKey.None)
				return;
			var accelerator = new KeyboardAccelerator
			{
				Key = shortKey.Key,
				Modifiers = shortKey.Modifiers,
			};
			accelerator.Invoked += Accelerator_Invoked;
			keyboard.KeyboardAccelerators.Add(accelerator);
		}
		private void RemoveFromKeyboard(ShortKey shortKey)
		{
			if (keyboard is null || shortKey.Key is VirtualKey.None)
				return;

			var accelerator = keyboard.KeyboardAccelerators
				.FirstOrDefault(a => a.Key == shortKey.Key && a.Modifiers == shortKey.Modifiers);
			if (accelerator is not null)
			{
				accelerator.Invoked -= Accelerator_Invoked;
				keyboard.KeyboardAccelerators.Remove(accelerator);
			}
		}

		private void Accelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
		{
			var shortKey = new ShortKey(sender.Key, sender.Modifiers);
			var action = this[shortKey];
			if (action.Code is not KeyboardActionCodes.None)
			{
				action.Execute();
				e.Handled = true;
			}
		}
	}
}
