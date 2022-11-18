using System;

namespace Files.App.Actions
{
	public class ActionEventArgs : EventArgs
	{
		public Actions Action { get; }

		public bool Handled { get; set; }

		public ActionEventArgs(Actions action) => Action = action;
	}
}
