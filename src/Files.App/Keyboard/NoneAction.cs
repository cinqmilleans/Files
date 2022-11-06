namespace Files.App.Keyboard
{
	public class NoneAction : IKeyboardAction
	{
		public string Label => string.Empty;
		public string Description => string.Empty;

		public KeyboardActionCodes Code => KeyboardActionCodes.None;
		public ShortKey ShortKey => ShortKey.None;

		public void Execute() {}
	}
}
