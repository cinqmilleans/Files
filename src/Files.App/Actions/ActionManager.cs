using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;

namespace Files.App.Actions
{
	public class ActionManager : IActionManager
	{
		public event ActionEventHandler? ActionEvent;

		public static ActionManager Instance { get; } = new ActionManager();

		private ActionManager() {}

		public void Execute(Actions action)
		{
			var a = FocusManager.GetFocusedElement(App.Window.Content.XamlRoot);

			ActionEventHandler[] handlers =
				ActionEvent
				?.GetInvocationList()
				.OfType<ActionEventHandler>()
				.Reverse()
				.ToArray()
				?? Array.Empty<ActionEventHandler>();

			var args = new ActionEventArgs(action);

			foreach (ActionEventHandler handler in handlers)
			{
				handler(this, args);
				if (args.Handled)
					return;
			}
		}
	}
}
