using System;

namespace Files.App.Keyboard
{
	public interface IKeyboardAction
	{
		event EventHandler? ShortKeyChanged;

		string Code { get; }

		string Label { get; }
		string Description { get; }

		ShortKey ShortKey { get; set; }
		ShortKey DefaultShortKey { get; }

		void Execute();
	}
}
