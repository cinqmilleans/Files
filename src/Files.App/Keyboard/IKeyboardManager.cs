using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace Files.App.Keyboard
{
	public interface IKeyboardManager
	{
		void Initialize(IEnumerable<IKeyboardAction> actions);

		void FillKeyboard(UIElement element);
		void FillMenu(UIElement element, KeyboardActionCodes actionCode);
	}
}
