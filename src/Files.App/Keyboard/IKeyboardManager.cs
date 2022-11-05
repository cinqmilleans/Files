using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;

namespace Files.App.Keyboard
{
	public interface IKeyboardManager
	{
		public void FillKeyboard(IList<KeyboardAccelerator> accelerators);
		public void FillMenu(IList<KeyboardAccelerator> accelerators, KeyboardActionCodes actionCode);
	}
}
