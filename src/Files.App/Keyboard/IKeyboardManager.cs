using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Files.App.Keyboard
{
	public interface IKeyboardManager
	{
		IKeyboardAction this[KeyboardActionCodes code] { get; }

		IEnumerable<IKeyboardAction> EnumerateActions();

		void Initialize(IEnumerable<IKeyboardAction> actions);

		void FillKeyboard(UIElement element);
		void FillMenu(UIElement element, KeyboardActionCodes code);
	}
}
