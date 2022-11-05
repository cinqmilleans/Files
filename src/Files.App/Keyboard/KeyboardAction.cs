using System;

namespace Files.App.Keyboard
{
	public abstract class KeyboardAction : IKeyboardAction
	{
		public event EventHandler? ShortKeyChanged;

		public abstract KeyboardActionCodes Code { get; }

		public abstract string Label { get; }
		public virtual string Description => string.Empty;

		private ShortKey shortKey;
		public ShortKey ShortKey
		{
			get => shortKey;
			set
			{
				if (shortKey != value)
				{
					shortKey = value;
					ShortKeyChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public virtual ShortKey DefaultShortKey => ShortKey.None;

		public KeyboardAction() => shortKey = DefaultShortKey;

		public abstract void Execute();
	}
}
