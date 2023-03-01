using Files.App.Commands;
using Files.App.Contexts;
using Files.App.Extensions;
using Windows.System;

namespace Files.App.Actions
{
	internal class LayoutAdaptiveAction : ToggleLayoutAction
	{
		public override string Label { get; } = "Adaptive".GetLocalizedResource();

		public override RichGlyph Glyph { get; } = new("\uF576");
		public override HotKey HotKey { get; } = new(VirtualKey.Number7, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift);

		protected override ContentLayoutTypes LayoutType => ContentLayoutTypes.Adaptive;
	}
}
